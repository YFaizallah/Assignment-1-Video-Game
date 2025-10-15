using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem; // for PlayerInput + Keyboard

public class PerspectiveSwitcher : MonoBehaviour
{
    [Header("Characters (roots in scene)")]
    public GameObject thirdPersonRoot;   // PlayerArmature
    public GameObject firstPersonRoot;   // PlayerCapsule

    [Header("Virtual Cameras (assign the GameObjects)")]
    public GameObject thirdPersonCamGO;  // TP vcam object
    public GameObject firstPersonCamGO;  // FP vcam object

    [Header("Hotkey")]
    public KeyCode toggleKey = KeyCode.C;

    [Header("Pose Sync")]
    public bool copyYawOnly = true;
    public float fpOffsetForward = 0f;

    [Header("Input / Cursor")]
    public string actionMapName = "Player";
    public bool lockCursorInFP = true;

    // cached comps
    private PlayerInput tpInput, fpInput;
    private Behaviour tpController, fpController;           // ThirdPersonController / FirstPersonController
    private MonoBehaviour tpStarterInputs, fpStarterInputs; // StarterAssetsInputs

    void Awake()
    {
        // Cache components we’ll toggle
        tpInput = thirdPersonRoot ? thirdPersonRoot.GetComponentInChildren<PlayerInput>(true) : null;
        fpInput = firstPersonRoot ? firstPersonRoot.GetComponentInChildren<PlayerInput>(true) : null;

        tpController = FindController(thirdPersonRoot, "ThirdPersonController");
        fpController = FindController(firstPersonRoot, "FirstPersonController");

        tpStarterInputs = FindByTypeName(thirdPersonRoot, "StarterAssetsInputs");
        fpStarterInputs = FindByTypeName(firstPersonRoot, "StarterAssetsInputs");

        // Start in TPP
        SetActiveRig(isThirdPerson: true);

        // Priorities (CM2/CM3 via reflection)
        SetPriority(thirdPersonCamGO, 10);
        SetPriority(firstPersonCamGO, 9);
    }

    void Update()
    {
        if ((Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame) || Input.GetKeyDown(toggleKey))
            Toggle();
    }

    void Toggle()
    {
        bool toFP = !firstPersonRoot.activeSelf;

        // sync pose so you don't teleport weirdly
        Transform from = toFP ? thirdPersonRoot.transform : firstPersonRoot.transform;
        Transform to = toFP ? firstPersonRoot.transform : thirdPersonRoot.transform;

        Vector3 pos = from.position;
        Quaternion rot = from.rotation;
        if (copyYawOnly)
        {
            var f = from.forward; f.y = 0f;
            rot = f.sqrMagnitude > 1e-6f ? Quaternion.LookRotation(f.normalized, Vector3.up)
                                         : Quaternion.Euler(0f, from.eulerAngles.y, 0f);
        }
        if (toFP && fpOffsetForward != 0f) pos += rot * Vector3.forward * fpOffsetForward;

        var cc = to.GetComponent<CharacterController>();
        if (cc) { cc.enabled = false; to.SetPositionAndRotation(pos, rot); cc.enabled = true; }
        else { to.SetPositionAndRotation(pos, rot); }

        // flip rig, inputs, controllers, cursor, and action maps
        SetActiveRig(isThirdPerson: !toFP);

        // swap CM priorities
        SetPriority(firstPersonCamGO, toFP ? 10 : 9);
        SetPriority(thirdPersonCamGO, toFP ? 9 : 10);
    }

    void SetActiveRig(bool isThirdPerson)
    {
        if (thirdPersonRoot) thirdPersonRoot.SetActive(isThirdPerson);
        if (firstPersonRoot) firstPersonRoot.SetActive(!isThirdPerson);

        // Enable exactly one PlayerInput + controller
        Enable(tpInput, isThirdPerson);
        Enable(fpInput, !isThirdPerson);
        Enable(tpController, isThirdPerson);
        Enable(fpController, !isThirdPerson);

        // Ensure the active rig’s input is fully re-armed + correct action map
        if (isThirdPerson)
        {
            ArmInput(tpInput, actionMapName);
            SetCursorState(tpStarterInputs, locked: true);          // TP also usually wants cursor locked
        }
        else
        {
            ArmInput(fpInput, actionMapName);
            SetCursorState(fpStarterInputs, locked: lockCursorInFP);
        }
    }

    // --- helpers ---

    static Behaviour FindController(GameObject root, string typeName)
    {
        if (!root) return null;
        return root.GetComponentsInChildren<MonoBehaviour>(true)
                   .FirstOrDefault(m => m && m.GetType().Name == typeName) as Behaviour;
    }

    static MonoBehaviour FindByTypeName(GameObject root, string typeName)
    {
        if (!root) return null;
        return root.GetComponentsInChildren<MonoBehaviour>(true)
                   .FirstOrDefault(m => m && m.GetType().Name == typeName);
    }

    static void ArmInput(PlayerInput pi, string map)
    {
        if (!pi) return;
        // reacquire actions
        try
        {
            if (pi.currentActionMap == null || pi.currentActionMap.name != map)
                pi.SwitchCurrentActionMap(map);
            pi.ActivateInput();
        }
        catch { /* ignore if using old Input Manager */ }
    }

    static void SetCursorState(MonoBehaviour starterInputs, bool locked)
    {
        // lock OS cursor (Starter Assets reads this)
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;

        // also set StarterAssetsInputs fields if present (names vary slightly by package)
        if (!starterInputs) return;
        var t = starterInputs.GetType();
        var f1 = t.GetField("cursorLocked", BindingFlags.Instance | BindingFlags.Public);
        var f2 = t.GetField("cursorInputForLook", BindingFlags.Instance | BindingFlags.Public);
        if (f1 != null) f1.SetValue(starterInputs, locked);
        if (f2 != null) f2.SetValue(starterInputs, true);
        // if it has a SetCursorState(bool) method, call it
        var m = t.GetMethod("SetCursorState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (m != null) m.Invoke(starterInputs, new object[] { locked });
    }

    static void Enable(Behaviour b, bool on) { if (b) b.enabled = on; }

    static void SetPriority(GameObject camGO, int value)
    {
        if (!camGO) return;
        var cm = camGO.GetComponent("CinemachineCamera")           // CM3
              ?? camGO.GetComponent("CinemachineVirtualCamera");   // CM2
        if (cm == null) return;
        var prop = cm.GetType().GetProperty("Priority");
        if (prop != null && prop.CanWrite) prop.SetValue(cm, value, null);
    }
}
