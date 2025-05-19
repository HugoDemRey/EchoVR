using UnityEngine;

namespace Prefabs.Rope
{
    public class RopeBehavior : MonoBehaviour
    {
        /// <summary>
        /// Thickness of the rope in the scene.
        /// </summary>
        public float width = .015f;

        /// <summary>
        /// Factor that determines the position of the zip line start and end along the rope, in percentage.
        /// </summary>
        public float snapSpherePosition = 0.05f;

        /// <summary>
        /// Starting point of the rope in the scene.
        /// </summary>
        public Vector3 start;

        /// <summary>
        /// End position of the rope in the scene.
        /// </summary>
        public Vector3 end;

        /// <summary>
        /// Material used to render the rope.
        /// </summary>
        public Material material;

        /// <summary>
        /// GameObject used as the cylindrical visual component for the rope in the RopeBehavior class.
        /// </summary>
        public GameObject cylinder;

        /// <summary>
        /// Starting point of the zip line.
        /// </summary>
        public GameObject zipLineStart;

        /// <summary>
        /// End point of the zip line as a GameObject.
        /// </summary>
        public GameObject zipLineEnd;

        /// <summary>
        /// Whether the rope's state has been validated.
        /// </summary>
        private bool _validated;

        /// <summary>
        /// Capsule collider component associated with the rope.
        /// </summary>
        private CapsuleCollider _collider;

        /// <summary>
        /// Initializes the rope's capsule collider.
        /// </summary>
        private void Start()
        {
            _collider = GetComponent<CapsuleCollider>();
        }

        /// <summary>
        /// Makes the rope update its start and end positions to the specified values.
        /// Sets _validated to false to ensure the rope's state is updated.
        /// </summary>
        /// <param name="newStart">The new start position of the rope.</param>
        /// <param name="newEnd">The new end position of the rope.</param>
        public void ForceUpdate(Vector3 newStart, Vector3 newEnd)
        {
            start = newStart;
            end = newEnd;
            _validated = false;
        }

        /// <summary>
        /// Updates the visual representation of the rope by adjusting the scale of the cylinder.
        /// </summary>
        private void UpdateLineVisuals()
        {
            cylinder.transform.localScale = new Vector3(width, Vector3.Distance(start, end) / 2f, width);
        }

        /// <summary>
        /// Updates the properties of the rope's capsule collider, specifically its radius and height.
        /// </summary>
        private void UpdateCollider()
        {
            _collider.radius = width;
            _collider.height = Vector3.Distance(start, end);
        }

        /// <summary>
        /// Updates the transform of the rope to ensure it is correctly placed between the start and end points.
        /// </summary>
        private void UpdateTransform()
        {
            transform.position = (start + end) / 2;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, (end - start).normalized);
        }

        /// <summary>
        /// Updates the interaction points and visual triggers for the zipline based on the start and end positions of the rope.
        /// Adjusts the position and rotation of the zipline triggers dynamically.
        /// </summary>
        private void UpdateInteractions()
        {
            // Zip line
            if (!Mathf.Approximately(start.y, end.y))
            {
                Vector3 startTrigger = (1f - snapSpherePosition) * start + snapSpherePosition * end;
                Vector3 endTrigger = (1f - snapSpherePosition) * end + snapSpherePosition * start;
                zipLineStart.transform.position = start.y > end.y
                    ? startTrigger
                    : endTrigger;
                zipLineEnd.transform.position = start.y > end.y
                    ? endTrigger
                    : startTrigger;
                
                zipLineStart.transform.rotation = Quaternion.Euler(
                    0,
                    transform.rotation.eulerAngles.y,
                    0
                );
            }
            else
            {
                // If the start and end points are the same, disable the zip line triggers
                zipLineStart.SetActive(false);
                zipLineEnd.SetActive(false);
            }
        }

        /// <summary>
        /// Updates all components of the rope.
        /// This method ensures the rope is fully synchronized and consistent with its defined state.
        /// </summary>
        private void UpdateAll()
        {
            UpdateLineVisuals();
            UpdateCollider();
            UpdateTransform();
            UpdateInteractions();
        }


        /// <summary>
        /// Invoked once per frame.
        /// Ensures the rope's properties are updated if their state has not been validated.
        /// </summary>
        private void Update()
        {
            if (_validated) return;
            
            UpdateAll();
            
            _validated = true;
        }

        /// <summary>
        /// Invoked when the script's properties are changed in the Inspector.
        /// Resets the validation state to ensure any modifications are processed appropriately.
        /// </summary>
        private void OnValidate()
        {
            _validated = false;
        }

        /// <summary>
        /// Returns the starting position of the zip line.
        /// </summary>
        /// <returns>The start position of the zip line as a Vector3.</returns>
        public Vector3 GetStartPoint()
        {
            return zipLineStart.transform.position;
        }

        /// <summary>
        /// Returns the position of the endpoint of the zip line.
        /// </summary>
        /// <returns>
        /// A Vector3 representing the position of the zip line endpoint.
        /// </returns>
        public Vector3 GetEndPoint()
        {
            return zipLineEnd.transform.position;
        }
    }
}
