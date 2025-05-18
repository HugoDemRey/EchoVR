using UnityEngine;

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
