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
    [SerializeField] private float springTravel; // spring max compress or extend destance // 0.5
    [SerializeField] private float springStiffness; // spring max force that can be exert(when its fully compressed) // 30000
    [SerializeField] private float damperStiffness; // 3000
    [SerializeField] private float WheelRadius; // 0.33

    private int[] wheelsIsGrounded = new int[4];
    private bool isGorunded = false;


    private int moveInput;
    private bool isBraking;

    [Header("Car Settings")]
    [SerializeField] private float maxSpeed = 100f; // 100
    [SerializeField] private float acceleration = 25f; // 25
    [SerializeField] private float deceleration = 10f; // 10
    [SerializeField] private float brakingDeceleration = 100f; // 100

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
    void LockXPosition()
    {
        // Lock the X position to 0
        Vector3 currentPosition = transform.position;
        currentPosition.x = 0;
        transform.position = currentPosition;

        // Lock X velocity to 0 to prevent drifting
        Vector3 currentVelocity = carRB.velocity;
        currentVelocity.x = 0;
        carRB.velocity = currentVelocity;

        // Lock Y rotation to 0
        Quaternion currentRotation = transform.rotation;
        currentRotation.eulerAngles = new Vector3(currentRotation.eulerAngles.x, 0, currentRotation.eulerAngles.z);
        transform.rotation = currentRotation;

        // Optional: Lock Y angular velocity to 0 to prevent rotation
        Vector3 currentAngularVelocity = carRB.angularVelocity;
        currentAngularVelocity.y = 0;
        carRB.angularVelocity = currentAngularVelocity;
    }

    private void FixedUpdate()
    {
        LockXPosition();
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
        Visuals();
        EngineSound();
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
        if (isGorunded)
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
        carRB.AddForceAtPosition((isBraking ? brakingDeceleration : deceleration) * carVelocityRatio * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    #endregion
    #region Visuals
    private void Visuals()
    {
        Vfx();
    }

    private void Vfx()
    {
        if (isGorunded && Mathf.Abs(currentCarLocalVelocity.x) > minSideSkidVelocity && isBraking)
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
        if (tempGroundedWheels > 1)
        {
            isGorunded = true;
        }
        else
        {
            isGorunded = false;
        }
    }
    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRB.velocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }
    #endregion
    #region Input Handling

    #endregion
    #region Suspension Functions
    private void Suspension()
    {
        //physics ahhh logic
        for (int i = 0; i < rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;
            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxLength + WheelRadius, drivable))
            {
                wheelsIsGrounded[i] = 1;
                float currentSpringLength = hit.distance - WheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(carRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float dampForce = damperStiffness * springVelocity;


                float springForce = springStiffness * springCompression;

                float netForce = springForce - dampForce;

                carRB.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);

                //Visuals
                SetTirePosition(tires[i], hit.point + rayPoints[i].up * WheelRadius);

                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
            }
            else
            {
                wheelsIsGrounded[i] = 0;

                //Visuals
                SetTirePosition(tires[i], rayPoints[i].position - rayPoints[i].up * maxLength);

                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (WheelRadius + maxLength) * -rayPoints[i].up, Color.green);
            }
        }
    }
    #endregion
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Roof"))
        {

            Destroy(GameObject.FindGameObjectWithTag("Oponent"));
            Debug.Log("A");

        }
    }
}
