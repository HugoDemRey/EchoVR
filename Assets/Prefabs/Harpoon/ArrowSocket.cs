using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Prefabs.Harpoon
{
    public class ArrowSocket : XRSocketInteractor
    {
        public HarpoonBehavior harpoon;
        public string arrowTag = "Arrow";
        
        private IXRSelectInteractable selectedInteractable;
        
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (args.interactableObject.transform.CompareTag(arrowTag))
            {
                base.OnSelectEntered(args);
                Debug.Log("Reloading harpoon");
                harpoon.Reload();
                selectedInteractable = args.interactableObject;
            }
        }
        
        public void DestroyArrow()
        {
            if (selectedInteractable != null)
            {
                Destroy(selectedInteractable.transform.gameObject);
            }
        }
    }
}