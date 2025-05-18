using UnityEngine;

public class StringAreaDetector : MonoBehaviour
{
    public GuitarSoundController guitarSoundController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            Debug.Log("entre");
            guitarSoundController.OnHandTouch(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            Debug.Log("sortie");
            guitarSoundController.OnHandTouch(false);
        }
    }
}