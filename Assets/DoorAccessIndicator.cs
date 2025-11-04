using UnityEngine;

public class DoorAccessIndicator : MonoBehaviour
{
    [Header("References")]
    public DoorController1 door;      // your hinge object with DoorController1
    public Light lampRed;             // child point light under red_bulb
    public Light lampYellow;          // child point light under yellow_bulb
    public Light lampGreen;           // child point light under green_bulb

    [Header("Access")]
    public bool requiresKey = true;   // your door requires a key
    private PlayerInventory inv;      // read HasKey from player

    void Start()
    {
        // Start with all off
        SetLamps(false, false, false);

        // Cache player inventory (object tagged Player)
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player) inv = player.GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (!door) return;

        bool hasKey = !requiresKey || (inv && inv.HasKey);

        if (door.isOpen)
        {
            // Open -> GREEN
            SetLamps(false, false, true);
        }
        else
        {
            if (!hasKey)
            {
                // Closed & locked -> RED
                SetLamps(true, false, false);
            }
            else
            {
                // Closed & unlocked (key owned) -> YELLOW
                SetLamps(false, true, false);
            }
        }
    }

    void SetLamps(bool red, bool yellow, bool green)
    {
        if (lampRed) lampRed.enabled = red;
        if (lampYellow) lampYellow.enabled = yellow;
        if (lampGreen) lampGreen.enabled = green;
    }
}
