using UnityEngine;

public class DoorAutoOpenTrigger : MonoBehaviour
{
    public DoorController1 door;
    public bool closeOnExit = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) door.Open();
    }

    void OnTriggerExit(Collider other)
    {
        if (closeOnExit && other.CompareTag("Player")) door.Close();
    }
}
