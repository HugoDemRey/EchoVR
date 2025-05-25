using UnityEngine;
using System.Collections;

/// <summary>
/// Checkpoint mechanism in the game that interacts with the
/// CheckpointManager to manage player respawn and teleportation functionality.
/// </summary>
public class FallingCheckpoint : MonoBehaviour
{
    /// <summary>
    /// A reference to the CheckpointManager instance responsible for managing spawn points
    /// and player teleportation in the game.
    /// </summary>
    public CheckpointManager checkpointManager;

    /// <summary>
    /// Handles the collision event when another collider enters the trigger attached to the game object.
    /// Checks if the colliding object is tagged as "Origin" and teleports the player to the last spawn point if applicable.
    /// </summary>
    /// <param name="other">The collider that triggered the event.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Origin"))
        {
            Transform spawnPoint = checkpointManager.getSpawnPoint(); // Get the spawn point from the CheckpointManager
            if (spawnPoint != null)
            {
                checkpointManager.teleportPlayerToLastSpawnPoint(other.transform); // Teleport the player to the last spawn point
            }
        }
    }
}
