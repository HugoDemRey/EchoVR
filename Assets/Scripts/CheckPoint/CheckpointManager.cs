using System.Collections;
using UnityEditor;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Transform spawnPoint; // The spawn point for the player
    private Transform gameStartPoint;
    public ClimbingMomentumManager climbingMomentumManager; // Reference to the ClimbingMomentumManager script

    public GameObject canvas; // Reference to the CanvasGroup component

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

    public void teleportPlayerToLastSpawnPoint(Transform player)
    {
        if (spawnPoint != null)
        {
            climbingMomentumManager.Adjust(spawnPoint.eulerAngles.y); // Adjust the climbing momentum manager's angle
            StartCoroutine(SmoothTeleportation(player, spawnPoint)); // Start the coroutine to handle the teleportation
        }
    }

    public IEnumerator SmoothTeleportation(Transform player, Transform spawnPoint)
    {
        // Trigger fade out
        canvas.GetComponent<FadingScript>().FadeOut(0.25f);

        // Wait for 0.25 seconds
        yield return new WaitForSeconds(0.25f);

        // Teleport the player
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.25f); // Wait for 0.25 seconds

        // Trigger fade in
        canvas.GetComponent<FadingScript>().FadeIn(0.25f);
    }
    
    public void teleportPlayerToStart(Transform player)
    {
        if (gameStartPoint != null)
        {
            climbingMomentumManager.Adjust(gameStartPoint.eulerAngles.y); // Adjust the climbing momentum manager's angle
            StartCoroutine(SmoothTeleportation(player, gameStartPoint)); // Start the coroutine to handle the teleportation
        }
    }
}
