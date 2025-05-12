using UnityEditor;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Transform spawnPoint; // The spawn point for the player
    public void updateSpawnPoint(Transform newSpawnPoint) {
        spawnPoint = newSpawnPoint; // Update the spawn point to the new one
    }

    public Transform getSpawnPoint() {
        return spawnPoint; 
    }
}
