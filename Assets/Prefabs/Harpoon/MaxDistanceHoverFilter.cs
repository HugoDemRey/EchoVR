using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Prefabs.Harpoon
{
    public class MaxDistanceSelectFilter : IXRSelectFilter
    {
        public float MaxDistance;
    
        public bool canProcess => true;

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            return Vector3.Distance(interactor.transform.position, interactable.transform.position) <= MaxDistance;
        }
    
        public void OnValidate()
        {
            MaxDistance = Mathf.Clamp(MaxDistance, 0, float.MaxValue);
        }
    }
}