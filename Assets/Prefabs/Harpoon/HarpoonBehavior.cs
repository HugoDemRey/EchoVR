using System;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Prefabs.Harpoon
{
    public class HarpoonBehavior : XRGrabInteractable
    {
        public InputActionReference triggerAction;
        public GameObject harpoonTip;
        public float grabRadius;
        public Sprite crosshairSprite;
        public bool infiniteAmmo = true;

        private XRGrabInteractable _grabInteractable;
        private bool _isHeld;

        private Stage _stage;
        private Vector3 _start;
        private Vector3 _end;

        private SpriteRenderer _renderer;
        private GameObject _crosshair;
        private Camera _mainCamera;      // Reference to the main camera

        private enum Stage
        {
            NonePlaced,
            StartPlaced,
            EndPlaced,
            Done
        }

        private void Start()
        {
            // Create a GameObject to hold the Sprite
            _crosshair = new GameObject("CrossSprite");
            _renderer = _crosshair.AddComponent<SpriteRenderer>();
            _renderer.sprite = crosshairSprite;             // Assign the PNG Sprite
            _renderer.sortingOrder = 10;                // Keep it in front of most objects
            _crosshair.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f); // Adjust cross size
            
            // Cache the main camera for better performance
            _mainCamera = Camera.main;
        }

        protected override void Awake()
        {
            base.Awake();
            _grabInteractable = GetComponent<XRGrabInteractable>();
            _grabInteractable.selectEntered.AddListener(OnGrab);
            _grabInteractable.selectExited.AddListener(OnRelease);
        }

        private void Update()
        {
            if (_isHeld && Physics.Raycast(harpoonTip.transform.position, harpoonTip.transform.forward, out var hit))
            {
                // Move the cross to the hit point and align it with the surface normal
                _crosshair.transform.position = hit.point + hit.normal * 0.01f; // Prevent z-fighting
                
                // Make the sprite face the camera (billboarding)
                _crosshair.transform.LookAt(_mainCamera.transform);
                _crosshair.transform.Rotate(0, 180, 0); // Flip to face correctly since LookAt might make it face backward

                _renderer.color = IsValidTarget(hit.collider) ? Color.green : Color.red;
                
                _crosshair.SetActive(true);
            }
            else _crosshair.SetActive(false);
        }

        private bool IsTooFar(Transform otherTransform)
        {
            return Vector3.Distance(transform.position, otherTransform.position) > grabRadius;
        }

        private bool IsValidTarget(Collider c)
        {
            return c.CompareTag("Harpoonable");
        }

        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            return !IsTooFar(interactor.transform) && base.IsSelectableBy(interactor);
        }

        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return !_isHeld && !IsTooFar(interactor.transform) && base.IsHoverableBy(interactor);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            triggerAction.action.Enable();
            triggerAction.action.performed += OnTriggerPressed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            triggerAction.action.Disable();
            triggerAction.action.performed -= OnTriggerPressed;
        }

        private void OnGrab(SelectEnterEventArgs args)
        {
            _isHeld = true;
        }

        private void OnRelease(SelectExitEventArgs args)
        {
            _isHeld = false;
        }

        private void OnTriggerPressed(InputAction.CallbackContext context)
        {
            if (!_isHeld) return;
            Shoot();
        }

        private void Shoot()
        {
            if (!Physics.Raycast(harpoonTip.transform.position, harpoonTip.transform.forward, out var hit)) return;
            if (!IsValidTarget(hit.collider)) return;

            switch (_stage)
            {
                case Stage.NonePlaced:
                    _start = hit.point;
                    _stage = Stage.StartPlaced;
                    break;
                case Stage.StartPlaced:
                    _end = hit.point;
                    _stage = Stage.EndPlaced;
                    break;
                case Stage.EndPlaced:
                    PlaceRope();
                    _stage = infiniteAmmo ? Stage.NonePlaced : Stage.Done;
                    break;
                case Stage.Done:
                default:
                    break;
            }
        }

    private void PlaceRope()
    {
        if (_stage is not Stage.EndPlaced) throw new Exception("Rope placement attempted in wrong stage.");
        
        LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;    
        
        lineRenderer.SetPosition(0, _start);
        lineRenderer.SetPosition(1, _end);
    }
}

}