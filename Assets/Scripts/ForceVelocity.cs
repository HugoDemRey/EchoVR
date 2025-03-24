using System;
using UnityEngine;

public class ForceVelocity : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("Starting jump coroutine");
        StartCoroutine(jump());
    }

    private System.Collections.IEnumerator jump()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("Jumping");
        rb.AddForce(Vector3.up * 10, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
