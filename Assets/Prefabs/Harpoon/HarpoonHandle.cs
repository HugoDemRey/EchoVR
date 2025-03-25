using System;
using UnityEngine;
public class HarpoonHandle : MonoBehaviour
{
    public GameObject harpoon;
    public GameObject harpoonParts;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        // On collision with the barrel trigger, spawn Harpoon and destroy Harpoon parts
        if (other.tag == "HarpoonBarrelTrigger")
        {
            Destroy(harpoonParts);
            Instantiate(harpoon, transform.position, Quaternion.identity);
        }
    }
}
