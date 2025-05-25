using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

/// <summary>
/// Handles the snapping of hand controllers to predefined positions and rotations
/// during climbing interactions.
/// </summary>
public class ClimbHandSnapping : MonoBehaviour
{
    /// <summary>
    /// Transform of the right controller.
    /// </summary>
    public Transform rightController;

    /// <summary>
    /// Transform of the left controller.
    /// </summary>
    public Transform leftController;

    /// <summary>
    /// Model of the right controller to be visually adjusted during climbing interactions.
    /// </summary>
    private Component rightControllerModel;

    /// <summary>
    /// Model of the left controller to be visually adjusted during climbing interactions.
    /// </summary>
    private Component leftControllerModel;

    /// <summary>
    /// Used to track the model's position relative to its original position.
    /// </summary>
    private ControllerTracking rightControllerTracking;

    /// <summary>
    /// Used to track the model's position relative to its original position.
    /// </summary>
    private ControllerTracking leftControllerTracking;

    /// <summary>
    /// Speed (in frames) at which the controller's position and rotation snap towards the target hand pose during alignment.
    /// </summary>
    public float snapSpeed = 8f;

    /// <summary>
    /// The object to snap to.
    /// </summary>
    private ClimbInteractable climbInteractable;
    
    /// <summary>
    /// List of predefined hand poses, every children with tag "SnappingPoint" is added to this list
    /// </summary>
    private List<Transform> SnappingPoints;

    private string SNAPPING_POINT_TAG = "SnappingPoint";
    private static string CONTROLLER_MODEL_TAG = "ControllerModel";

    void Start()
    {

        InitializeInputDevices();

        climbInteractable = GetComponent<ClimbInteractable>();


        // Add all snapping points to the list
        SnappingPoints = new List<Transform>();
        foreach (Transform child in transform) {
            if (child.CompareTag(SNAPPING_POINT_TAG)){
                SnappingPoints.Add(child);
                Debug.Log("Snapping Point: " + child.name);
            }
        }

        if (climbInteractable == null)
        {
            Debug.LogError("XRClimbInteractable is missing on " + gameObject.name);
            return;
        }
        if (SnappingPoints.Count == 0)
        {
            Debug.LogError("Hand Poses are missing on " + gameObject.name);
            return;
        }
        if (rightController == null || leftController == null)
        {
            Debug.LogError("Right or Left Controller is missing on " + gameObject.name);
            return;
        }

        rightControllerTracking = new ControllerTracking(rightControllerModel.transform.localPosition, rightControllerModel.transform.localRotation);
        leftControllerTracking = new ControllerTracking(leftControllerModel.transform.localPosition, leftControllerModel.transform.localRotation);

        climbInteractable.selectEntered.AddListener(AlignHandModel);
        climbInteractable.selectExited.AddListener(ResetHandModel);
    }

    /// <summary>
    /// Triggers the alignment of the hand model of the interactor object to the closest predefined snapping point.
    /// </summary>
    /// <param name="args">Event arguments that provide context about the select enter interaction.</param>
    void AlignHandModel(SelectEnterEventArgs args)
    {
        if (args.interactorObject == null)
        {
            Debug.LogError("Interactor or Ideal Hand Pose is missing!");
            return;
        }

        // Get the visual hand model inside the interactor object
        Component triggeredComponent;
        ControllerTracking controllerTracking;

        if (args.interactorObject.transform.parent.Equals(rightController)){
            triggeredComponent = rightControllerModel;
            controllerTracking = rightControllerTracking;
        } else {
            triggeredComponent = leftControllerModel;
            controllerTracking = leftControllerTracking;
        }
        
        if (triggeredComponent == null) return;

        // Store original tracked position (to restore later)
        Transform closestHandPose = GetClosestHandPose(triggeredComponent.transform.position);
        controllerTracking.closestHandPose = closestHandPose;

        StartCoroutine(Align(triggeredComponent, controllerTracking));
    }

    /// <summary>
    /// Retrieves the closest predefined snapping point to the given hand position.
    /// </summary>
    /// <param name="handPosition">The current position of the hand for which the closest snapping point is being calculated.</param>
    /// <returns>The Transform representing the closest predefined snapping point.</returns>
    private Transform GetClosestHandPose(Vector3 handPosition)
    {
        Transform closestPose = SnappingPoints[0];
        float closestDistance = Vector3.Distance(handPosition, closestPose.position);

        foreach (Transform pose in SnappingPoints)
        {
            float distance = Vector3.Distance(handPosition, pose.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPose = pose;
            }
        }

        return closestPose;
    }

    /// <summary>
    /// Smoothly aligns the controller's position and rotation to the closest predefined hand pose while snapping is active.
    /// </summary>
    /// <param name="controller">The visual representation of the controller to be aligned.</param>
    /// <param name="controllerTracking">The tracking information associated with the controller.</param>
    /// <returns>Returns an enumerator to handle the coroutine for continuous alignment of the controller to the target pose.</returns>
    private System.Collections.IEnumerator Align(Component controller, ControllerTracking controllerTracking)
    {
        controllerTracking.isTriggered = true;
        // Keep hand aligned while selected - Not smooth for now

        while (controllerTracking.isTriggered) {
            if (Vector3.Distance(controller.transform.position, controllerTracking.closestHandPose.position) < 0.005f) break;

            Vector3 newPosition = Vector3.Lerp(controller.transform.position, controllerTracking.closestHandPose.position, Time.deltaTime * snapSpeed);
            Quaternion newRotation = Quaternion.Lerp(controller.transform.rotation, controllerTracking.closestHandPose.rotation, Time.deltaTime * snapSpeed);

            controller.transform.position = newPosition;
            controller.transform.rotation = newRotation;
            yield return null;
            
        }

        while (controllerTracking.isTriggered)
        {
            controller.transform.position = controllerTracking.closestHandPose.position;
            controller.transform.rotation = controllerTracking.closestHandPose.rotation;
            yield return null;
        }
    }

    /// <summary>
    /// Resets the hand model of the interactor object to its original position and rotation relative to the controller.
    /// </summary>
    /// <param name="args">Event arguments that provide context about the select exit interaction.</param>
    void ResetHandModel(SelectExitEventArgs args)
    {
        Component triggeredComponent;
        ControllerTracking controllerTracking;

        if (args.interactorObject.transform.parent.Equals(rightController)){
            triggeredComponent = rightControllerModel;
            controllerTracking = rightControllerTracking;
        } else {
            triggeredComponent = leftControllerModel;
            controllerTracking = leftControllerTracking;
        }

        if (triggeredComponent == null) return;

        controllerTracking.isTriggered = false;
        triggeredComponent.transform.localPosition = controllerTracking.originalRelativePosition;
        triggeredComponent.transform.localRotation = controllerTracking.originalRelativeRotation;
    }

    /// <summary>
    /// Initializes the input devices by assigning the controller models for both right
    /// and left controllers based on their tagged child components.
    /// </summary>
    private void InitializeInputDevices()
    {
        rightControllerModel = rightController.GetComponentsInChildren<Transform>(true).FirstOrDefault(child => child.CompareTag(CONTROLLER_MODEL_TAG));
        leftControllerModel = leftController.GetComponentsInChildren<Transform>(true).FirstOrDefault(child => child.CompareTag(CONTROLLER_MODEL_TAG));
    }

}
