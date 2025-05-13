using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Prefabs.Rope
{
    public class ZipLineBehavior : XRSocketInteractor
    {
        public ZipLineHandleBehavior handlePrefab;
        public RopeBehavior rope;
        public String handleTag;

        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            return base.CanSelect(interactable) && interactable.transform.CompareTag(handleTag);
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            Debug.Log("OnSelectEntered in ZipLineBehavior");

            Transform pos = transform;
            Destroy(this);
            Destroy(args.interactableObject.transform.gameObject);
            ZipLineHandleBehavior handle = Instantiate(handlePrefab);
            handle.transform.parent = transform.parent;
            handle.ForceUpdate(rope, pos);
        }
    }
}
