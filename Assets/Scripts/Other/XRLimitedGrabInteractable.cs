using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Prefabs
{
    /// <summary>
    /// Represents an XRGrabInteractable object that can be grabbed
    /// or hovered within a limited radius.
    /// </summary>
    public class XRLimitedGrabInteractable : XRGrabInteractable
    {
        /// <summary>
        /// Specifies the maximum distance within which an object can be hovered or grabbed
        /// by an interactor. If the interactor is outside this radius, selection and hover are not allowed.
        /// </summary>
        public float grabRadius = 1f;

        /// <summary>
        /// Indicates whether the interactable object is currently being held by an interactor.
        /// </summary>
        protected bool isHeld { get; private set; }

        /// <summary>
        /// Registers event listeners.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(OnGrab);
            selectExited.AddListener(OnRelease);
        }

        /// <summary>
        /// Unregisters event listeners.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            selectEntered.RemoveListener(OnGrab);
            selectExited.RemoveListener(OnRelease);
        }

        /// <summary>
        /// Determines if the interactable object can be selected by the specified interactor,
        /// considering distance constraints and the base class's selection conditions.
        /// </summary>
        /// <param name="interactor">The interactor attempting to select the object.</param>
        /// <returns>
        /// True if the interactor is within the defined grab radius
        /// and the base class's selection conditions are met; otherwise, false.
        /// </returns>
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            return !IsTooFar(interactor.transform) && base.IsSelectableBy(interactor);
        }

        /// <summary>
        /// Determines if the interactable object can be hovered over
        /// by the specified interactor, considering the object's held state
        /// and distance constraints.
        /// </summary>
        /// <param name="interactor">The interactor attempting to hover over the object.</param>
        /// <returns>
        /// True if the object is not currently held, the interactor is within the defined grab radius,
        /// and the base class's hover conditions are met; otherwise, false.
        /// </returns>
        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return !isHeld && !IsTooFar(interactor.transform) && base.IsHoverableBy(interactor);
        }

        /// <summary>
        /// Determines if the distance between the interactable object and another transform
        /// exceeds the defined grab radius.
        /// </summary>
        /// <param name="otherTransform">The transform to compare the distance against.</param>
        /// <returns>
        /// True if the distance between the interactable object's position and the other transform's position
        /// is greater than the defined grab radius; otherwise, false.
        /// </returns>
        private bool IsTooFar(Transform otherTransform)
        {
            return Vector3.Distance(transform.position, otherTransform.position) > grabRadius;
        }

        /// <summary>
        /// Updates the state to indicate that the object is currently being held.
        /// </summary>
        /// <param name="args">Ignored</param>
        protected void OnGrab(SelectEnterEventArgs args)
        {
            if (args.interactorObject is not XRSocketInteractor)
            {
                Debug.Log(transform.name + " OnGrab called by " + args.interactorObject.transform.name);
                isHeld = true;
            }
        }

        /// <summary>
        /// Updates the state to indicate that the object is currently being held.
        /// </summary>
        /// <param name="args">Ignored</param>
        private void OnRelease(SelectExitEventArgs args)
        {
            if (args.interactorObject is not XRSocketInteractor)
            {
                Debug.Log(transform.name + " OnRelease called by " + args.interactorObject.transform.name);
                isHeld = false;
            }
        }
    }
}