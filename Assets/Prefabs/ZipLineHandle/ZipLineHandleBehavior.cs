using System;
using Prefabs.Rope;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

namespace Prefabs.ZipLineHandle
{
    public class ZiplineHandle : ClimbInteractable
    {
        public Transform leftHandTransform;
        public Transform rightHandTransform;
        public RopeBehavior ropeBehavior;

        private IXRSelectInteractor _leftHandInteractor;
        private IXRSelectInteractor _rightHandInteractor;
        
        private Transform _playerOrigin;

        private bool _triggerAnimation;
        private bool _running;
        private float _animationStartTime;
        private float _animationDuration;

        private void Start()
        {
            // _triggerAnimation = true;
            // TODO
        }

        private void Update() {
            if (_running)
            {
                var currentTime = Time.time;
                var currentDuration = Mathf.Clamp01((currentTime - _animationStartTime) / _animationDuration);

                transform.position = Vector3.Lerp(
                    ropeBehavior.GetStartPoint(),
                    ropeBehavior.GetEndPoint(),
                    GetPosition(currentDuration)
                );

                if (currentTime - _animationStartTime >= _animationDuration)
                {
                    _running = false;
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    Destroy(this);
                    return;
                }
            }

            if (_triggerAnimation && !_running)
            {
                _running = true;
                _triggerAnimation = false;
                _animationStartTime = Time.time;
                _animationDuration = Vector3.Distance(ropeBehavior.GetStartPoint(), ropeBehavior.GetEndPoint());
            }
        }

        private float GetPosition(float distance)
        {
            return Mathf.Pow(distance, 2);
        }
        
        public void ForceUpdate(RopeBehavior newRopeBehavior, Transform newPivotTransform)
        {
            ropeBehavior = newRopeBehavior;
            
            transform.position = newPivotTransform.position;
            transform.rotation = newPivotTransform.rotation;
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
            _triggerAnimation = true;
        }

        private void DetachPlayer()
        {
            _playerOrigin?.SetParent(null, true);
        }
    }
}
