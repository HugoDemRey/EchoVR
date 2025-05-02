using UnityEngine;

public class CheckpointState : MonoBehaviour
{   
    public MeshRenderer tissueRenderer;
    public ParticleSystem particleSystem; // Reference to the ParticleSystem component
    public AudioSource audioSource; // Reference to the AudioSource component
    
    private bool isActivated = false; // Flag to check if the checkpoint is activated
    public void activateCheckpoint()
    {
        if (isActivated) return; // If already activated, do nothing
        isActivated = true; // Set the checkpoint as activated
        tissueRenderer.material.color = Color.green; // Change the color of the checkpoint to green
        particleSystem.Play(); // Play the particle system
        audioSource.Play(); // Play the audio source
    }
}
