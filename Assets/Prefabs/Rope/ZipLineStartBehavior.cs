using Prefabs.Harpoon;
using Prefabs.ZipLineHarpoonHandle;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Prefabs.Rope
{
    public class ZipLineStartBehavior : XRSocketInteractor
    {
        public ZiplineHandle handlePrefab;
        public RopeBehavior rope;
        
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (args.interactableObject is not HarpoonBehavior) return;
            
            Transform position = args.interactableObject.transform; 
            Destroy(args.interactableObject.transform.gameObject);
            ZiplineHandle handle = Instantiate(handlePrefab);
            handle.ForceUpdate(rope, position);
        }
    }
}
