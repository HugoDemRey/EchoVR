using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class SnapTurnListener : SnapTurnProvider
{
    /// <summary>
    /// Called whenever a snap turn is triggered.
    /// </summary>
    public event System.Action<float> onSnapTurn;

    /// <inheritdoc/>
    protected override void StartTurn(float amount)
    {
        base.StartTurn(amount);

        // Fire the event right after the original logic.
        if (Mathf.Abs(amount) > 0f)
        {
            onSnapTurn?.Invoke(amount);
            Debug.Log($"[SnapTurnListener] Snap turn triggered: {amount}°");
        }
    }
}
