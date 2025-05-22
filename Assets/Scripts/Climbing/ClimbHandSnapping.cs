using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
public class ClimbHandSnapping : MonoBehaviour
{

    public Transform rightController;
    public Transform leftController;
    private Component rightControllerModel;
    private Component leftControllerModel;

    private ControllerTracking rightControllerTracking;
    private ControllerTracking leftControllerTracking;

    public float snapSpeed = 8f; // TODO - Implement smooth transition

    private ClimbInteractable climbInteractable;
    private List<Transform> SnappingPoints; // List of predefined hand poses, every children with tag "SnappingPoint" is added to this list

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

        Debug.Log("Snapping Points: " + SnappingPoints.Count);
        // foreach (Transform child in transform.parent)
        // {
        //     if (child.CompareTag(SNAPPING_POINT_TAG)) SnappingPoints.Add(child);
        // }


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

    private void InitializeInputDevices()
    {
        // rightController = GameObject.FindGameObjectWithTag("RightController").transform;
        // leftController = GameObject.FindGameObjectWithTag("LeftController").transform;

        Debug.Log("Right Controller: " + rightController);
        Debug.Log("Left Controller: " + leftController);

        rightControllerModel = rightController.GetComponentsInChildren<Transform>(true).FirstOrDefault(child => child.CompareTag(CONTROLLER_MODEL_TAG));
        leftControllerModel = leftController.GetComponentsInChildren<Transform>(true).FirstOrDefault(child => child.CompareTag(CONTROLLER_MODEL_TAG));

        Debug.Log("Right Controller Model: " + rightControllerModel);
        Debug.Log("Left Controller Model: " + leftControllerModel);

    }

}
