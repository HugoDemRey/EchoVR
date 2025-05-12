using System.Collections;
using UnityEngine;

public class FadingScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup; // Reference to the CanvasGroup component

    public void FadeIn(float fadeDuration = 1.0f)
    
    {
        // Start the fade-in coroutine
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0f, fadeDuration));
    }

    public void FadeOut(float fadeDuration = 1.0f)
    {
        // Start the fade-out coroutine
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1f, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float duration) {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null; // Wait for the next frame
        }
        canvasGroup.alpha = end; // Ensure the final value is set

    }
}
