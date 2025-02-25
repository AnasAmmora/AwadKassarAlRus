using UnityEngine;
using UnityEngine.AI;

public class OpponentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform accelerationPoint;

    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float stoppingDistance = 5f;

    private int moveInput;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        carRB = GetComponent<Rigidbody>();
        agent.speed = maxSpeed;
        agent.acceleration = acceleration;
        agent.stoppingDistance = stoppingDistance;
    }

    void Update()
    {
        if (player == null) return;

        // Calculate relative position
        Vector3 toPlayer = player.position - transform.position;
        float dotProduct = Vector3.Dot(transform.forward, toPlayer.normalized);

        // Decide movement direction
        if (dotProduct > 0) // Player is in front
        {
            moveInput = 1;
        }
        else // Player is behind
        {
            moveInput = -1;
        }

        // Move the car
        MoveCar();
    }

    private void MoveCar()
    {
        if (carRB == null) return;

        if (moveInput != 0)
        {
            carRB.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
        }
        else
        {
            carRB.AddForceAtPosition(deceleration * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
        }
    }

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
            Debug.Log("Oponent Wins!");
            // Stop movement completely
            GetComponent<Rigidbody>().isKinematic = true;
            //collision.gameObject.GetComponentInParent<Rigidbody>().isKinematic = true;
        }
    }



    private void StopOpponent()
    {
        // Stop movement & rotation completely
        carRB.velocity = Vector3.zero;
        carRB.angularVelocity = Vector3.zero;
        maxSpeed = 0f;
        agent.speed = 0;
        acceleration = 0f;
        deceleration = 0f;

        // Optional: Play a hit effect or sound
        Debug.Log("Opponent hit the roof! Stopping completely.");
    }



}
