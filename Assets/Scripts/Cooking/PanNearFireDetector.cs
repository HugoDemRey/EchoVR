using UnityEngine;

public class PanNearFireDetector : MonoBehaviour
{
    public CookingManager cookingManager; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pan")) 
        {
            cookingManager.StartCooking();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pan"))  
        {
            
        }
    }
}