using UnityEngine;
using System.Collections;

public class FallingCheckpoint : MonoBehaviour
{
    public CheckpointManager checkpointManager;
    public GameObject canvas; // Reference to the CanvasGroup component

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Origin"))
        {
            Transform spawnPoint = checkpointManager.getSpawnPoint(); // Get the spawn point from the CheckpointManager
            if (spawnPoint != null)
            {
                StartCoroutine(HandleCheckpoint(other, spawnPoint));    
            }
        }
    }

    private IEnumerator HandleCheckpoint(Collider player, Transform spawnPoint)
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
}
