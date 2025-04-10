using UnityEngine;

public class SnapTurnLogger : MonoBehaviour
{
    [SerializeField] SnapTurnListener snapTurnListener;

    void OnEnable()
    {
        if (snapTurnListener != null)
            snapTurnListener.onSnapTurn += HandleSnapTurn;
    }

    void OnDisable()
    {
        if (snapTurnListener != null)
            snapTurnListener.onSnapTurn -= HandleSnapTurn;
    }

    void HandleSnapTurn(float degrees)
    {
        Debug.Log($"Snap turn detected: {degrees}°");
    }
}
