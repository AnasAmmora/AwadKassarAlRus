using UnityEngine;

public class AttachRoof : MonoBehaviour
{
    [SerializeField] private Rigidbody carRB; // Assign your car's Rigidbody in the Inspector

    private FixedJoint joint;

    void Start()
    {
        joint = gameObject.AddComponent<FixedJoint>(); // Add FixedJoint
        joint.connectedBody = carRB; // Attach to the car's Rigidbody
        joint.breakForce = Mathf.Infinity; // Never breaks
        joint.breakTorque = Mathf.Infinity;
    }
}
