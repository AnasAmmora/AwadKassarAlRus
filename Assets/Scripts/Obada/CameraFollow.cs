using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The object the camera will follow
    public Vector3 offset = new Vector3(0, 2, -10); // Offset from the target

    void LateUpdate()
    {
        if (target != null)
        {
            // Set the camera's position to the target's position plus the offset
            transform.position = target.position + offset;

            // Make the camera look at the target
            transform.LookAt(target);
        }
    }
}