using UnityEngine;

/// <summary>
/// A class that enables specific functionality or behavior in the Unity Editor during edit mode ONLY.
/// </summary>
[ExecuteInEditMode]
public class ShowInEditMode : MonoBehaviour
{
    private void Update()
    {
        // Hide object in play mode
        gameObject.SetActive(!Application.isPlaying);
    }
}
