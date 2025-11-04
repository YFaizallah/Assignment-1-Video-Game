using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum Kind { Key, Battery }
    public Kind type = Kind.Key;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var inv = other.GetComponent<PlayerInventory>();
        if (!inv) inv = other.gameObject.AddComponent<PlayerInventory>();
        if (type == Kind.Key) inv.AddKey(); else inv.AddBattery();
        Destroy(gameObject);
    }
}
