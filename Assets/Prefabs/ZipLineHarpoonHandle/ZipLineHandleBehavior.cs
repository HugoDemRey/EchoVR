using Prefabs.Rope;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Prefabs.ZipLineHarpoonHandle
{
    public class ZiplineHandle : XRGrabInteractable
    {
        public Transform leftHandTransform;
        public Transform rightHandTransform;
        public RopeBehavior ropeBehavior;

        private IXRSelectInteractor _leftHandInteractor;
        private IXRSelectInteractor _rightHandInteractor;
        
        private Transform _playerOrigin;

        public void ForceUpdate(RopeBehavior newRopeBehavior, Transform newPosition)
        {
            ropeBehavior = newRopeBehavior;
            transform.position = newPosition.position;
            transform.rotation = newPosition.rotation;
        }
        
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            
            if (_leftHandInteractor is null && args.interactorObject.handedness == InteractorHandedness.Left)
            {
                _leftHandInteractor = args.interactorObject;
                SetHandPosition(args.interactorObject, leftHandTransform);
            }
            else if (_rightHandInteractor is null && args.interactorObject.handedness == InteractorHandedness.Right)
            {
                _rightHandInteractor = args.interactorObject;
                SetHandPosition(args.interactorObject, rightHandTransform);
            }
            
            if (_leftHandInteractor is not null && _rightHandInteractor != null)
            {
                AttachPlayer();
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            switch (args.interactorObject.handedness)
            {
                case InteractorHandedness.Left:
                    _leftHandInteractor = null;
                    break;
                case InteractorHandedness.Right:
                    _rightHandInteractor = null;
                    break;
            }

            if (_leftHandInteractor is null || _rightHandInteractor is null)
            {
                DetachPlayer();
            }
        }

        private void SetHandPosition(IXRSelectInteractor interactor, Transform handTarget)
        {
            interactor.transform.position = handTarget.position;
            interactor.transform.rotation = handTarget.rotation;
        }

        private void AttachPlayer()
        {
            _playerOrigin ??= _leftHandInteractor.transform.root;
            _playerOrigin.SetParent(transform, true);
            ropeBehavior.StartZipLine(this);
        }

        private void DetachPlayer()
        {
            _playerOrigin?.SetParent(null, true);
        }
    }
}
