using UnityEngine;
using System.Collections;

public class FallingCheckpoint : MonoBehaviour
{
    public CheckpointManager checkpointManager;

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
