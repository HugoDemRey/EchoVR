using Unity.VisualScripting;
using UnityEngine;

public class MarkCheckpoint : MonoBehaviour
{
    public CheckpointState checkpointState;
    public CheckpointManager checkpointManager; // Reference to the CheckpointManager
    public Transform spawnPoint; // Reference to the spawn point
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Origin"))
        {
            checkpointState.activateCheckpoint();
            checkpointManager.updateSpawnPoint(spawnPoint); // Update the spawn point in the CheckpointManager
        }
    }
}
