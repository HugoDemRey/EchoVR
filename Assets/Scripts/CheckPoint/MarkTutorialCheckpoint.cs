using Unity.VisualScripting;
using UnityEngine;

public class MarkTutorialCheckpoint : MonoBehaviour
{
    public CheckpointState checkpointState;
    public TutorialCheckpointManager checkpointManager; // Reference to the CheckpointManager
    public Transform spawnPoint; // Reference to the spawn point
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Origin"))
        {
            checkpointState.activateCheckpoint();
            checkpointManager.teleportPlayer(other.transform, spawnPoint); // Teleport the player to the spawn point
        }
    }
}
