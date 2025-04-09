using System;
using Prefabs.Rope;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Prefabs.Harpoon
{
    public class HarpoonBehavior : XRGrabInteractable
    {
        public InputActionReference triggerAction;
        public GameObject harpoonTip;
        public GameObject ropePrefab;
        public float grabRadius;
        public bool infiniteAmmo = true;
        
        public Material triggerMaterial;
        
        public Sprite validCrosshairSprite;
        public Sprite invalidCrosshairSprite;
        
        public String ropeStartTag;
        public String ropeEndTag;

        private XRGrabInteractable _grabInteractable;
        private bool _isHeld;

        private TargetType _targetType = TargetType.None;
        private Stage _stage = Stage.None;
        private Vector3 _start;
        private Vector3 _end;
        
        private SpriteRenderer _renderer;
        private GameObject _crosshair;
        private Camera _mainCamera;
        private LineRenderer _lineRenderer;

        private enum Stage
        {
            None,
            StartPlaced,
            Done
        }

        private enum TargetType
        {
            None,
            Invalid, 
            Valid
        }

        private void Start()
        {
            _crosshair = new GameObject("CrossSprite");
            _renderer = _crosshair.AddComponent<SpriteRenderer>();
            _renderer.sortingOrder = 10; // Gives priority
            _crosshair.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
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
            TargetType nextTargetType = TargetType.None;
            if (_isHeld && Physics.Raycast(harpoonTip.transform.position, harpoonTip.transform.forward, out var hit))
            {
                nextTargetType = IsValidTarget(hit.collider, _stage) ? TargetType.Valid : TargetType.Invalid;
                
                _crosshair.transform.position = hit.point + hit.normal * 0.01f;
                _crosshair.transform.LookAt(_mainCamera.transform);
                _crosshair.transform.Rotate(0, 180, 0);
                
                if (_stage == Stage.StartPlaced)
                {
                    _lineRenderer ??= _crosshair.AddComponent<LineRenderer>();
                
                    _lineRenderer.startWidth = 0.01f;
                    _lineRenderer.endWidth = 0.01f;
                    _lineRenderer.positionCount = 2;
                    _lineRenderer.useWorldSpace = true;
                    _lineRenderer.material = triggerMaterial;
                    
                    _lineRenderer.SetPosition(0, _start);
                    _lineRenderer.SetPosition(1, hit.point);
                }
            }
            
            if (nextTargetType == TargetType.Valid && _targetType != TargetType.Valid)
            {
                // todo play sound
            }
            
            _targetType = nextTargetType;
            
            switch (_targetType)
            {
                case TargetType.Valid:
                    _renderer.sprite = validCrosshairSprite;
                    break; 
                case TargetType.Invalid:
                    _renderer.sprite = invalidCrosshairSprite;
                    break;
                case TargetType.None:
                default:
                    _crosshair.SetActive(false);
                    return;
            }
            
            _crosshair.SetActive(true);
        }

        private bool IsTooFar(Transform otherTransform)
        {
            return Vector3.Distance(transform.position, otherTransform.position) > grabRadius;
        }

        private bool IsValidTarget(Collider c, Stage stage)
        {
            switch (stage)
            {
                case Stage.None:
                    return c.CompareTag(ropeStartTag);
                case Stage.StartPlaced:
                    return c.CompareTag(ropeEndTag);
                case Stage.Done:
                default:
                    return false;
            }
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
            Debug.Log("OnEnable: " + gameObject.name);
            base.OnEnable();
            triggerAction.action.Enable();
            triggerAction.action.performed += OnTriggerPressed;
        }

        protected override void OnDisable()
        {
            Debug.Log("OnDisable: " + gameObject.name);
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
            Debug.Log("Trigger pressed: " + context.action.name);
            if (!_isHeld) return;
            Shoot();
        }

        private void Shoot()
        {
            Destroy(_lineRenderer);
            _lineRenderer = null;
            
            if (!Physics.Raycast(harpoonTip.transform.position, harpoonTip.transform.forward, out var hit)) return;
            if (!IsValidTarget(hit.collider, _stage)) return;

            switch (_stage)
            {
                case Stage.None:
                    _start = hit.point;
                    _stage = Stage.StartPlaced;
                    break;
                case Stage.StartPlaced:
                    _end = hit.point;
                    _stage = infiniteAmmo ? Stage.None : Stage.Done;
                    PlaceRope();
                    break;
                case Stage.Done:
                default:
                    break;
            }
        }

        private void PlaceRope()
        {
            var rope = Instantiate(ropePrefab);
            rope.GetComponent<RopeBehavior>().ForceUpdate(_start, _end);
        }
}

}