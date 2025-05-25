using UnityEngine;

/// <summary>
/// Detects proximity of the right-hand transform to the strings area defined by a BoxCollider.
/// </summary>
public class ProximityStringDetector : MonoBehaviour
{
    /// <summary>
    /// A reference to the Transform component representing the right hand.
    /// </summary>
    public Transform rightHandTransform;

    /// <summary>
    /// BoxCollider defining the area where proximity detection occurs with the right-hand transform.
    /// </summary>
    public BoxCollider stringsAreaCollider;
    [Tooltip("Facteur pour agrandir la zone de d�tection autour du BoxCollider")]
    public Vector3 detectionScaleMultiplier = new Vector3(1.5f, 1.5f, 1.5f);

    private GuitarSoundController guitarSoundController;
    private bool isHandInside = false;
    
    void Start()
    {
        guitarSoundController = GetComponent<GuitarSoundController>();
        if (guitarSoundController == null)
            Debug.LogError("GuitarSoundController non trouv� !");
    }

    /// <summary>
    /// Called every frame to detect whether the right-hand transform is inside the proximity
    /// area defined by the `stringsAreaCollider`. If the hand enters or exits the area, the
    /// method triggers actions in the `GuitarSoundController` to handle the state change.
    /// </summary>
    void Update()
    {
        if (rightHandTransform == null || stringsAreaCollider == null)
            return;

        Vector3 center = stringsAreaCollider.transform.position;
        Vector3 baseHalfExtents = Vector3.Scale(stringsAreaCollider.size, stringsAreaCollider.transform.lossyScale) / 2f;

        Vector3 scaledHalfExtents = Vector3.Scale(baseHalfExtents, detectionScaleMultiplier);

        Quaternion orientation = stringsAreaCollider.transform.rotation;

        Vector3 localPos = stringsAreaCollider.transform.InverseTransformPoint(rightHandTransform.position);

        bool inside =
            localPos.x > -scaledHalfExtents.x && localPos.x < scaledHalfExtents.x &&
            localPos.y > -scaledHalfExtents.y && localPos.y < scaledHalfExtents.y &&
            localPos.z > -scaledHalfExtents.z && localPos.z < scaledHalfExtents.z;

        if (inside && !isHandInside)
        {
            isHandInside = true;
            guitarSoundController.OnHandTouch(true);
            Debug.Log("Main entr�e dans la zone des cordes (agrandie)");
        }
        else if (!inside && isHandInside)
        {
            isHandInside = false;
            guitarSoundController.OnHandTouch(false);
            Debug.Log("Main sortie de la zone des cordes (agrandie)");
        }
    }
}
