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
    [SerializeField] private GameObject[] frontTireParent = new GameObject[2];
    [SerializeField] private TrailRenderer[] skidMarks = new TrailRenderer[2];
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

    //[Header("Inputs")]
    //private float moveInput;
    //private float steerInput;
    private int moveInput;
    //private int steerInput;
    private bool isBraking;

    [Header("Car Settings")]
    [SerializeField] private float maxSpeed = 100f; // 100
    [SerializeField] private float acceleration = 25f; // 25
    [SerializeField] private float deceleration = 10f; // 10
    //[SerializeField] private float steerStrength = 15f; // 15
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragCoefficient = 1f; // 1
    [SerializeField] private float brakingDeceleration = 100f; // 100
    [SerializeField] private float brakingDragCoefficient = 0.5f; // 0.5

    private Vector3 currentCarLocalVelocity = Vector3.zero;
    private float carVelocityRatio = 0f;

    [Header("Visuals")]
    [SerializeField] private float tireRotationSpeed = 3000f; // 3000
    //[SerializeField] private float maxSteeringAngle = 30f; // 30
    [SerializeField] private float minSideSkidVelocity = 10f; // 10

    [Header("Audio")]
    [SerializeField]
    [Range(0, 1)] private float minPitch = 1f;
    [SerializeField]
    [Range(1, 5)] private float maxPitch = 5f;

    //[Header("Gyroscope")]
    //private Gyroscope gyro; // Added variable for gyroscope

    public void SetMoveInput(int input)
    {
        moveInput = input;
    }

    //public void SetSteerInput(int input)
    //{
    //    steerInput = input;
    //}
    #region Main functions
    void Start()
    {
        carRB = GetComponent<Rigidbody>();
        moveInput = 0;
        //steerInput = 0;
        isBraking = false;

        //if (SystemInfo.supportsGyroscope)
        //{
        //    gyro = Input.gyro;
        //    gyro.enabled = true;
        //}

    }

    void Update()
    {
        //GetPlayerInput();
        //// Use gyroscope to control steerInput
        //if (gyro != null && gyro.enabled)
        //{
        //    steerInput = Mathf.Clamp(gyro.rotationRateUnbiased.y, -1f, 1f);
        //}
    }



    private void FixedUpdate()
    {
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

    //public void OnLeftButtonPressed()
    //{
    //    steerInput = -1;
    //}

    //public void OnRightButtonPressed()
    //{
    //    steerInput = 1;
    //}

    public void OnMoveButtonReleased()
    {
        moveInput = 0;
    }

    //public void OnSteerButtonReleased()
    //{
    //    steerInput = 0;
    //}
    //public void OnBrakeButtonPressed()
    //{
    //    isBraking = true;
    //}
    //public void OnBrakeButtonReleased()
    //{
    //    isBraking = false;
    //}

    #endregion
    #region Movement
    private void Movement()
    {
        if (isGorunded)
        {
            Acceleration();
            Deceleration();
            //Turn();
            //SidewayDrag();
        }
        //Turn();
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
    //private void Turn()
    //{
    //    carRB.AddRelativeTorque(steerStrength * steerInput * turningCurve.Evaluate(Mathf.Abs(carVelocityRatio)) * Mathf.Sign(carVelocityRatio) * carRB.transform.up, ForceMode.Acceleration);
    //}
    private void SidewayDrag()
    {
        float currentSideWaySpeed = currentCarLocalVelocity.x;
        float dragMagnitude = -currentSideWaySpeed * ((isBraking ? brakingDragCoefficient : dragCoefficient));
        //float dragMagnitude = -currentSideWaySpeed *  brakingDragCoefficient;
        Vector3 dragForce = transform.right * dragMagnitude;
        carRB.AddForceAtPosition(dragForce, carRB.worldCenterOfMass, ForceMode.Acceleration);
    }
    #endregion
    #region Visuals
    private void Visuals()
    {
        //TireVisuals();
        Vfx();
    }
    //private void TireVisuals()
    //{
    //    float steeringAngle = maxSteeringAngle * steerInput;
    //    for (int i = 0; i < tires.Length; i++)
    //    {
    //        if (i < 2)
    //        {
    //            frontTireParent[i].transform.localEulerAngles = new Vector3(frontTireParent[i].transform.localEulerAngles.x,
    //                steeringAngle, frontTireParent[i].transform.localEulerAngles.z);
    //            //tires[i].transform.Rotate(Vector3.right, tireRotationSpeed * carVelocityRatio * Time.deltaTime, Space.Self);

    //        }
    //        tires[i].transform.Rotate(Vector3.right, tireRotationSpeed * carVelocityRatio * Time.deltaTime, Space.Self);
    //        //else
    //        //{
    //        //    tires[i].transform.Rotate(Vector3.right, tireRotationSpeed * moveInput * Time.deltaTime, Space.Self);
    //        //}
    //    }
    //}
    private void Vfx()
    {
        //if (isGorunded&&Mathf.Abs(currentCarLocalVelocity.x)>minSideSkidVelocity)
        if (isGorunded && Mathf.Abs(currentCarLocalVelocity.x) > minSideSkidVelocity && isBraking)
        {
            //ToggleSkidMarks(true);
            ToggleSkidSmokes(true);
            ToggleSlidSound(true);
        }
        else
        {
            //ToggleSkidMarks(false);
            ToggleSkidSmokes(false);
            ToggleSlidSound(false);

        }
    }
    //private void ToggleSkidMarks(bool toggle)
    //{
    //    foreach (var skidMark in skidMarks)
    //    {
    //        skidMark.emitting = toggle;
    //    }
    //}
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

    //private void GetPlayerInput()
    //{
    //moveInput = Input.GetAxis("Vertical");
    //steerInput = Input.GetAxis("Horizontal");

    //steerInput = SimpleInput.GetAxis("Horizontal");
    //}
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
}
