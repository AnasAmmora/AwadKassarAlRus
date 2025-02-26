using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationPoint;
    [SerializeField] private GameObject[] tires = new GameObject[4];
    [SerializeField] private ParticleSystem[] skidSmokes = new ParticleSystem[2];
    [SerializeField] private AudioSource engineSound, skidSound;

    [Header("Suspension Settings")]
    [SerializeField] private float restLength; // spring length // 1
    [SerializeField] private float springTravel; // spring max compress or extend distance // 0.5
    [SerializeField] private float springStiffness; // spring max force that can be exerted (when fully compressed) // 30000
    [SerializeField] private float damperStiffness; // 3000
    [SerializeField] private float WheelRadius; // 0.33

    private int[] wheelsIsGrounded = new int[4];
    private bool isGrounded = false;

    private int moveInput;
    private bool isBraking;

    [Header("Car Settings")]
    [SerializeField] private float maxSpeed = 100f; // 100
    [SerializeField] private float acceleration = 25f; // 25
    [SerializeField] private float deceleration = 10f; // 10

    private Vector3 currentCarLocalVelocity = Vector3.zero;
    private float carVelocityRatio = 0f;

    [Header("Visuals")]
    [SerializeField] private float minSideSkidVelocity = 10f; // 10

    [Header("Audio")]
    [SerializeField]
    [Range(0, 1)] private float minPitch = 1f;
    [SerializeField]
    [Range(1, 5)] private float maxPitch = 5f;

    public void SetMoveInput(int input)
    {
        moveInput = input;
    }

    #region Main functions
    void Start()
    {
        carRB = GetComponent<Rigidbody>();
        moveInput = 0;
        isBraking = false;

    }

    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
        Visuals();
        EngineSound();
        CheckForUpsideDown(); // Check and stabilize when upside down
    }
    #endregion

    #region Innovations
    public void OnUpButtonPressed()
    {
        moveInput = 1;
    }

    public void OnDownButtonPressed()
    {
        moveInput = -1;
    }

    public void OnMoveButtonReleased()
    {
        moveInput = 0;
    }
    #endregion

    #region Movement
    private void Movement()
    {
        if (isGrounded)
        {
            Acceleration();
            Deceleration();
        }
    }

    private void Acceleration()
    {
        if (currentCarLocalVelocity.z < maxSpeed)
        {
            carRB.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
        }
    }

    private void Deceleration()
    {
        carRB.AddForceAtPosition(deceleration * carVelocityRatio * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }
    #endregion

    #region Visuals
    private void Visuals()
    {
        Vfx();
    }

    private void Vfx()
    {
        if (isGrounded && Mathf.Abs(currentCarLocalVelocity.x) > minSideSkidVelocity && isBraking)
        {
            ToggleSkidSmokes(true);
            ToggleSlidSound(true);
        }
        else
        {
            ToggleSkidSmokes(false);
            ToggleSlidSound(false);
        }
    }

    private void ToggleSkidSmokes(bool toggle)
    {
        foreach (var smoke in skidSmokes)
        {
            if (toggle)
            {
                smoke.Play();
            }
            else
            {
                smoke.Stop();
            }
        }
    }

    private void SetTirePosition(GameObject tire, Vector3 targetPosition)
    {
        tire.transform.position = targetPosition;
    }
    #endregion

    #region Audio
    private void EngineSound()
    {
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(carVelocityRatio));
    }

    private void ToggleSlidSound(bool toggle)
    {
        skidSound.mute = !toggle;
    }
    #endregion

    #region Car Status Check
    private void GroundCheck()
    {
        int tempGroundedWheels = 0;
        for (int i = 0; i < wheelsIsGrounded.Length; i++)
        {
            tempGroundedWheels += wheelsIsGrounded[i];
        }
        isGrounded = tempGroundedWheels > 1;
    }

    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRB.velocity);
        carVelocityRatio = Mathf.Clamp01(currentCarLocalVelocity.z / maxSpeed);
    }
    #endregion

    #region Suspension Functions
    private void Suspension()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;
            bool isWheelOnGround = Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxLength + WheelRadius, drivable);

            if (isWheelOnGround)
            {
                wheelsIsGrounded[i] = 1;
                float currentSpringLength = hit.distance - WheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                // Apply less damping when airborne
                float springVelocity = Vector3.Dot(carRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float airDampingFactor = isGrounded ? 1f : 0.2f;
                float dampForce = damperStiffness * springVelocity * airDampingFactor;

                float springForce = springStiffness * springCompression;
                float netForce = springForce - dampForce;

                carRB.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);

                SetTirePosition(tires[i], hit.point + rayPoints[i].up * WheelRadius);
            }
            else
            {
                wheelsIsGrounded[i] = 0;
                SetTirePosition(tires[i], rayPoints[i].position - rayPoints[i].up * maxLength);
            }
        }
    }
    #endregion

    #region Collision Handling
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Oponent") || collision.gameObject.CompareTag("Player"))
        {
            Rigidbody otherRB = collision.gameObject.GetComponent<Rigidbody>();

            if (otherRB != null)
            {
                Vector3 impactDirection = (collision.transform.position - transform.position).normalized;
                impactDirection.y = 1f; // Apply an upward push to help flipping

                // Apply random force to make the crash feel more natural
                float forceMagnitude = Random.Range(500f, 800f);
                otherRB.AddForce(impactDirection * forceMagnitude, ForceMode.Impulse);

                // Apply torque to create spinning effect
                float torqueMagnitude = Random.Range(-300f, 300f);
                otherRB.AddTorque(transform.right * torqueMagnitude, ForceMode.Impulse);
            }
        }
        if (collision.gameObject.CompareTag("Roof"))
        {
            Debug.Log("Player Wins!");
            collision.gameObject.GetComponentInParent<Rigidbody>().isKinematic = true;
        }
    }
    #endregion

    #region Upside Down Handling
    private void CheckForUpsideDown()
    {
        if (Vector3.Dot(transform.up, Vector3.down) > 0.5f)  // Car is upside down
        {
            // Limit the angular velocity to prevent excessive flipping
            carRB.angularVelocity = Vector3.ClampMagnitude(carRB.angularVelocity, 5f);

            // Apply downward force to simulate gravity when upside down
            if (Mathf.Abs(currentCarLocalVelocity.z) < 10f)  // Slow speed
            {
                carRB.AddForce(Vector3.down * 50f, ForceMode.Acceleration);  // Stronger downward force
            }

            // Apply small corrective torque over time to slow down flipping
            Vector3 torqueDirection = transform.right;
            float correctiveTorque = Mathf.Lerp(0f, 300f, Mathf.Abs(currentCarLocalVelocity.z) / maxSpeed);
            carRB.AddTorque(torqueDirection * correctiveTorque, ForceMode.Acceleration);
        }
    }
    #endregion
}
