using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

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

    private void HandleSnapTurn(float angle)
    {
        Debug.Log($"Snap Turned {angle} degrees");
    }
}
