using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
public class HandSnapping : MonoBehaviour
{
    public List<Transform> SnappingPoints; // List of predefined hand poses

    public Component rightController;
    public Component leftController;
    public Component rightControllerModel;
    public Component leftControllerModel;

    private ControllerTracking rightControllerTracking;
    private ControllerTracking leftControllerTracking;

    // public float snapSpeed = 5f; // TODO - Implement smooth transition

    private ClimbInteractable climbInteractable;
    private bool isAligning = false;

    void Awake()
    {
        climbInteractable = GetComponent<ClimbInteractable>();

        if (climbInteractable == null)
        {
            Debug.LogError("XRGrabInteractable is missing on " + gameObject.name);
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
        Debug.Log("Entered Triggered on " + triggeredComponent);
        
        if (triggeredComponent == null) return;

        // Store original tracked position (to restore later)
        Transform closestHandPose = GetClosestHandPose(triggeredComponent.transform.position);
        controllerTracking.closestHandPose = closestHandPose;

        // Start smooth transition to ideal pose
        isAligning = true;
        StartCoroutine(Align(triggeredComponent, controllerTracking));
    }

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

    private System.Collections.IEnumerator Align(Component controller, ControllerTracking controllerTracking)
    {
        controllerTracking.isTriggered = true;
        // Keep hand aligned while selected - Not smooth for now
        while (controllerTracking.isTriggered)
        {
            controller.transform.position = controllerTracking.closestHandPose.position;
            controller.transform.rotation = controllerTracking.closestHandPose.rotation;
            yield return null;
        }
    }

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

        Debug.Log("Exited Triggered on " + triggeredComponent);

        controllerTracking.isTriggered = false;
        triggeredComponent.transform.localPosition = controllerTracking.originalRelativePosition;
        triggeredComponent.transform.localRotation = controllerTracking.originalRelativeRotation;
    }
}
