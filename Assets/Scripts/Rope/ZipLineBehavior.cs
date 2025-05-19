using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Prefabs.Rope
{
    /// <summary>
    /// Represents the behavior of a zip line start.
    /// The class allows a GrabbableZipLineHandle to be selected by the socket, replacing it by a climbable ZipLineHandleBehavior. 
    /// </summary>
    public class ZipLineBehavior : XRSocketInteractor
    {
        /// <summary>
        /// The prefab for the zip line handle.
        /// </summary>
        public ZipLineHandleBehavior handlePrefab;

        /// <summary>
        /// The rope component used, passed to the ZipLineHandleBehavior instance.
        /// </summary>
        public RopeBehavior rope;

        /// <summary>
        /// The tag used to identify a grabbable zip line handle.
        /// </summary>
        public String handleTag;

        /// <summary>
        /// Determines whether the interactable can be selected by the socket.
        /// </summary>
        /// <param name="interactable">The interactable object to evaluate for selection.</param>
        /// <returns>True if the interactable has the right tag.</returns>
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            return base.CanSelect(interactable) && interactable.transform.CompareTag(handleTag);
        }

        /// <summary>
        /// Called when a GrabbableZipLineHandle is selected by the socket.
        /// </summary>
        /// <param name="args">The event arguments containing details of the selection.</param>
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            Debug.Log("OnSelectEntered in ZipLineBehavior");
            
            Destroy(args.interactableObject.transform.gameObject); // Destroy the GrabbableZipLineHandle to replace it
            GetComponent<MeshRenderer>().enabled = false; // Hide handle preview
            enabled = false; // Disable the socket
            
            // Replace by a climbable ZipLineHandleBehavior, which has the Rope as parent
            ZipLineHandleBehavior handle = Instantiate(handlePrefab, transform.parent.parent, true);
            handle.ForceUpdate(rope, transform.parent); // Update position based on the ZipLineStart transform
        }

        /// <summary>
        /// Reactivates the zip line socket by enabling its functionality and making the handle preview visible.
        /// </summary>
        public void Reactivate()
        {
            GetComponent<MeshRenderer>().enabled = true; // Show handle preview
            enabled = true; // Enable the socket
        }
    }
}
