using UnityEngine;
using System.Collections;

public class FallingCheckpoint : MonoBehaviour
{
    public Transform SpawnPoint;
    public GameObject canvas; // Reference to the CanvasGroup component

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleCheckpoint(other));
        }
    }

    private IEnumerator HandleCheckpoint(Collider player)
    {
        // Trigger fade out
        canvas.GetComponent<FadingScript>().FadeOut(0.25f);

        // Wait for 0.25 seconds
        yield return new WaitForSeconds(0.25f);

        // Teleport the player
        player.transform.position = SpawnPoint.position;
        player.transform.rotation = SpawnPoint.rotation;

        yield return new WaitForSeconds(0.25f); // Wait for 0.25 seconds

        // Trigger fade in
        canvas.GetComponent<FadingScript>().FadeIn(0.25f);
    }
}
