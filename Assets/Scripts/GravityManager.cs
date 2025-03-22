using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

public class ClimbEndGravityHandler : MonoBehaviour
{
    private ClimbProvider climbProvider;
    private LocomotionState previousPhase;

    void Start()
    {
        climbProvider = GetComponent<ClimbProvider>();
        if (climbProvider == null)
        {
            Debug.LogError("ClimbProvider not found on this GameObject!");
        }

        previousPhase = climbProvider.locomotionState;
    }

    void Update()
    {
        if (climbProvider == null)
            return;

        LocomotionState currentPhase = climbProvider.locomotionState;

        // Check if we just transitioned to Done
        if (previousPhase != LocomotionState.Ended && currentPhase == LocomotionState.Ended)
        {
            OnClimbFinished();
        }

        previousPhase = currentPhase;
    }

    void OnClimbFinished()
    {
        // Your logic here
        Debug.Log("Climb finished! Enabling gravity...");

    }
}
