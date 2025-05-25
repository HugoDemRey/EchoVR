using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Manages tutorial checkpoints in the game.
/// Their behavior is different from regular checkpoints, as they teleport the player on collision.
/// </summary>
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
