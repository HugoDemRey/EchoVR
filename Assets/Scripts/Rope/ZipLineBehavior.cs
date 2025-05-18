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

            Transform pos = transform; // Save the transform to pass it to the climbable ZipLineHandleBehavior
            Destroy(this);
            Destroy(args.interactableObject.transform.gameObject); // Destroy the GrabbableZipLineHandle to replace it
            ZipLineHandleBehavior handle = Instantiate(handlePrefab); // Replace by a climbable ZipLineHandleBehavior
            handle.transform.parent = transform.parent; // Needed for placement & rotation
            handle.ForceUpdate(rope, pos); // Update position
        }
    }
}
