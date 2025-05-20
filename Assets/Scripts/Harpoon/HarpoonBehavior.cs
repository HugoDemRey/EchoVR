using System;
using Prefabs.Rope;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using InputDevice = UnityEngine.XR.InputDevice;

namespace Prefabs.Harpoon
{
    public class HarpoonBehavior : XRLimitedGrabInteractable
    {
        /// <summary>
        /// Input action reference for the trigger functionality associated with the harpoon shooting interaction.
        /// </summary>
        public InputActionReference triggerAction;

        /// <summary>
        /// Tip of the harpoon used for aiming and calculating hit detection.
        /// </summary>
        public GameObject harpoonTip;

        /// <summary>
        /// Prefab used for instantiating the rope when the harpoon interaction is triggered.
        /// </summary>
        public GameObject ropePrefab;

        /// <summary>
        /// Indicates whether the harpoon has infinite ammunition.
        /// </summary>
        public bool infiniteAmmo = true;

        /// <summary>
        /// Arrow socket component of the harpoon used for managing the interaction with arrows.
        /// </summary>
        public ArrowSocket arrowSocket;
        
        /// <summary>
        /// Reference to the preview GameObject indicating where arrows should be placed.
        /// </summary>
        public GameObject arrowPreview;

        /// <summary>
        /// Reference to the particle effect prefab that is instantiated when a successful hit occurs with the harpoon.
        /// </summary>
        public GameObject hitParticlesPrefab;

        /// <summary>
        /// Specifies the material used on the rope preview.
        /// </summary>
        [FormerlySerializedAs("triggerMaterial")] public Material previewMaterial;

        /// <summary>
        /// Sprite displayed on the crosshair when the target is valid.
        /// </summary>
        public Sprite validCrosshairSprite;

        /// <summary>
        /// Sprite displayed on the crosshair when the target is not valid.
        /// </summary>
        public Sprite invalidCrosshairSprite;

        /// <summary>
        /// Sprite image to be displayed when the harpoon is not loaded with an arrow.
        /// </summary>
        public Sprite noAmmoSprite;

        /// <summary>
        /// Tag used to identify valid starting points for rope attachments.
        /// </summary>
        public String ropeStartTag;

        /// <summary>
        /// Specifies the tag used to identify the end point for the rope in the harpoon interaction.
        /// </summary>
        public String ropeEndTag;

        /// <summary>
        /// Defines the audio clip to be played when the harpoon makes its first hit.
        /// </summary>
        public AudioClip startPlacedSound;

        /// <summary>
        /// Audio clip that is played when a rope is successfully placed by the harpoon.
        /// </summary>
        public AudioClip ropePlacedSound;

        /// <summary>
        /// Audio clip to be played when the harpoon aims at a valid target.
        /// </summary>
        public AudioClip validTargetSound;

        /// <summary>
        /// Audio clip played when the harpoon shoots an invalid object or fails to lock onto a target.
        /// </summary>
        public AudioClip invalidTargetSound;

        /// <summary>
        /// Audio clip that plays when the harpoon item is grabbed.
        /// </summary>
        public AudioClip itemEquippedSound;

        /// <summary>
        /// Current type of target detected by the harpoon system.
        /// </summary>
        private TargetType _targetType;

        /// <summary>
        /// Current stage of the harpoon's interaction process.
        /// </summary>
        private Stage _stage = Stage.None;

        /// <summary>
        /// Starting position of the harpoon rope during its placement sequence.
        /// </summary>
        private Vector3 _start;

        /// <summary>
        /// Endpoint position of the harpoon target in world space.
        /// </summary>
        private Vector3 _end;

        /// <summary>
        /// SpriteRenderer component responsible for rendering the crosshair.
        /// </summary>
        private SpriteRenderer _renderer;

        /// <summary>
        /// Crosshair object used for aiming and visual feedback during harpoon interactions.
        /// </summary>
        private GameObject _crosshair;

        /// <summary>
        /// Reference to the main camera used for handling visual interactions and positioning of the crosshair in the harpoon system.
        /// </summary>
        private Camera _mainCamera;

        /// <summary>
        /// LineRenderer component used to visualize a preview of the rope connected to the harpoon.
        /// </summary>
        private LineRenderer _ropePreviewLineRenderer;

        /// <summary>
        /// Scale of the crosshair used for targeting with the harpoon, updated depending on the crosshair sprite.
        /// </summary>
        private Vector3 _crosshairScale = new(0.005f, 0.005f, 0.005f);

        /// <summary>
        /// Represents the various stages of an operation or process related to the harpoon behavior:
        /// None - No operation is in progress. The harpoon is ready to be used.
        /// StartPlaced - The starting point of the rope has been placed, the end point has to be placed.
        /// Done - The operation is complete, and the rope has been placed. The harpoon is not loaded.
        /// </summary>
        private enum Stage
        {
            None,
            StartPlaced,
            Done
        }

        /// <summary>
        /// Represents the state of a target in the context of harpoon interactions.
        /// None - No target is currently aimed at
        /// Invalid - The detected target does not have a valid tag for the current stage of interaction.
        /// Valid - The detected target has a valid tag for the current stage of interaction.
        /// </summary>
        private enum TargetType
        {
            None,
            Invalid, 
            Valid
        }

        /// <summary>
        /// Initializes the harpoon's components and properties at the start of the game.
        /// This includes setting up the stage based on the infiniteAmmo flag, creating
        /// and configuring the crosshair, and initializing the rope preview line renderer.
        /// </summary>
        private void Start()
        {
            _stage = infiniteAmmo ? Stage.None : Stage.Done;
            _crosshair = new GameObject("CrossSprite");
            _renderer = _crosshair.AddComponent<SpriteRenderer>();
            _renderer.sortingOrder = 10; // Gives priority
            _crosshair.transform.localScale = _crosshairScale;
            _mainCamera = Camera.main;
            _ropePreviewLineRenderer = gameObject.AddComponent<LineRenderer>();
            _ropePreviewLineRenderer.startWidth = 0.01f;
            _ropePreviewLineRenderer.endWidth = 0.01f;
            _ropePreviewLineRenderer.positionCount = 2;
            _ropePreviewLineRenderer.useWorldSpace = true;
            _ropePreviewLineRenderer.material = previewMaterial;
        }

        /// <summary>
        /// Called when the HarpoonBehavior object is initialized.
        /// This method overrides the base Awake function to perform additional setup tasks,
        /// setting up the selectEntered with a listener to handle grabbing behavior.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            selectEntered.AddListener(OnGrab);
            
        }

        /// <summary>
        /// Updates the harpoon's behavior within the game loop. This includes:
        /// determining the current target type based on raycasting results,
        /// updating the position, orientation, and scale of the crosshair to provide aiming feedback,
        /// toggling the visibility of the rope preview line renderer if needed,
        /// and managing crosshair appearance and ammo indicators based on the current stage and ammo state.
        /// </summary>
        private void Update()
        {
            // Start with no target
            TargetType nextTargetType = TargetType.None;
            
            // If the harpoon is held and aims at a target
            if (isHeld && Physics.Raycast(harpoonTip.transform.position, harpoonTip.transform.forward, out var hit))
            {
                // update the target type
                nextTargetType = IsValidTarget(hit.collider, _stage) ? TargetType.Valid : TargetType.Invalid;
                
                // update the crosshair 
                _crosshair.transform.position = Vector3.Lerp(harpoonTip.transform.position, hit.point + hit.normal * 0.01f, .95f);
                _crosshair.transform.LookAt(_mainCamera.transform);
                _crosshair.transform.Rotate(0, 180, 0);
                
                // scale the crosshair to make it visible
                _crosshair.transform.localScale = _crosshairScale * (1 + Vector3.Distance(_mainCamera.transform.position, hit.point) / 2f);
                
                // if the start is placed, show the rope preview
                if (_stage == Stage.StartPlaced)
                {
                    _ropePreviewLineRenderer.enabled = true;
                    _ropePreviewLineRenderer.SetPosition(0, _start);
                    _ropePreviewLineRenderer.SetPosition(1, hit.point);
                } 
            } else if (_stage == Stage.StartPlaced)
            {
                // If the rope preview was visible but there is not object to project it to, hide it
                _ropePreviewLineRenderer.enabled = false;
            }
            
            // If the target type changed to valid, play feedback
            if (nextTargetType == TargetType.Valid && _targetType != TargetType.Valid)
            {
                PlayValidTargetFeedback();
            }
            
            _targetType = nextTargetType;
            
            // Select the crosshair sprite
            if (!infiniteAmmo && _stage is Stage.Done)
            {
                _crosshairScale = new Vector3(0.025f, 0.025f, 0.025f);
                _renderer.sprite = noAmmoSprite;
            } else
            {
                _crosshairScale = new Vector3(0.005f, 0.005f, 0.005f);
                _renderer.sprite = _targetType switch
                {
                    TargetType.Valid => validCrosshairSprite,
                    _ => invalidCrosshairSprite
                };
            }
            
            // Show or hide the crosshair based on the target type
            _crosshair.SetActive(_targetType is not TargetType.None);
        }

        /// <summary>
        /// Resets the harpoon's state to its initial configuration. This method disables
        /// the rope preview line renderer and arrow preview, and updates the stage.
        /// </summary>
        public void Reload()
        {
            _stage = Stage.None;
            _ropePreviewLineRenderer.enabled = false;
            arrowPreview.SetActive(false);
        }

        /// <summary>
        /// Determines if a given collider is a valid target based on the current stage of the harpoon interaction.
        /// </summary>
        /// <param name="c">The collider to be checked as a potential target.</param>
        /// <param name="stage">The current stage of the harpoon interaction.</param>
        /// <returns>True if the collider is a valid target for the stage; otherwise, false.</returns>
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

        /// <summary>
        /// Activates and sets up the trigger action when the harpoon becomes enabled.
        /// This includes subscribing to the trigger's performed event.
        /// </summary>
        protected override void OnEnable()
        {
            triggerAction.action.Enable();
            triggerAction.action.performed += OnTriggerPressed;
            base.OnEnable();
        }

        /// <summary>
        /// Disables the trigger action and unsubscribes the OnTriggerPressed event
        /// handler to prevent unintended behavior and memory leaks.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            triggerAction.action.Disable();
            triggerAction.action.performed -= OnTriggerPressed;
        }

        /// <summary>
        /// Called when the harpoon is grabbed by the player. This method extends
        /// the functionality of the base OnGrab method from XRLimitedGrabInteractable
        /// by playing feedback.
        /// </summary>
        /// <param name="args">Event arguments containing details about the grab interaction.</param>
        protected void OnGrab(SelectEnterEventArgs args)
        {
            base.OnGrab(args);
            PlayItemEquippedFeedback();
        }

        /// <summary>
        /// Handles the logic for when the trigger input action is performed while the harpoon is held.
        /// Checks if the harpoon is held before invoking the shooting functionality.
        /// </summary>
        /// <param name="context">The context of the input action event triggered by the player.</param>
        private void OnTriggerPressed(InputAction.CallbackContext context)
        {
            if (!isHeld)
            {
                return;
            }
            
            Shoot();
        }

        /// <summary>
        /// Fires the harpoon, handling its interaction with the environment and updating its state.
        /// Executes a raycast from the harpoon tip to determine if a valid target has been hit,
        /// transitioning between stages of the harpoon's functionality (None, StartPlaced, Done).
        /// Provides feedback based on the current stage and either places the starting point or completes
        /// the harpoon placement depending on the stage and conditions.
        /// </summary>
        private void Shoot()
        {
            // Get the target point and check if it is valid
            if (
                !Physics.Raycast(harpoonTip.transform.position, harpoonTip.transform.forward, out var hit) 
                || !IsValidTarget(hit.collider, _stage)
                )
            {
                PlayInvalidTargetFeedback();
                Debug.Log("Invalid target");
                return;
            };
            
            // Hide the rope preview
            _ropePreviewLineRenderer.enabled = false;

            // Update the states based on current states and raycast results, and play feedback
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
                    arrowPreview.SetActive(true);
                    break;
                case Stage.Done:
                default:
                    Debug.Log("Already placed or no ammo");
                    break;
            }
        }

        /// <summary>
        /// Instantiates a new rope using the configured rope prefab and positions it between
        /// the start and end points.
        /// </summary>
        private void PlaceRope()
        {
            var rope = Instantiate(ropePrefab);
            rope.GetComponent<RopeBehavior>().ForceUpdate(_start, _end);
        }

        /// <summary>
        /// Plays feedback associated with the placement of the rope's starting position.
        /// </summary>
        private void PlayStartPlacedFeedback()
        {
            Instantiate(hitParticlesPrefab, _crosshair.transform);
            PlaySound(startPlacedSound, transform.position, .5f);
            HapticFeedback(.1f, .5f);
        }

        /// <summary>
        /// Plays feedback associated with the placement of the rope's end position.
        /// </summary>
        private void PlayRopePlacedFeedback()
        {
            Instantiate(hitParticlesPrefab, _crosshair.transform);
            PlaySound(ropePlacedSound, transform.position, .5f);
            HapticFeedback(.5f, .75f);
        }

        /// <summary>
        /// Plays feedback when a valid target is detected for the harpoon.
        /// </summary>
        private void PlayValidTargetFeedback()
        {
            PlaySound(validTargetSound, _crosshair.transform.position, .2f);
            HapticFeedback(.1f, .1f);
        }

        /// <summary>
        /// Provides feedback to the user when the harpoon shoots at an invalid object.
        /// </summary>
        private void PlayInvalidTargetFeedback()
        {
            PlaySound(invalidTargetSound, transform.position, .1f);
        }

        /// <summary>
        /// Plays feedback sound when the harpoon is grabbed.
        /// </summary>
        private void PlayItemEquippedFeedback()
        {
            PlaySound(itemEquippedSound, transform.position, .5f);
        }

        /// <summary>
        /// Plays a specified audio clip at a given position with a defined volume.
        /// If the audio clip is null, a warning will be logged and no sound will be played.
        /// </summary>
        /// <param name="clip">The audio clip to be played.</param>
        /// <param name="position">The 3D position at which the sound will be played.</param>
        /// <param name="volume">The volume at which the sound will be played, with a default value of 1.0.</param>
        private void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null)
            {
                Debug.LogWarning("Audio clip is null");
                return;
            }
            
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }

        /// <summary>
        /// Provides haptic feedback to the user's XR controllers through both hand devices.
        /// This method checks if haptic feedback is supported on each device, and if so, sends an impulse
        /// to deliver feedback with specified duration and amplitude.
        /// </summary>
        /// <param name="duration">The length of time the haptic feedback should be applied, in seconds.</param>
        /// <param name="amplitude">The strength of the haptic feedback, ranging from 0 (no feedback) to 1 (maximum intensity).</param>
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