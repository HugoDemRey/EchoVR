using UnityEngine;

namespace Prefabs.Rope
{
    public class RopeBehavior : MonoBehaviour
    {
        public float width = .015f;
        public float snapSpherePosition = 0.05f;
        public Vector3 start;
        public Vector3 end;

        public GameObject zipLineStart;
        public GameObject zipLineEnd;
        
        private bool _validated;

        private LineRenderer _lineRenderer;
        private CapsuleCollider _collider;

        private GameObject _handle;
        
        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _collider = GetComponent<CapsuleCollider>();
        }

        public void ForceUpdate(Vector3 newStart, Vector3 newEnd)
        {
            start = newStart;
            end = newEnd;
            _validated = false;
        }

        private void UpdateLineRenderer()
        {
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);   
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
            UpdateLineRenderer();
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

        public void SetHandle(GameObject handle)
        {
            _handle = handle;
        }

        public void StartZipLine(ZipLineHarpoonHandle.ZiplineHandle handle)
        {
            //TODO
        }
    }
}
