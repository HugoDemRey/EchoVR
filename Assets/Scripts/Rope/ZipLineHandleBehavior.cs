using System;
using Prefabs.Rope;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

/// <summary>
/// Represents the behavior of a zip line handle, managing interactions with player hands
/// and ensuring alignment with the specified rope and pivot.
/// </summary>
public class ZipLineHandleBehavior : ClimbInteractable
{
    /// <summary>
    /// Represents the rope associated with the zip line handle.
    /// Used to determine the animation path.
    /// </summary>
    public RopeBehavior ropeBehavior;

    /// <summary>
    /// Stores the reference to the interactor associated with the left hand.
    /// Used to keep track the player's attachment to the zip line handle.
    /// </summary>
    private IXRSelectInteractor _leftHandInteractor;

    /// <summary>
    /// Stores the reference to the interactor associated with the right hand.
    /// Used to keep track the player's attachment to the zip line handle.
    /// </summary>
    private IXRSelectInteractor _rightHandInteractor;

    /// <summary>
    /// Used to manage the player's position during the animation
    /// </summary>
    private CharacterController _characterController;

    /// <summary>
    /// Triggers the animation associated with the zip line handle.
    /// Is then set to false.
    /// </summary>
    private bool _triggerAnimation;

    /// <summary>
    /// Indicates whether the zip line handle system is actively running.
    /// </summary>
    private bool _running;

    /// <summary>
    /// Indicates whether the player is currently attached to the zip line handle.
    /// </summary>
    private bool _attached;

    /// <summary>
    /// Stores the time at which the animation started.
    /// </summary>
    private float _animationStartTime;

    /// <summary>
    /// Defines the duration of the animation sequence for the zip line handle.
    /// Set when the animation is triggered.
    /// </summary>
    private float _animationDuration;

    /// <summary>
    /// Initializes the zip line handle behavior. Sets reference components.
    /// </summary>
    private void Start()
    {
        GameObject origin = GameObject.FindGameObjectWithTag("Origin");
        if (!origin)
        {
            Debug.LogWarning("No XR origin found. The XR Rig should be tagged as 'Origin'.");
        }
        
        _characterController = origin.GetComponent<CharacterController>();
        if (_characterController == null)
        {
            Debug.LogWarning("No CharacterController found.");
        }

    }

    /// <summary>
    /// Handles per-frame updates for the zip line handle behavior. Checks and updates the animation
    /// state, applies constraints when the animation completes, and triggers the animation if required.
    /// </summary>
    private void Update()
    {
        if (_running && UpdateAnimation())
        {
            // Animation completed
            _running = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            Destroy(this);
        }

        if (_triggerAnimation && !_running)
        {
            // Trigger the animation
            TriggerAnimation();
        }
    }

    /// <summary>
    /// Updates the animation state for the zipline handle movement. Calculates the new position
    /// of the handle and player, if attached.
    /// </summary>
    /// <returns>
    /// Returns true if the animation has completed and the handle should not be constrained anymore.
    /// </returns>
    private bool UpdateAnimation()
    {
        var currentTime = Time.time;
        var currentDurationPct = Mathf.Clamp01((currentTime - _animationStartTime) / _animationDuration);

        Vector3 newPosition = Vector3.Lerp(
            ropeBehavior.GetStartPoint(),
            ropeBehavior.GetEndPoint(),
            GetPositionPctFromTimePct(currentDurationPct)
        );

        if (_attached)
        {
            Vector3 relativePosition = newPosition - transform.position;
            _characterController.Move(relativePosition);
            HapticFeedback(.1f, .75f);
        }

        transform.position = newPosition;

        

        return currentTime - _animationStartTime >= _animationDuration;
    }

    /// <summary>
    /// Initiates the zipline handle animation by setting the parameters.
    /// </summary>
    private void TriggerAnimation()
    {
        _running = true;
        _triggerAnimation = false;
        _animationStartTime = Time.time;
        _animationDuration = Vector3.Distance(ropeBehavior.GetStartPoint(), ropeBehavior.GetEndPoint()) / 5f;
    }

    /// <summary>
    /// Calculates the positional percentage along the zip line path based on a given time percentage.
    /// Applies a non-linear mapping to convert the time percentage to position percentage.
    /// </summary>
    /// <param name="timePct">The time percentage, ranging from 0 to 1, representing the progress along the zip line timeline.</param>
    /// <returns>The position percentage along the zip line path, ranging from 0 to 1, after applying the mapping transformation.</returns>
    private float GetPositionPctFromTimePct(float timePct)
    {
        return (Mathf.Pow(timePct + 1, 2f) - 1) / 3f;
    }

    /// <summary>
    /// Updates the zip line handle's rope reference and aligns the handle's position
    /// and rotation with the specified pivot transform.
    /// </summary>
    /// <param name="newRopeBehavior">The new rope behavior to associate with the zip line handle.</param>
    /// <param name="newPivotTransform">The new pivot transform to align the handle's position and rotation with.</param>
    public void ForceUpdate(RopeBehavior newRopeBehavior, Transform newPivotTransform)
    {
        ropeBehavior = newRopeBehavior;

        transform.position = newPivotTransform.position;
        
        // Ensures correct handle rotation
        transform.rotation = Quaternion.Euler(
            0,
            transform.parent.rotation.eulerAngles.y,
            0
        );
    }

    /// <summary>
    /// Handles the event when an interactor selects the zip line handle. Tracks the interactor's handedness
    /// and assigns the appropriate hand interactor. Triggers the animation when both hands are interacting with the handle.
    /// </summary>
    /// <param name="args">The event arguments containing information about the interactor and interaction.</param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("OnSelectEntered in ZipLineHandleBehavior");
        
        if (args.interactorObject.handedness == InteractorHandedness.Left)
        {
            Debug.Log("Left hand grabbed the handle");
            _leftHandInteractor = args.interactorObject;
        }
        else if (args.interactorObject.handedness == InteractorHandedness.Right)
        {
            Debug.Log("Right hand grabbed the handle");
            _rightHandInteractor = args.interactorObject;
        }
        else
        {
            Debug.Log("Handle grabbed by hand: " + args.interactorObject.handedness);
        }
        
        base.OnSelectEntered(args);
        
        if (_leftHandInteractor is not null && _rightHandInteractor is not null)
        {
            Debug.Log("Zip line started");
            AttachPlayer();
        }
    }

    /// <summary>
    /// Handles the event when an interactor releases the zip line handle. Updates the interactor's handedness
    /// state, clears the associated hand interactor, and detaches the player if one or more hands release the handle.
    /// </summary>
    /// <param name="args">The event arguments containing information about the interactor and the end of interaction.</param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        switch (args.interactorObject.handedness)
        {
            case InteractorHandedness.Left:
                _leftHandInteractor = null;
                break;
            case InteractorHandedness.Right:
                _rightHandInteractor = null;
                break;
        }

        if (_leftHandInteractor is null || _rightHandInteractor is null)
        {
            DetachPlayer();
        }
    }

    /// <summary>
    /// Attaches the player to the zip line handle, triggering the animation.
    /// Logs a warning if the character controller is not found in the scene.
    /// </summary>
    private void AttachPlayer()
    {
        _attached = true;
        _triggerAnimation = true;
        _characterController.excludeLayers = Physics.AllLayers; // Disable collisions
        climbProvider.enabled = false; // Disable climbing (to allow forced player movement)
    }

    /// <summary>
    /// Detaches the player from the zip line handle.
    /// </summary>
    private void DetachPlayer()
    {
        _attached = false;
        climbProvider.enabled = true; // Re-enable climbing
        _characterController.excludeLayers = 0; // Reset collisions
    }

    /// <summary>
    /// Provides haptic feedback to the user's XR controllers through both hand devices.
    /// This method checks if haptic feedback is supported on each device, and if so, sends an impulse
    /// to deliver feedback with specified duration and amplitude.
    /// </summary>
    /// <param name="duration">The length of time the haptic feedback should be applied, in seconds.</param>
    /// <param name="amplitude">The strength of the haptic feedback, ranging from 0 (no feedback) to 1 (maximum intensity).</param>
    private void HapticFeedback(float duration, float amplitude)
    {
        InputDevice leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (leftHandDevice.TryGetHapticCapabilities(out var leftHapticCapabilities) && leftHapticCapabilities.supportsImpulse)
        {
            leftHandDevice.SendHapticImpulse(0, amplitude, duration);
        }
        if (rightHandDevice.TryGetHapticCapabilities(out var rightHapticCapabilities) && rightHapticCapabilities.supportsImpulse)
        {
            rightHandDevice.SendHapticImpulse(0, amplitude, duration);
        }
    }
}
