using UnityEngine;
using TMPro;

public class DoorPromptTrigger : MonoBehaviour
{
    [Header("Door + UI")]
    public DoorController1 door;            // The hinge object with DoorController
    public TextMeshProUGUI promptText;     // The screen-space TMP text (inside Canvas)
    public KeyCode key = KeyCode.T;

    [Header("Access")]
    public bool requiresKey = true;        // Turn off if you don’t want gating later
    public string lockedMessage = "Door locked — need key";

    bool playerInside;
    PlayerInventory inv;                   // Cached while player is inside
    bool lastHasKey;                       // For live prompt updates if inventory changes

    void Awake()
    {
        if (promptText) promptText.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (promptText) promptText.gameObject.SetActive(false);
        playerInside = false;
        inv = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;
        inv = other.GetComponent<PlayerInventory>();           // okay if null; we handle it
        lastHasKey = inv && inv.HasKey;
        RefreshPrompt();                                       // show correct message immediately
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        if (promptText) promptText.gameObject.SetActive(false);
        inv = null;
    }

    void Update()
    {
        if (!playerInside) return;

        // If player picks the key while still in the zone, update the text live.
        bool hasKeyNow = inv && inv.HasKey;
        if (hasKeyNow != lastHasKey) { lastHasKey = hasKeyNow; RefreshPrompt(); }

        if (Input.GetKeyDown(key))
        {
            // Enforce key requirement
            if (requiresKey && !(inv && inv.HasKey))
            {
                UIToast.Show("Door is locked. Find a key.");
                return;
            }

            // Toggle door and update hint
            if (door) door.ToggleDoor();
            RefreshPrompt();
        }
    }

    void RefreshPrompt()
    {
        if (!promptText) return;

        if (requiresKey && !(inv && inv.HasKey))
        {
            promptText.text = lockedMessage;                   // “Door locked — need key”
        }
        else
        {
            promptText.text = door && door.isOpen
                ? "Press T to close door"
                : "Press T to open door";
        }

        promptText.gameObject.SetActive(true);
    }
}
