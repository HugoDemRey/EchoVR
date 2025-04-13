using UnityEngine;

public class Tamere : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag the transform of your right-hand controller (or hand) here.")]
    [SerializeField] private Transform rightHandTransform;

    [Tooltip("Audio source that will play the strum sound.")]
    [SerializeField] private AudioSource guitarAudioSource;

    [Tooltip("Sound clip to play when strumming.")]
    [SerializeField] private AudioClip strumClip;

    [Header("Strum Distance Settings")]
    [Tooltip("Maximum distance at which we consider the guitar to be 'strummed'.")]
    [SerializeField] private float strumDistance = 0.1f;

    private bool wasInRange;

    private void Start()
    {
        guitarAudioSource.clip = strumClip;
        wasInRange = false;
    }

    private void Update()
    {
        if (rightHandTransform == null) return;

        float distance = Vector3.Distance(rightHandTransform.position, transform.position);
        bool isInRange = distance <= strumDistance;

        if (isInRange && !wasInRange)
        {
            if(!guitarAudioSource.isPlaying) {
                guitarAudioSource.Play();
            }

        }
        else if (!isInRange && wasInRange)
        {
            guitarAudioSource.Stop();
        }

        wasInRange = isInRange;
    }
}