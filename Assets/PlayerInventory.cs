using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool HasKey { get; private set; }
    public bool HasBattery { get; private set; }

    public void AddKey() { HasKey = true; UIToast.Show("Key collected"); }
    public void AddBattery() { HasBattery = true; UIToast.Show("Battery collected"); }
}
