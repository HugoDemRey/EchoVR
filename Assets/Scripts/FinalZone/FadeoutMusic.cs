using UnityEngine;

/// <summary>
/// Handles the process of fading out the audio of a specified AudioSource over a specified duration, on collision.
/// </summary>
public class FadeoutMusic : MonoBehaviour
{
    /// <summary>
    /// AudioSource component that handles playing and controlling audio.
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// Specifies the duration, in seconds, for fading out the audio volume.
    /// </summary>
    public float fadeDuration = 2f;

    /// <summary>
    /// Holds a reference to the active fade-out Coroutine for the audio source.
    /// </summary>
    private Coroutine fadeCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Origin"))
        {
            if (fadeCoroutine == null)
            {
                fadeCoroutine = StartCoroutine(FadeOutCoroutine(fadeDuration));
            }
        }
    }

    /// <summary>
    /// Gradually fades out the volume of the audio source over the specified duration.
    /// </summary>
    /// <param name="duration">The amount of time, in seconds, over which the audio volume should fade out to zero.</param>
    /// <returns>An IEnumerator that facilitates the coroutine execution for the fade-out process.</returns>
    private System.Collections.IEnumerator FadeOutCoroutine(float duration)
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            float startVolume = audioSource.volume;
            float t = 0f;
            while (audioSource.volume > 0)
            {
                t += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
                yield return null;
            }
            audioSource.volume = 0;
            audioSource.Pause();
        }
        fadeCoroutine = null;
    }
}
