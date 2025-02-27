using UnityEngine;
using UnityEngine.AI;

public class OpponentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform accelerationPoint;

    [Header("Car Settings")]
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private float accelerationForce = 2000f;
    [SerializeField] private float brakingForce = 3000f;
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private float stoppingDistance = 5f;

    private int moveInput;
    private bool isGrounded;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        carRB = GetComponent<Rigidbody>();

        // Allow NavMeshAgent to calculate paths, but not move the car directly
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.speed = maxSpeed;
        agent.stoppingDistance = stoppingDistance;

        // Add some drag to avoid infinite sliding
        carRB.drag = 0.1f;
        carRB.angularDrag = 0.5f;
    }

    void Update()
    {
        if (player == null) return;

        // Set target for NavMeshAgent
        agent.SetDestination(player.position);

        // Calculate movement direction
        Vector3 toTarget = agent.steeringTarget - transform.position;
        float dotProduct = Vector3.Dot(transform.forward, toTarget.normalized);

        // Move forward if target is ahead, backward if behind
        moveInput = dotProduct > 0 ? 1 : -1;
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        MoveCar();
    }

    private void MoveCar()
    {
        if (!IsGrounded()) return;

        float currentSpeed = carRB.velocity.magnitude;

        if (moveInput != 0 && currentSpeed < maxSpeed)
        {
            // Accelerate forward or backward
            carRB.AddForceAtPosition(moveInput * accelerationForce * transform.forward, accelerationPoint.position, ForceMode.Force);
        }
        else
        {
            // Apply braking force to gradually stop
            carRB.AddForce(-carRB.velocity.normalized * brakingForce, ForceMode.Force);
        }
    }

    private void ApplyGravity()
    {
        if (!IsGrounded())
        {
            carRB.AddForce(Vector3.down * gravityMultiplier * Physics.gravity.y, ForceMode.Acceleration);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -transform.up, 1.2f);
    }
}
