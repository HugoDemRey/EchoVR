using System.Collections.Generic;
using UnityEngine;

namespace Prefabs.Rope
{
    [ExecuteInEditMode]
    public class DynamicRopeBehavior : MonoBehaviour
    {
        public int numberOfBones;
        public float boneLength = .3f;
        public float boneWidth = .1f;
        public float boneMass = 1;
        public Transform start;

        private bool _validated;

        public void Update()
        {
            if (!_validated)
            {
                UpdateRope();
                _validated = true;
            }
        }

        private void OnValidate()
        {
            _validated = false;
        }

        void UpdateRope()
        {
            transform.localScale = Vector3.one;
            transform.SetPositionAndRotation(start.position, start.rotation);
            ClearBones();
            CreateBones();
        }

        void CreateBones()
        {
            GameObject lastBone = null;
            for (int i = 0; i < numberOfBones; i++)
            {
                GameObject newBone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                
                if (Application.isPlaying)
                {
                    Destroy(newBone.GetComponent<Collider>());
                }
                else
                {
                    UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(newBone.GetComponent<Collider>());;
                }
                
                newBone.transform.parent = lastBone is null ? transform : lastBone.transform;
                newBone.transform.localScale = lastBone is null ? new Vector3(boneWidth*2, boneLength/2, boneWidth*2) : Vector3.one;
                newBone.name = "Bone " + i;
                

                Vector3 origin = lastBone is null ? start.position : lastBone.transform.position;
                newBone.transform.SetPositionAndRotation(
                    origin - new Vector3(0, boneLength, 0),
                    newBone.transform.rotation
                );

                if (lastBone is not null)
                {
                    HingeJoint joint = newBone.AddComponent<HingeJoint>();
                    joint.connectedBody = lastBone.GetComponent<Rigidbody>();
                    
                    joint.anchor = new Vector3(0, boneLength / 2f, 0); 
                    joint.axis = new Vector3(1, 0, 0);
                }
                else
                {
                    Rigidbody _rigidbody = newBone.AddComponent<Rigidbody>();
                    _rigidbody.mass = boneMass;
                    _rigidbody.constraints = RigidbodyConstraints.FreezePosition;
                    
                }
                
                lastBone = newBone;
            }
        }

        void ClearBones()
        {
            List<Transform> children = new List<Transform>();
    
            for (int i = 0; i < transform.childCount; i++)
            {
                children.Add(transform.GetChild(i));
            }

            foreach (Transform child in children)
            {
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}
