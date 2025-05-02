using UnityEngine;

public class CheckpointState : MonoBehaviour
{   
    public MeshRenderer tissueRenderer;

    public void activateCheckpoint()
    {
        tissueRenderer.material.color = Color.green; // Change the color of the checkpoint to green
    }
}
