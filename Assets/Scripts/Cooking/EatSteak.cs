using UnityEngine;
using System.Collections;

public class EatSteak : MonoBehaviour
{
    /// <summary>
    /// AudioSource component responsible for playing the sound effect when the player eats an item.
    /// </summary>
    public AudioSource eatSound;

    /// <summary>
    /// Maximum distance within which the steak can be detected by the player.
    /// </summary>
    public float detectionDistance = 0.2f;

    /// <summary>
    /// A reference to the player's camera transform, used for detecting
    /// the player's position in relation to the steak object's position.
    /// </summary>
    public Transform playerCamera;         

    private bool isSteakEaten = false;    

    void Start()
    {
        if (eatSound == null)
        {
            Debug.Log("Eat Sound not assigned!");
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

    /// <summary>
    /// Executes the logic for eating the steak when it is within the detection range.
    /// </summary>
    void Eat()
    {
        isSteakEaten = true;
        StartCoroutine(PlayAndDisappear());
    }

    /// <summary>
    /// Plays the eating sound effect and disables the steak object after the sound finishes playing.
    /// </summary>
    /// <returns>Returns an IEnumerator to facilitate coroutine execution.</returns>
    IEnumerator PlayAndDisappear()
    {
        eatSound.Play();
        yield return new WaitForSeconds(eatSound.clip.length);
        gameObject.SetActive(false);        

    }
}
