using UnityEngine;

public class PanNearFireDetector : MonoBehaviour
{
    /// <summary>
    /// Reference to the CookingManager instance managing the cooking process within the scene.
    /// </summary>
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