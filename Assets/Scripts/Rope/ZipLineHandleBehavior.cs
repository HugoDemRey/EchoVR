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

    //TODO turn into a coroutine
    private void Update() {
        if (_running && UpdateAnimation())
        {
            _running = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            Destroy(this);
        }

        if (_triggerAnimation && !_running)
        {
            TriggerAnimation();
        }
    }

    /// Updates the animation state for the zipline handle movement. Calculates the new position
    /// of the handle and player, if attached.
    /// <returns>
    /// Returns true if the animation has completed and the handle should not be contrained anymore.
    /// </returns>
    private bool UpdateAnimation()
    {
        var currentTime = Time.time;
        var currentDurationPct = Mathf.Clamp01((currentTime - _animationStartTime) / _animationDuration);

        Vector3 newPosition = Vector3.Lerp(
            ropeBehavior.GetStartPoint(),
            ropeBehavior.GetEndPoint(),
            GetPositionPctFromTimePct(currentDurationPct)
        );

        if (_attached)
        {
            Vector3 relativePosition = newPosition - transform.position;
            _characterController.Move(relativePosition);
        }

        transform.position = newPosition;

        return currentTime - _animationStartTime >= _animationDuration;
    }

    /// Initiates the zipline handle animation by setting the parameters.
    private void TriggerAnimation()
    {
        _running = true;
        _triggerAnimation = false;
        _animationStartTime = Time.time;
        _animationDuration = Vector3.Distance(ropeBehavior.GetStartPoint(), ropeBehavior.GetEndPoint()) / 5f;
    }

    private float GetPositionPctFromTimePct(float timePct)
    {
        return (Mathf.Pow(timePct + 1, 2f) - 1) / 3f;
    }
    
    public void ForceUpdate(RopeBehavior newRopeBehavior, Transform newPivotTransform)
    {
        ropeBehavior = newRopeBehavior;

        transform.position = newPivotTransform.position;
        transform.rotation = Quaternion.Euler(
            0,
            transform.parent.rotation.eulerAngles.y,
            0
        );
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
        
        if (_characterController == null)
        {
            Debug.LogWarning("No character controller found. The XR Rig should be tagged as 'Origin'.");
        }

        climbProvider.enabled = false;
    }

    private void DetachPlayer()
    {
        _attached = false;
        climbProvider.enabled = true;
    }
}
