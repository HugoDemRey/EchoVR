using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class ClimbingMomentumManager : MonoBehaviour
{
    public CharacterController characterController; // To manipulate the position of the player
    public CustomSnapTurnProvider snapTurnProvider; // To manipulate the rotation of the player

    public CustomContinuousTurnProvider continuousTurnProvider; // To manipulate the rotation of the player
    public InputActionProperty rightControllerVelocityAction; // Link this to XRI Right/Velocity
    public InputActionProperty leftControllerVelocityAction; // Link this to XRI Left/Velocity
    
    private float angleOffset = 0f; // Offset to apply to the rotation
    private ClimbProvider climbProvider;
    private LocomotionState previousPhase;
    private ClimbInteractable lastClimbAnchorInteractable;
    private float FORCE_MULTIPLIER = 3f; // Adjust this value to control the momentum

    private float MOMENTUM_DECAY_RATE = 0.98f; // Adjust this value to control how quickly the momentum decays
    private float MIN_VELOCITY_THRESHOLD = 2f; // Threshold to stop the coroutine

    void OnEnable()
    {
        if (snapTurnProvider != null)
            snapTurnProvider.onSnapTurn += HandleTurn;

        if (continuousTurnProvider != null)
            continuousTurnProvider.onContinuousTurn += HandleTurn;

        Transform mainCamera = transform.parent.parent.Find("Camera Offset").Find("Main Camera");
        angleOffset += mainCamera.eulerAngles.y; // Get the initial angle of the camera  
        
    }

    public void Adjust(float newAngle)
    {
        angleOffset = newAngle;
    }

    void OnDisable()
    {
        if (snapTurnProvider != null)
            snapTurnProvider.onSnapTurn -= HandleTurn;

        if (continuousTurnProvider != null)
            continuousTurnProvider.onContinuousTurn -= HandleTurn;
    }

    private void HandleTurn(float angle)
    {
        angleOffset = (angleOffset + angle) % 360;
    }

    void Start()
    {
        climbProvider = GetComponent<ClimbProvider>();

        if (climbProvider == null)
        {
            Debug.LogError("ClimbProvider not found on this GameObject!");
        }

        previousPhase = climbProvider.locomotionState;
    }

    void Update() {

        if (climbProvider == null) return;

        LocomotionState currentPhase = climbProvider.locomotionState;

        if (climbProvider.climbAnchorInteractable != null && lastClimbAnchorInteractable != climbProvider.climbAnchorInteractable) {
            lastClimbAnchorInteractable = climbProvider.climbAnchorInteractable;
        }
        if (previousPhase != LocomotionState.Ended && currentPhase == LocomotionState.Ended){
            OnClimbFinished();
        }

        previousPhase = currentPhase;

    }

    private System.Collections.IEnumerator ApplyMomentum(Vector3 velocity) {

        while (velocity.magnitude > MIN_VELOCITY_THRESHOLD)
        {
            characterController.Move(velocity * Time.deltaTime);
            velocity *= MOMENTUM_DECAY_RATE; // Reduce the velocity over time
            yield return null; // Wait for the next frame
        }

    }

    void OnClimbFinished()
    {

        OverrideMomentumForce momentumForce;
        if (lastClimbAnchorInteractable != null && (momentumForce = lastClimbAnchorInteractable.GetComponent<OverrideMomentumForce>()) != null)
        {
            Coroutine momentumCoroutine =  StartCoroutine(ApplyMomentum(momentumForce.forceDirection * momentumForce.forceMagnitude));
            float momentumDuration = momentumForce.seconds;
            if (momentumDuration > 0) StartCoroutine(StopMomentumAfterDelay(momentumCoroutine, momentumDuration)); // Stop after the specified duration
            return;
        }


        // Your logic here
        Vector3 rightVelocity = rightControllerVelocityAction.action.ReadValue<Vector3>();
        Vector3 leftVelocity = leftControllerVelocityAction.action.ReadValue<Vector3>();

        Vector3 greatestVelocity = rightVelocity.magnitude > leftVelocity.magnitude ? rightVelocity : leftVelocity;
        // Apply the angle offset to the velocity
        Quaternion rotation = Quaternion.Euler(0, angleOffset, 0);
        greatestVelocity = rotation * greatestVelocity; // Rotate the velocity vector

        Vector3 momentumVelocity = -greatestVelocity * FORCE_MULTIPLIER; // Adjust this value to control the momentum
        
        StartCoroutine(ApplyMomentum(momentumVelocity));


    }

        // Add this helper coroutine
    private System.Collections.IEnumerator StopMomentumAfterDelay(Coroutine momentumCoroutine, float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds); // Wait for 200ms
        StopCoroutine(momentumCoroutine); // Stop the ApplyMomentum coroutine
    }
}

