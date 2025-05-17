using UnityEngine;

public class FadeoutMusic : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeDuration = 2f;

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
