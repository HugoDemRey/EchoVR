using System.Collections;
using Prefabs.Rope;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Manages the player's checkpoints and spawn points within the game.
/// </summary>
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

    /// <summary>
    /// Updates the current spawn point for the player in the game.
    /// </summary>
    /// <param name="newSpawnPoint">The new transform to be set as the player's spawn point.</param>
    public void updateSpawnPoint(Transform newSpawnPoint)
    {
        spawnPoint = newSpawnPoint; // Update the spawn point to the new one
    }

    /// <summary>
    /// Returns the current spawn point designated for the player within the game.
    /// </summary>
    /// <returns>The transform representing the player's current spawn point.</returns>
    public Transform getSpawnPoint()
    {
        return spawnPoint;
    }

    /// <summary>
    /// Teleports the player to the last recorded spawn point, reactivating relevant objects like ziplines and adjusting player settings accordingly.
    /// </summary>
    /// <param name="player">The transform of the player to be teleported to the last spawn point.</param>
    public void teleportPlayerToLastSpawnPoint(Transform player)
    {
        if (spawnPoint != null)
        {
            climbingMomentumManager.Adjust(spawnPoint.eulerAngles.y); // Adjust the climbing momentum manager's angle
            StartCoroutine(SmoothTeleportation(player, spawnPoint)); // Start the coroutine to handle the teleportation

            // Find the ziplines if any, and reactivate them
            var ziplines = FindObjectsByType<ZipLineBehavior>(FindObjectsSortMode.None);

            if (ziplines != null) 
            {
                foreach (ZipLineBehavior zip in ziplines)
                {
                    zip.Reactivate();
                }
            }
        }
    }

    /// <summary>
    /// Handles the smooth teleportation of the player to a specified spawn point with fade-in and fade-out effects.
    /// </summary>
    /// <param name="player">The transform of the player to be teleported.</param>
    /// <param name="spawnPoint">The target transform representing the spawn point for the player.</param>
    /// <returns>Returns an IEnumerator for use in a coroutine.</returns>
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

    /// <summary>
    /// Teleports the player to the starting point of the game, adjusting related systems like climbing momentum.
    /// </summary>
    /// <param name="player">The transform of the player to be teleported to the game start point.</param>
    public void teleportPlayerToStart(Transform player)
    {
        if (gameStartPoint != null)
        {
            climbingMomentumManager.Adjust(gameStartPoint.eulerAngles.y); // Adjust the climbing momentum manager's angle
            StartCoroutine(SmoothTeleportation(player, gameStartPoint)); // Start the coroutine to handle the teleportation
        }
    }
}
