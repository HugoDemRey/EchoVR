using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Prefabs.Harpoon
{
    /// <summary>
    /// Defines a custom socket interactor that interacts specifically with arrows,
    /// allowing functionality such as detecting when an arrow is inserted
    /// and reloading the associated harpoon mechanism.
    /// </summary>
    public class ArrowSocket : XRSocketInteractor
    {
        /// <summary>
        /// A reference to the associated HarpoonBehavior instance.
        /// This variable is used to interact with and control the harpoon's functionality,
        /// such as triggering the reload mechanism when an arrow is inserted into the socket.
        /// </summary>
        public HarpoonBehavior harpoon;

        /// <summary>
        /// Specifies the tag used to identify objects as valid arrows for interaction with the ArrowSocket.
        /// </summary>
        public string arrowTag = "Arrow";

        /// <summary>
        /// Stores a reference to the interactable object currently selected by the ArrowSocket.
        /// This variable is used to destroy the object when it is no longer needed.
        /// </summary>
        private IXRSelectInteractable _selectedInteractable;

        /// <summary>
        /// Handles the event when an interactable object is selected and entered into the socket.
        /// This method ensures that only objects with a specific tag (e.g., "Arrow")
        /// are valid for selection, reloads the associated harpoon mechanism,
        /// and keeps a reference to the selected interactable object.
        /// </summary>
        /// <param name="args">Arguments containing information about the selected interactable object.</param>
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (args.interactableObject.transform.CompareTag(arrowTag))
            {
                base.OnSelectEntered(args);
                Debug.Log("Reloading harpoon");
                harpoon.Reload();
                _selectedInteractable = args.interactableObject;
            }
        }

        /// <summary>
        /// Destroys the currently selected interactable arrow object in the socket.
        /// This method ensures proper cleanup of the arrow object by removing it from the scene
        /// when no longer needed, such as after the harpoon has been fired.
        /// </summary>
        public void DestroyArrow()
        {
            if (_selectedInteractable != null)
            {
                Destroy(_selectedInteractable.transform.gameObject);
            }
        }
    }
}