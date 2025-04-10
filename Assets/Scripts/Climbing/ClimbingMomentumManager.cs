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

public class ClimbingMomentumManager : MonoBehaviour
{
    public CharacterController characterController; // To manipulate the position of the player
    public InputActionProperty rightControllerVelocityAction; // Link this to XRI Right/Velocity
    public InputActionProperty leftControllerVelocityAction; // Link this to XRI Left/Velocity
    public Transform mainCamera; // Reference to the camera offset transform
    public InputActionProperty headRotationAction; // Link this to XRI Head/Rotation
    private ClimbProvider climbProvider;
    private LocomotionState previousPhase;
    private float FORCE_MULTIPLIER = 3f; // Adjust this value to control the momentum

    private float MOMENTUM_DECAY_RATE = 0.98f; // Adjust this value to control how quickly the momentum decays
    private float MIN_VELOCITY_THRESHOLD = 1f; // Threshold to stop the coroutine

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


        // Check if we just transitioned to Done
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

        Debug.Log("Momentum Finished!");
    }

     void OnClimbFinished()
    {
        // Your logic here
        Vector3 rightVelocity = rightControllerVelocityAction.action.ReadValue<Vector3>();
        Vector3 leftVelocity = leftControllerVelocityAction.action.ReadValue<Vector3>();

        Vector3 greatestVelocity = rightVelocity.magnitude > leftVelocity.magnitude ? rightVelocity : leftVelocity;


        // Quaternion offsetRotation = Quaternion.Inverse(headRotationAction.action.ReadValue<Quaternion>()) * mainCamera.rotation;
        // greatestVelocity = offsetRotation * greatestVelocity; // Rotate the velocity vector to match the camera's rotation


        // Correcting the velocity to the camera's offset in the virtual world
        //greatestVelocity = mainCamera.TransformDirection(greatestVelocity); 

        Vector3 momentumVelocity = -greatestVelocity * FORCE_MULTIPLIER; // Adjust this value to control the momentum
        
        StartCoroutine(ApplyMomentum(momentumVelocity));


    }
}

