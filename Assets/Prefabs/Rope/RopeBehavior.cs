using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Prefabs.Rope
{
    [ExecuteInEditMode]
    public class RopeBehavior : MonoBehaviour
    {
        public float width = .015f;
        public float snapSpherePosition = 0.05f;
        public Vector3 start;
        public Vector3 end;
        public Material ropeMaterial;
        public Material snapTriggerMaterial;

        private bool _validated;
        
        private LineRenderer _lineRenderer;
        private Rigidbody _rigidbody;
        private CapsuleCollider _collider;
        private XRGrabInteractable _grabInteractable;
        
        private GameObject _zipLineStart;
        private GameObject _zipLineEnd;

        public static RopeBehavior Create(Vector3 newStart, Vector3 newEnd, Material material, Material triggerMaterial)
        {
            GameObject go = new GameObject("Harpoon Rope");
            RopeBehavior rb = go.AddComponent<RopeBehavior>();
            rb.Setup(newStart, newEnd, material, triggerMaterial);
            return rb;
        }
        private void Setup(Vector3 newStart, Vector3 newEnd, Material newMaterial, Material newTriggerMaterial)
        {
            start = newStart;
            end = newEnd;
            ropeMaterial = newMaterial;
            snapTriggerMaterial = newTriggerMaterial;
            _validated = false;
        }

        private void UpdateLineRenderer()
        {
            if (_lineRenderer is null)
            {
                _lineRenderer = gameObject.AddComponent<LineRenderer>();
                _lineRenderer.positionCount = 2;
                _lineRenderer.useWorldSpace = true;
            }
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;
            _lineRenderer.material = ropeMaterial;
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);   
        }

        private void UpdateRigidBody()
        {
            if (_rigidbody is null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
                _rigidbody.isKinematic = false;
                _rigidbody.useGravity = true;
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        private void UpdateCollider()
        {
            if (_collider is null)
            {
                _collider = gameObject.AddComponent<CapsuleCollider>();
                _collider.center = Vector3.zero;
            }
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
            if (_grabInteractable is null)
            {
                // TODO use hugo&nico script here
                gameObject.AddComponent<XRGrabInteractable>();
            }
            
            // Zip line
            if (!Mathf.Approximately(start.y, end.y))
            {
                if (_zipLineStart is null)
                {
                    CreateZipLine();
                }
                _zipLineStart!.transform.localScale = Vector3.one * (width * 3);
                _zipLineEnd!.transform.localScale = Vector3.one * (width * 3);
                Vector3 startTrigger = (1f - snapSpherePosition) * start + snapSpherePosition * end;
                Vector3 endTrigger = (1f - snapSpherePosition) * end + snapSpherePosition * start;
                _zipLineStart.transform.position = start.y > end.y
                    ? startTrigger
                    : endTrigger;
                _zipLineEnd.transform.position = start.y > end.y
                    ? endTrigger
                    : startTrigger;
            }
        }

        private void UpdateAll()
        {
            UpdateLineRenderer();
            UpdateRigidBody();
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

        private void CreateZipLine()
        {
            _zipLineStart = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _zipLineStart.transform.parent = transform;
            _zipLineStart.name = "Zip Line Start";
            _zipLineStart.GetComponent<Renderer>().material = snapTriggerMaterial;
            _zipLineStart.GetComponent<Collider>().isTrigger = true;
            
            _zipLineEnd = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _zipLineEnd.transform.parent = transform;
            _zipLineEnd.name = "Zip Line End";
            _zipLineEnd.GetComponent<Renderer>().enabled = true; // TODO set to false before building
        }
    }
}
