using Prefabs.Harpoon;
using Prefabs.ZipLineHandle;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Prefabs.Rope
{
    public class ZipLineBehavior : XRSocketInteractor
    {
        public ZiplineHandle handlePrefab;
        public RopeBehavior rope;
        
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (args.interactableObject is not HarpoonBehavior) return;

            Transform pos = transform;
            Destroy(this);
            Destroy(args.interactableObject.transform.gameObject);
            ZiplineHandle handle = Instantiate(handlePrefab);
            handle.ForceUpdate(rope, pos);
        }
    }
}
