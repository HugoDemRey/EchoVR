using UnityEngine;
using UnityEditor; // Required for Handles

public class OverrideMomentumForce : MonoBehaviour
{
    [SerializeField]
    public float forceMagnitude = 0f; // The magnitude of the force

    public Vector3 forceDirection = new Vector3(0, 0, 0); // The force to apply to the character controller

    [Tooltip("The duration of the momentum effect in seconds. If set to 0 (default), the force will stop normally with the default decay rate defined in the momentum manager.")]
    [Header("Momentum Duration (0 = default duration)")]
    public float seconds = 0; // Duration of the momentum effect in seconds

    private void OnValidate()
    {
    }

    /// <summary>
    /// Draws visual debugging aids in the scene view to represent the force direction and magnitude.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (forceDirection == Vector3.zero || forceMagnitude == 0f)
        {
            return;
        }

        Gizmos.color = Color.red;
        Vector3 normalizedDirection = forceDirection.normalized;
        Gizmos.DrawLine(transform.position, transform.position + normalizedDirection * forceMagnitude * 0.3f);
        Vector3 arrowHead = transform.position + normalizedDirection * forceMagnitude * 0.3f;
        float coneRadius = 0.1f; // Radius of the cone base
        int coneSegments = 20; // Number of segments for the cone base

        // Draw the cone sides
        for (int i = 0; i < coneSegments; i++)
        {
            float angle1 = i / (float)coneSegments * Mathf.PI * 2;
            float angle2 = (i + 1) / (float)coneSegments * Mathf.PI * 2;

            Vector3 point1 = arrowHead + Quaternion.LookRotation(normalizedDirection) * new Vector3(Mathf.Cos(angle1) * coneRadius, Mathf.Sin(angle1) * coneRadius, 0) - normalizedDirection * 0.2f;
            Vector3 point2 = arrowHead + Quaternion.LookRotation(normalizedDirection) * new Vector3(Mathf.Cos(angle2) * coneRadius, Mathf.Sin(angle2) * coneRadius, 0) - normalizedDirection * 0.2f;

            Gizmos.DrawLine(point1, point2); // Draw base edge
            Gizmos.DrawLine(arrowHead, point1); // Connect base to tip
        }

    }
}
