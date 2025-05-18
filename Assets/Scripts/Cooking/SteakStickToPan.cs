using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SteakStickToPan : MonoBehaviour
{

    private Transform steakPlacement;
    private Transform initialParent;

    private CookingManager cookingManager;

    private void Start()
    {
        steakPlacement = transform.parent.Find("SteakPlacement");
        if (steakPlacement == null)
        {
            Debug.LogError("SteakPlacement not found in the pan object.");
        }
        initialParent = transform.parent.parent.Find("Steak");
        Debug.Log("Initial parent set to: " + initialParent.name);

        cookingManager = transform.parent.GetComponent<CookingManager>();

    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Steak"))
        {
            Debug.Log("Steak entered the pan trigger");
            other.transform.SetParent(transform.parent);
            other.transform.position = steakPlacement.position;
            other.transform.rotation = steakPlacement.rotation;
            XRGrabInteractable gi = other.GetComponent<XRGrabInteractable>();
            // disable the grab interactable
            gi.enabled = false;

            cookingManager.isSteakInPan = true;

            Debug.Log("Steak parent set to: " + transform.parent.name);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Steak"))
        {
            Debug.Log("Steak exited the pan trigger");
            other.transform.SetParent(initialParent);

            XRGrabInteractable gi = other.GetComponent<XRGrabInteractable>();
            // enable the grab interactable
            gi.enabled = true;

            cookingManager.isSteakInPan = false;

            Debug.Log("Steak parent set back to: " + initialParent.name);
        }
    }
}
