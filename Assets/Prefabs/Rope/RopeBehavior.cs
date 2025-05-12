using UnityEngine;

namespace Prefabs.Rope
{
    public class RopeBehavior : MonoBehaviour
    {
        public float width = .015f;
        public float snapSpherePosition = 0.05f;
        public Vector3 start;
        public Vector3 end;

        public Material material;

        public GameObject cylinder  = null;
        public GameObject zipLineStart;
        public GameObject zipLineEnd;
        
        private bool _validated;

        private CapsuleCollider _collider;

        private GameObject _handle;
        
        private void Start()
        {
            _collider = GetComponent<CapsuleCollider>();
        }

        public void ForceUpdate(Vector3 newStart, Vector3 newEnd)
        {
            start = newStart;
            end = newEnd;
            _validated = false;
        }

        private void UpdateLineVisuals()
        {
            cylinder.transform.localScale = new Vector3(width, Vector3.Distance(start, end) / 2f, width);
            //cylinder.transform.localPosition = Vector3.zero; //Vector3.Lerp(start, end, .5f);
            //cylinder.transform.localRotation = Quaternion.identity; // Quaternion.FromToRotation(Vector3.up, (end - start).normalized);
        }

        private void UpdateCollider()
        {
            _collider.radius = width;
            _collider.height = Vector3.Distance(start, end);
        }

        private void UpdateTransform()
        {
            transform.position = (start + end) / 2;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, (end - start).normalized);
        }

        private void UpdateInteractions()
        {
            // Climbing
            // TODO use hugo&nico script here
            
            // Zip line
            if (!Mathf.Approximately(start.y, end.y))
            {
                zipLineStart!.transform.localScale = Vector3.one * (width * 3);
                zipLineEnd!.transform.localScale = Vector3.one * (width * 3);
                Vector3 startTrigger = (1f - snapSpherePosition) * start + snapSpherePosition * end;
                Vector3 endTrigger = (1f - snapSpherePosition) * end + snapSpherePosition * start;
                zipLineStart.transform.position = start.y > end.y
                    ? startTrigger
                    : endTrigger;
                zipLineEnd.transform.position = start.y > end.y
                    ? endTrigger
                    : startTrigger;
            }
            else
            {
                zipLineStart.SetActive(false);
                zipLineEnd.SetActive(false);
            }
        }

        private void UpdateAll()
        {
            UpdateLineVisuals();
            UpdateCollider();
            UpdateTransform();
            UpdateInteractions();
        }
        

        private void Update()
        {
            if (_validated) return;
            
            UpdateAll();
            
            _validated = true;
        }

        private void OnValidate()
        {
            _validated = false;
        }

        public Vector3 GetStartPoint()
        {
            return zipLineStart.transform.position;
        }

        public Vector3 GetEndPoint()
        {
            return zipLineEnd.transform.position;
        }
    }
}
