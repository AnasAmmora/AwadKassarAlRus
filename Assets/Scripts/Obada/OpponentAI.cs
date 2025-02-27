using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOpponentAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NewCarController carController; // Reference to the NewCarController
    [SerializeField] private Transform player; // Reference to the player's position

    private void Start()
    {
        if (carController == null)
            carController = GetComponent<NewCarController>(); // Get the car controller component
    }

    private void FixedUpdate()
    {
        if (player == null) return; // Ensure the player is set

        // Calculate the direction of the player relative to the AI car
        Vector3 directionToPlayer = player.position - transform.position;

        // Dot product to check if the player is in front or behind
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer.normalized);

        // If the player is in front (dotProduct > 0), set move input to 1 (move forward)
        if (dotProduct > 0)
        {
            carController.SetMoveInput(1); // Move forward
        }
        // If the player is behind (dotProduct < 0), set move input to -1 (reverse)
        else if (dotProduct < 0)
        {
            carController.SetMoveInput(-1); // Move backward
        }
    }
}
