using UnityEngine;

/// <summary>
/// Controls the sound behavior for a guitar simulation.
/// This class interacts with an AudioSource to play or pause guitar sound
/// when a hand interacts with the guitar strings.
/// </summary>
public class GuitarSoundController : MonoBehaviour
{
    public AudioSource audioSource;

    private bool isHandOnStrings = false;

    void Start()
    {
        audioSource.loop = false; 
    }

    /// <summary>
    /// Handles the interaction with the guitar strings when the hand touches or leaves them.
    /// Plays the guitar sound when the hand touches the strings and pauses it when the hand moves away.
    /// </summary>
    /// <param name="touching">A boolean value indicating whether the hand is touching the strings (true) or not (false).</param>
    public void OnHandTouch(bool touching)
    {
        isHandOnStrings = touching;

        if (isHandOnStrings)
        {
            Debug.Log("nfo entre");

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            Debug.Log("info sortie");

            if (audioSource.isPlaying)
                audioSource.Pause();
        }
    }

    /// <summary>
    /// Updates the state of the guitar simulation, ensuring that the guitar sound
    /// plays continuously while the hand is touching the strings.
    /// Resets and restarts the audio playback if needed.
    /// </summary>
    void Update()
    {
        if (isHandOnStrings && !audioSource.isPlaying)
        {
            audioSource.time = 0f;
            audioSource.Play();
        }
    }
}