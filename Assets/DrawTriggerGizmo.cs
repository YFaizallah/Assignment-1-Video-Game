using UnityEngine;
public class DrawTriggerGizmo : MonoBehaviour
{
    void OnDrawGizmos()
    {
        var c = GetComponent<BoxCollider>();
        if (!c) return;
        Gizmos.color = Color.green;
        var m = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = m;
        Gizmos.DrawWireCube(c.center, c.size);
    }
}
