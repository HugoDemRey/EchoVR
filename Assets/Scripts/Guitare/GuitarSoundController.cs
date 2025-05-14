using UnityEngine;

public class GuitarSoundController : MonoBehaviour
{
    public AudioSource audioSource;

    private bool isHandOnStrings = false;

    void Start()
    {
        audioSource.loop = false; 
    }

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

    void Update()
    {
        if (isHandOnStrings && !audioSource.isPlaying)
        {
            audioSource.time = 0f;
            audioSource.Play();
        }
    }
}