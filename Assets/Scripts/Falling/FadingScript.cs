using System.Collections;
using UnityEngine;

/// <summary>
/// Class responsible for handling fade-in and fade-out effects on a CanvasGroup.
/// </summary>
public class FadingScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup; // Reference to the CanvasGroup component

    /// <summary>
    /// Initiates a fade-in effect on a CanvasGroup over a specified duration.
    /// </summary>
    /// <param name="fadeDuration">The duration of the fade-in effect in seconds. Defaults to 1.0f if not specified.</param>
    public void FadeIn(float fadeDuration = 1.0f)
    
    {
        // Start the fade-in coroutine
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0f, fadeDuration));
    }

    /// <summary>
    /// Initiates a fade-out effect on a CanvasGroup over a specified duration.
    /// </summary>
    /// <param name="fadeDuration">The duration of the fade-out effect in seconds. Defaults to 1.0f if not specified.</param>
    public void FadeOut(float fadeDuration = 1.0f)
    {
        // Start the fade-out coroutine
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1f, fadeDuration));
    }

    /// <summary>
    /// Smoothly transitions the alpha value of a CanvasGroup from a start value to an end value over a specified duration.
    /// This coroutine updates the transparency of the CanvasGroup incrementally each frame.
    /// </summary>
    /// <param name="canvasGroup">The CanvasGroup component to be faded.</param>
    /// <param name="start">The starting alpha value of the CanvasGroup.</param>
    /// <param name="end">The target alpha value of the CanvasGroup.</param>
    /// <param name="duration">The duration of the fade effect in seconds.</param>
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
