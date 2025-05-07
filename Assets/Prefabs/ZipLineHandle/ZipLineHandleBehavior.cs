using Prefabs.Rope;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

public class ZipLineHandleBehavior : ClimbInteractable
{
    public RopeBehavior ropeBehavior;

    private IXRSelectInteractor _leftHandInteractor;
    private IXRSelectInteractor _rightHandInteractor;
    
    private CharacterController _characterController;

    private bool _triggerAnimation;
    private bool _running;
    private bool _attached;
    private float _animationStartTime;
    private float _animationDuration;

    private void Update() {
        if (_running)
        {
            var currentTime = Time.time;
            var currentDuration = Mathf.Clamp01((currentTime - _animationStartTime) / _animationDuration);

            Vector3 newPosition = Vector3.Lerp(
                ropeBehavior.GetStartPoint(),
                ropeBehavior.GetEndPoint(),
                UpdatePosition(currentDuration)
            );

            if (_attached)
            {
                Vector3 relativePosition = newPosition - transform.position;
                _characterController.Move(relativePosition);
            }

            transform.position = newPosition;

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
            _animationDuration = Vector3.Distance(ropeBehavior.GetStartPoint(), ropeBehavior.GetEndPoint()) / 5f;
        }
    }

    private float UpdatePosition(float time)
    {
        return (Mathf.Pow(time + 1, 2f) - 1) / 3f;
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
        }
        else if (_rightHandInteractor is null && args.interactorObject.handedness == InteractorHandedness.Right)
        {
            _rightHandInteractor = args.interactorObject;
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

    private void AttachPlayer()
    {
        _attached = true;
        _triggerAnimation = true;
        _characterController = GameObject.FindGameObjectWithTag("Origin").GetComponent<CharacterController>();
        climbProvider.enabled = false;
    }

    private void DetachPlayer()
    {
        _attached = false;
        climbProvider.enabled = true;
    }
}
