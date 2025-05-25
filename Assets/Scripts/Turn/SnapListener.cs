using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

/// <summary>
/// SnapListener provides functionality related to handling and reacting to snap events within the Unity environment.
/// </summary>
public class SnapListener : MonoBehaviour
{

    [SerializeField] CustomSnapTurnProvider snapTurnProvider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if (snapTurnProvider != null)
            snapTurnProvider.onSnapTurn += HandleSnapTurn;
    }

    void OnDisable()
    {
        if (snapTurnProvider != null)
            snapTurnProvider.onSnapTurn -= HandleSnapTurn;
    }

    /// <summary>
    /// Handles the event triggered by a snap turn. Logs the angle of the snap turn to the console.
    /// </summary>
    /// <param name="angle">The angle in degrees by which the snap turn has been executed.</param>
    private void HandleSnapTurn(float angle)
    {
        Debug.Log($"Snap Turned {angle} degrees");
    }
}
