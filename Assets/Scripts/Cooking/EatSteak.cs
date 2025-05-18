using UnityEngine;
using System.Collections;

public class EatSteak : MonoBehaviour
{
    public AudioSource eatSound;           
    public float detectionDistance = 0.2f;  
    public Transform playerCamera;         

    private bool isSteakEaten = false;    

    void Start()
    {
        if (eatSound == null)
        {
            Debug.LogError("Eat Sound not assigned!");
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;  
        }
    }

    void Update()
    {
        if (!isSteakEaten && Vector3.Distance(transform.position, playerCamera.position) <= detectionDistance)
        {
            Eat();
        }
    }

    void Eat()
    {
        isSteakEaten = true;
        StartCoroutine(PlayAndDisappear());
    }

    IEnumerator PlayAndDisappear()
    {
        eatSound.Play();
        yield return new WaitForSeconds(eatSound.clip.length);
        gameObject.SetActive(false);
    }
}
