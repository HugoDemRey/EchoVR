using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
public class GrabHandSnapping : MonoBehaviour
{

    public Component rightController;
    public Component leftController;
    public Component rightControllerModel;
    public Component leftControllerModel;

    private ControllerTracking rightControllerTracking;
    private ControllerTracking leftControllerTracking;

    public float snapSpeed = 12f; // TODO - Implement smooth transition

    private XRGrabInteractable grabInteractable;
    private List<Transform> SnappingPoints; // List of predefined hand poses, every children with tag "SnappingPoint" is added to this list

    private string SNAPPING_POINT_TAG = "SnappingPoint";

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Add all snapping points to the list
        SnappingPoints = new List<Transform>();
        foreach (Transform child in transform) {
            if (child.CompareTag(SNAPPING_POINT_TAG)) SnappingPoints.Add(child);
        }

        foreach (Transform child in transform.parent)
        {
            if (child.CompareTag(SNAPPING_POINT_TAG)) SnappingPoints.Add(child);
        }


        if (grabInteractable == null)
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

        grabInteractable.selectEntered.AddListener(AlignHandModel);
        grabInteractable.selectExited.AddListener(ResetHandModel);
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
        
        if (triggeredComponent == null) return;

        // Store original tracked position (to restore later)
        Transform closestHandPose = GetClosestHandPose(triggeredComponent.transform.position);
        controllerTracking.closestHandPose = closestHandPose;

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
        // TODO - Instead of tracking the controller position and get it closer to the wanted position, we should track the grabbed object position and get the controller closer to it
        controllerTracking.isTriggered = true;
        // Keep hand aligned while selected - Not smooth for now

        // while (controllerTracking.isTriggered) {
        //     if (Vector3.Distance(controller.transform.position, controllerTracking.closestHandPose.position) < 0.005f) break;

        //     Vector3 newPosition = Vector3.Lerp(controller.transform.position, controllerTracking.closestHandPose.position, Time.deltaTime * snapSpeed);
        //     Quaternion newRotation = Quaternion.Lerp(controller.transform.rotation, controllerTracking.closestHandPose.rotation, Time.deltaTime * snapSpeed);

        //     controller.transform.position = newPosition;
        //     controller.transform.rotation = newRotation;
        //     yield return null;
            
        // }
        controller.transform.position = controllerTracking.closestHandPose.position;
        controller.transform.rotation = controllerTracking.closestHandPose.rotation;
        yield return null;
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

        controllerTracking.isTriggered = false;
        triggeredComponent.transform.localPosition = controllerTracking.originalRelativePosition;
        triggeredComponent.transform.localRotation = controllerTracking.originalRelativeRotation;
    }
}
