// CameraModeSwitcher_CM3.cs
using UnityEngine;
using Unity.Cinemachine;     // <— IMPORTANT for CM3

public class CameraSwitcher : MonoBehaviour
{
    [Header("Assign your two CM3 cameras")]
    public CinemachineCamera fpCam;   // FPP camera
    public CinemachineCamera tpCam;   // TPP camera

    [Header("Hotkey")]
    public KeyCode toggleKey = KeyCode.C;

    public int activePriority = 20;
    public int inactivePriority = 10;

    [Header("Optional: hide head in FP")]
    public Renderer[] headRenderers;

    bool isFP = true;

    void Start() => Apply(isFP, force: true);

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isFP = !isFP;
            Apply(isFP, force: false);
        }
    }

    void Apply(bool fp, bool force)
    {
        if (!fpCam || !tpCam) return;

        fpCam.Priority = fp ? activePriority : inactivePriority;
        tpCam.Priority = fp ? inactivePriority : activePriority;

        if (headRenderers != null)
            foreach (var r in headRenderers) if (r) r.enabled = !fp;

        if (fp || force) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
    }
}
