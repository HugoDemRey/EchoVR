using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Prefabs.Rope
{
    public class ZipLineBehavior : XRSocketInteractor
    {
        public ZipLineHandleBehavior handlePrefab;
        public RopeBehavior rope;
        public String handleTag;
 
            protected override void OnSelectEntered(SelectEnterEventArgs args)
            {
                Debug.Log("OnSelectEntered in ZipLineBehavior");
                if (!args.interactableObject.transform.CompareTag(handleTag)) return;

                Transform pos = transform;
                Destroy(this);
                Destroy(args.interactableObject.transform.gameObject);
                ZipLineHandleBehavior handle = Instantiate(handlePrefab);
                handle.ForceUpdate(rope, pos);
            }
    }
}
