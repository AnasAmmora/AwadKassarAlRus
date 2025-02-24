using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 20f; // Base movement speed
    public float acceleration = 50f; // Acceleration force

    [Header("Tilting")]
    public float tiltSpeed = 100f; // Speed of tilting
    public float maxTiltAngle = 30f; // Max tilt angle in degrees

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleMovement();
        HandleTilting();
    }

    void HandleMovement()
    {
        // Forward/Backward movement using AddForce (A/D or Arrow Keys)
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddForce(-transform.forward * acceleration, ForceMode.Acceleration);
        }

        // Speed clamp for arcade feel
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, moveSpeed);
    }

    void HandleTilting()
    {
        // Get target tilt based on input
        float targetTilt = 0f;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            targetTilt = maxTiltAngle; // Tilt forward
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            targetTilt = -maxTiltAngle; // Tilt backward
        }

        // Smoothly rotate the car toward the target tilt
        Quaternion targetRotation = Quaternion.Euler(targetTilt, transform.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, tiltSpeed * Time.deltaTime);
    }
}