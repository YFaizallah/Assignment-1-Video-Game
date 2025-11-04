using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [SerializeField] Light spot;
    void Reset() { if (!spot) spot = GetComponentInChildren<Light>(true); }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            spot.enabled = !spot.enabled;
            UIToast.Show(spot.enabled ? "Flashlight ON" : "Flashlight OFF");
        }
    }
}
