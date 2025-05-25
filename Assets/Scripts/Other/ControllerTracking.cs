using UnityEngine;

/// <summary>
/// Represents the tracking data for a controller, including its position and rotation relative to its original state.
/// Provides functionality to store and manage the original relative position, rotation, and the closest hand pose.
/// </summary>
public class ControllerTracking
{
    public Vector3 originalRelativePosition { get; set; }
    public Quaternion originalRelativeRotation { get; set; }

    public Transform closestHandPose { get; set; }
    public bool isTriggered { get; set; }

    public ControllerTracking(Vector3 localPos, Quaternion localRot)
    {
        this.originalRelativePosition = localPos;
        this.originalRelativeRotation = localRot;
        this.closestHandPose = null;
        this.isTriggered = false;
    }
}
