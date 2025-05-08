using System;
using Prefabs.Rope;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using InputDevice = UnityEngine.XR.InputDevice;

namespace Prefabs.Harpoon
{
    public class HarpoonBehavior : XRLimitedGrabInteractable
    {
        public InputActionReference triggerAction;
        public GameObject harpoonTip;
        public GameObject ropePrefab;
        public bool infiniteAmmo = true;
        public ArrowSocket arrowSocket;
        
        public Material triggerMaterial;
        
        public Sprite validCrosshairSprite;
        public Sprite invalidCrosshairSprite;
        
        public String ropeStartTag;
        public String ropeEndTag;
        
        public AudioClip startPlacedSound;
        public AudioClip ropePlacedSound;
        public AudioClip validTargetSound;
        public AudioClip invalidTargetSound;
        public AudioClip itemEquippedSound;

        private TargetType _targetType;
        private Stage _stage = Stage.None;
        private Vector3 _start;
        private Vector3 _end;
        
        private SpriteRenderer _renderer;
        private GameObject _crosshair;
        private Camera _mainCamera;
        private LineRenderer _ropePreviewLineRenderer;
        
        private static readonly Vector3 CrosshairScale = new(0.005f, 0.005f, 0.005f);

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
            _stage = infiniteAmmo ? Stage.None : Stage.Done;
            _crosshair = new GameObject("CrossSprite");
            _renderer = _crosshair.AddComponent<SpriteRenderer>();
            _renderer.sortingOrder = 10; // Gives priority
            _crosshair.transform.localScale = CrosshairScale;
            _mainCamera = Camera.main;
            _ropePreviewLineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        protected override void Awake()
        {
            base.Awake();
            selectEntered.AddListener(OnGrab);
        }

        private void Update()
        {
            TargetType nextTargetType = TargetType.None;
            if (isHeld && Physics.Raycast(harpoonTip.transform.position, harpoonTip.transform.forward, out var hit))
            {
                nextTargetType = IsValidTarget(hit.collider, _stage) ? TargetType.Valid : TargetType.Invalid;
                
                _crosshair.transform.position = hit.point + hit.normal * 0.01f;
                _crosshair.transform.LookAt(_mainCamera.transform);
                _crosshair.transform.Rotate(0, 180, 0);
                
                // scale the crosshair to make it visible
                _crosshair.transform.localScale = CrosshairScale * (1 + Vector3.Distance(_mainCamera.transform.position, hit.point) / 2f);
                
                if (_stage == Stage.StartPlaced)
                {
                    _ropePreviewLineRenderer.enabled = true;
                    
                    _ropePreviewLineRenderer.startWidth = 0.01f;
                    _ropePreviewLineRenderer.endWidth = 0.01f;
                    _ropePreviewLineRenderer.positionCount = 2;
                    _ropePreviewLineRenderer.useWorldSpace = true;
                    _ropePreviewLineRenderer.material = triggerMaterial;
                    
                    _ropePreviewLineRenderer.SetPosition(0, _start);
                    _ropePreviewLineRenderer.SetPosition(1, hit.point);
                }
            } else if (_stage == Stage.StartPlaced)
            {
                _ropePreviewLineRenderer.enabled = false;
            }
            
            if (nextTargetType == TargetType.Valid && _targetType != TargetType.Valid)
            {
                PlayValidTargetFeedback();
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
        
        public void Reload()
        {
            _stage = Stage.None;
            _ropePreviewLineRenderer.enabled = false;
            _crosshair.SetActive(false);
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
            PlayItemEquippedFeedback();
        }

        private void OnTriggerPressed(InputAction.CallbackContext context)
        {
            if (!isHeld)
            {
                Debug.LogWarning("Harpoon's OnTriggerPressed called while not held");
                return;
            }
            
            Debug.Log("Harpoon's OnTriggerPressed called");
            Shoot();
        }

        private void Shoot()
        {
            if (
                !Physics.Raycast(harpoonTip.transform.position, harpoonTip.transform.forward, out var hit) 
                || !IsValidTarget(hit.collider, _stage)
                )
            {
                PlayInvalidTargetFeedback();
                Debug.Log("Invalid target");
                return;
            };
            
            _ropePreviewLineRenderer.enabled = false;

            switch (_stage)
            {
                case Stage.None:
                    _start = hit.point;
                    _stage = Stage.StartPlaced;
                    PlayStartPlacedFeedback();
                    Debug.Log("Start placed");
                    break;
                case Stage.StartPlaced:
                    _end = hit.point;
                    _stage = infiniteAmmo ? Stage.None : Stage.Done;
                    PlayRopePlacedFeedback();
                    Debug.Log("End placed");
                    PlaceRope();
                    arrowSocket.DestroyArrow();
                    break;
                case Stage.Done:
                default:
                    Debug.Log("Already placed or no ammo");
                    break;
            }
        }

        private void PlaceRope()
        {
            var rope = Instantiate(ropePrefab);
            rope.GetComponent<RopeBehavior>().ForceUpdate(_start, _end);
        }

        private void PlayStartPlacedFeedback()
        {
            PlaySound(startPlacedSound, transform.position, .5f);
            HapticFeedback(.1f, .5f);
        }
        
        private void PlayRopePlacedFeedback()
        {
            PlaySound(ropePlacedSound, transform.position, .5f);
            HapticFeedback(.5f, .75f);
        }
        
        private void PlayValidTargetFeedback()
        {
            PlaySound(validTargetSound, _crosshair.transform.position, .2f);
            HapticFeedback(.1f, .1f);
        }
        
        private void PlayInvalidTargetFeedback()
        {
            PlaySound(invalidTargetSound, transform.position, .1f);
        }
        
        private void PlayItemEquippedFeedback()
        {
            PlaySound(itemEquippedSound, transform.position, .5f);
        }

        private void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
        {
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }

        private void HapticFeedback(float duration, float amplitude)
        {
            InputDevice leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            
            if (leftHandDevice.TryGetHapticCapabilities(out var leftHapticCapabilities) && leftHapticCapabilities.supportsImpulse)
            {
                leftHandDevice.SendHapticImpulse(0, amplitude, duration);
            }
            if (rightHandDevice.TryGetHapticCapabilities(out var rightHapticCapabilities) && rightHapticCapabilities.supportsImpulse)
            {
                rightHandDevice.SendHapticImpulse(0, amplitude, duration);
            }
        }
    }
}