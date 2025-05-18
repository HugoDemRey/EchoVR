using UnityEngine;

public class PanNearFireDetector : MonoBehaviour
{
    public CookingManager cookingManager; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pan")) 
        {
            cookingManager.panNearFire = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pan"))  
        {
            cookingManager.panNearFire = false;
        }
    }
}