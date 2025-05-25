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
    /// <summary>
    /// Used to manipulate the position of the player.
    /// </summary>
    public CharacterController characterController;

    /// <summary>
    /// Used to manipulate the rotation of the player
    /// </summary>
    public CustomSnapTurnProvider snapTurnProvider; 

    /// <summary>
    /// Used to manipulate the rotation of the player
    /// </summary>
    public CustomContinuousTurnProvider continuousTurnProvider;

    /// <summary>
    /// Link this to XRI Right/Velocity
    /// </summary>
    public InputActionProperty rightControllerVelocityAction;

    /// <summary>
    /// Link this to XRI Left/Velocity
    /// </summary>
    public InputActionProperty leftControllerVelocityAction;
    
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

    /// <summary>
    /// Adjusts the angle offset used for player rotation, accounting for rotations.
    /// </summary>
    /// <param name="newAngle">The new angle value used to set the rotation offset.</param>
    public void Adjust(float newAngle)
    {
        angleOffset = newAngle;
    }

    /// <summary>
    /// Handles logic for disabling the climbing momentum manager.
    /// </summary>
    void OnDisable()
    {
        if (snapTurnProvider != null)
            snapTurnProvider.onSnapTurn -= HandleTurn;

        if (continuousTurnProvider != null)
            continuousTurnProvider.onContinuousTurn -= HandleTurn;
    }

    /// <summary>
    /// Handles turn adjustments for the player by modifying the internal angle offset.
    /// </summary>
    /// <param name="angle">The angle value to adjust the current rotational offset.</param>
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

    /// <summary>
    /// Applies momentum to the character using the specified velocity and gradually reduces
    /// the velocity over time based on a decay rate until a minimum threshold is reached.
    /// </summary>
    /// <param name="velocity">The initial velocity vector to be applied as momentum.</param>
    /// <returns>Returns an enumerator to be used for coroutine execution.</returns>
    private System.Collections.IEnumerator ApplyMomentum(Vector3 velocity) {

        while (velocity.magnitude > MIN_VELOCITY_THRESHOLD)
        {
            characterController.Move(velocity * Time.deltaTime);
            velocity *= MOMENTUM_DECAY_RATE; // Reduce the velocity over time
            yield return null; // Wait for the next frame
        }

    }

    /// <summary>
    /// Handles the logic to execute when the climbing action is finished.
    /// </summary>
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
        
        Vector3 rightVelocity = rightControllerVelocityAction.action.ReadValue<Vector3>();
        Vector3 leftVelocity = leftControllerVelocityAction.action.ReadValue<Vector3>();

        Vector3 greatestVelocity = rightVelocity.magnitude > leftVelocity.magnitude ? rightVelocity : leftVelocity;
        // Apply the angle offset to the velocity
        Quaternion rotation = Quaternion.Euler(0, angleOffset, 0);
        greatestVelocity = rotation * greatestVelocity; // Rotate the velocity vector

        Vector3 momentumVelocity = -greatestVelocity * FORCE_MULTIPLIER; // Adjust this value to control the momentum
        
        StartCoroutine(ApplyMomentum(momentumVelocity));


    }
    
        /// <summary>
        /// Stops the momentum effect after a specified delay by halting the associated coroutine.
        /// </summary>
        /// <param name="momentumCoroutine">The coroutine controlling the momentum effect that needs to be stopped.</param>
        /// <param name="delayInSeconds">The time in seconds to wait before stopping the momentum effect.</param>
        /// <returns>An enumerator used for coroutine execution.</returns>
        private System.Collections.IEnumerator StopMomentumAfterDelay(Coroutine momentumCoroutine, float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds); // Wait for 200ms
        StopCoroutine(momentumCoroutine); // Stop the ApplyMomentum coroutine
    }
}

