using UnityEditor;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Transform spawnPoint; // The spawn point for the player
    private Transform gameStartPoint;

    private void Start()
    {
        gameStartPoint = GameObject.Find("GameStart").transform; // Find the GameStartPoint in the scene
    }
    public void updateSpawnPoint(Transform newSpawnPoint)
    {
        spawnPoint = newSpawnPoint; // Update the spawn point to the new one
    }

    public Transform getSpawnPoint()
    {
        return spawnPoint;
    }

    public void teleportPlayer(Transform player)
    {
        if (spawnPoint != null)
        {
            player.position = spawnPoint.position; // Teleport the player to the spawn point
            player.rotation = spawnPoint.rotation; // Set the player's rotation to the spawn point's rotation
        }
    }
    
    public void teleportPlayerToStart(Transform player)
    {
        if (gameStartPoint != null)
        {
            player.position = gameStartPoint.position; // Teleport the player to the GameStart point
            player.rotation = gameStartPoint.rotation; // Set the player's rotation to the GameStart point's rotation
        }
    }
}
