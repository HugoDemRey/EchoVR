using UnityEngine;

[ExecuteInEditMode]
public class ShowInEditMode : MonoBehaviour
{
    private void Update()
    {
        // Hide object in play mode
        gameObject.SetActive(!Application.isPlaying);
    }
}
