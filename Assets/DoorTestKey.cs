using UnityEngine;
public class DoorTestKey : MonoBehaviour
{
    public DoorController1 door;
    public KeyCode key = KeyCode.T;
    void Reset() { if (!door) door = GetComponent<DoorController1>(); }
    void Update() { if (Input.GetKeyDown(key)) door.ToggleDoor(); }
}
