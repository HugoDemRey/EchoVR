using UnityEngine;

public class ProximityStringDetector : MonoBehaviour
{
    public Transform rightHandTransform;
    public BoxCollider stringsAreaCollider;
    [Tooltip("Facteur pour agrandir la zone de détection autour du BoxCollider")]
    public Vector3 detectionScaleMultiplier = new Vector3(1.5f, 1.5f, 1.5f);

    private GuitarSoundController guitarSoundController;
    private bool isHandInside = false;

    void Start()
    {
        guitarSoundController = GetComponent<GuitarSoundController>();
        if (guitarSoundController == null)
            Debug.LogError("GuitarSoundController non trouvé !");
    }

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
            Debug.Log("Main entrée dans la zone des cordes (agrandie)");
        }
        else if (!inside && isHandInside)
        {
            isHandInside = false;
            guitarSoundController.OnHandTouch(false);
            Debug.Log("Main sortie de la zone des cordes (agrandie)");
        }
    }
}
