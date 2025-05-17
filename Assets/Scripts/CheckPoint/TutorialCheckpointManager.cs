using System.Collections;
using UnityEditor;
using UnityEngine;

public class TutorialCheckpointManager : MonoBehaviour
{
    public GameObject canvas;
    public ClimbingMomentumManager climbingMomentumManager;
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
