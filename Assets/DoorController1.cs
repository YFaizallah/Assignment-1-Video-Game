using UnityEngine;

public class DoorController1 : MonoBehaviour
{
    public enum OpenMode { RotateAroundY, SlideUp }
    [Header("Motion")]
    public OpenMode mode = OpenMode.RotateAroundY;
    public Transform movingPart;      // Assign the panel (your Cube)
    public float openAmount = 90f;    // degrees (Rotate) or meters (Slide)
    public float speed = 4f;

    [Header("State")]
    public bool isOpen = false;

    // Internals
    Vector3 closedPos, openPos;
    Quaternion closedRot, openRot;

    void Reset()
    {
        if (!movingPart) movingPart = transform; // fallback
    }

    void Start()
    {
        if (!movingPart) movingPart = transform;

        closedPos = movingPart.position;
        closedRot = movingPart.rotation;

        if (mode == OpenMode.SlideUp)
        {
            openPos = closedPos + Vector3.up * openAmount;
            openRot = closedRot;
        }
        else
        {
            openPos = closedPos;
            openRot = Quaternion.Euler(movingPart.eulerAngles + new Vector3(0f, openAmount, 0f));
        }
    }

    void Update()
    {
        if (mode == OpenMode.SlideUp)
        {
            movingPart.position = Vector3.Lerp(
                movingPart.position,
                isOpen ? openPos : closedPos,
                Time.deltaTime * speed
            );
        }
        else
        {
            movingPart.rotation = Quaternion.Slerp(
                movingPart.rotation,
                isOpen ? openRot : closedRot,
                Time.deltaTime * speed
            );
        }
    }

    // Call from trigger or key interaction
    public void ToggleDoor() => isOpen = !isOpen;
    public void Open() => isOpen = true;
    public void Close() => isOpen = false;
}
