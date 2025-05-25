using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Manages teleportation of players to predefined tutorial spawn points, ensuring smooth transitions with additional effects.
/// </summary>
public class TutorialCheckpointManager : MonoBehaviour
{
    public GameObject canvas;
    public ClimbingMomentumManager climbingMomentumManager;

    /// <summary>
    /// Teleports the player to a specified spawn point and initiates smooth teleportation effects such as fade-in and fade-out.
    /// </summary>
    /// <param name="player">The transform of the player to be teleported.</param>
    /// <param name="spawnPoint">The target transform representing the spawn point for the player.</param>
    public void teleportPlayer(Transform player, Transform spawnPoint)
    {
        if (spawnPoint != null)
        {
            Debug.Log("Teleporting " + player.name + " to " + spawnPoint.name);
            // Start the coroutine to handle the teleportation
            climbingMomentumManager.Adjust(spawnPoint.eulerAngles.y);
            StartCoroutine(SmoothTeleporation(player, spawnPoint));
        }
        else
        {
            Debug.LogError("Spawn point is null. Cannot teleport player.");
        }
    }
    
    /// <summary>
    /// Handles the smooth teleportation of the player to a specified spawn point with fade-in and fade-out effects.
    /// </summary>
    /// <param name="player">The transform of the player to be teleported.</param>
    /// <param name="spawnPoint">The target transform representing the spawn point for the player.</param>
    /// <returns>Returns an IEnumerator for use in a coroutine.</returns>
    private IEnumerator SmoothTeleporation(Transform player, Transform spawnPoint)
    {
        // Wait a few seconds before teleporting
        yield return new WaitForSeconds(1f);

        // Trigger fade out
        canvas.GetComponent<FadingScript>().FadeOut(1f);

        // Wait for 0.25 seconds
        yield return new WaitForSeconds(1f);

        // Teleport the player
        player.position = spawnPoint.position;
        player.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(1f); // Wait for 0.25 seconds

        // Trigger fade in
        canvas.GetComponent<FadingScript>().FadeIn(1f);
    }
}
