using UnityEngine;

public class LockedDevice : MonoBehaviour
{
    public DoorController1 door;   // assign your hinge (DoorRoot/Cylinder with DoorController)
    public bool needsKey = true;  // if false: needs Battery

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var inv = other.GetComponent<PlayerInventory>();
        bool ok = inv && (needsKey ? inv.HasKey : inv.HasBattery);
        UIToast.Show(ok ? "Access granted" : "Missing required item");
        if (ok && door) door.Open();

    }
}
