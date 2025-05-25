using UnityEngine;
using System.Collections;

public class CookingManager : MonoBehaviour
{
    /// <summary>
    /// Raw steak that can be placed in the pan for cooking.
    /// </summary>
    public GameObject rawSteak;

    /// <summary>
    /// Cooked steak.
    /// </summary>
    public GameObject cookedSteak;

    /// <summary>
    /// Frying pan used for cooking in the scene.
    /// </summary>
    public GameObject pan;

    /// <summary>
    /// Fire camp GameObject that is utilized to provide heat for cooking.
    /// </summary>
    public GameObject fireCamp;

    /// <summary>
    /// AudioSource component used to play the sound associated with the cooking process.
    /// </summary>
    public AudioSource cookingSound;

    /// <summary>
    /// Parent Transform used to organize steak-related objects within the scene.
    /// </summary>
    public Transform steakParent;

    /// <summary>
    ///Particle system used to simulate smoke effects during the cooking process.
    /// </summary>
    public ParticleSystem smokeEffect;

    /// <summary>
    /// Indicates whether the steak is currently placed in the pan.
    /// </summary>
    public bool isSteakInPan = false;

    /// <summary>
    /// Indicates whether the pan is positioned near the fire source.
    /// </summary>
    public bool panNearFire = false;

    private bool isCooking = false;    
    private float cookingTime = 7f;
    private float FireTime = 1f;


    void Start()
    {
        cookedSteak.SetActive(false);
    }

    void Update()
    {
        if (!isCooking && rawSteak.activeSelf && isSteakInPan && panNearFire)
        {
            StartCooking();
        }
    }

    /// <summary>
    /// Initiates the cooking process if the necessary conditions are met.
    /// Activates audio feedback and begins the cooking coroutine.
    /// </summary>
    /// <remarks>
    /// This method assumes that the 'rawSteak' is active, the steak is placed in the pan,
    /// and the pan is near the fire. It checks if cooking is not already in progress,
    /// then starts the cooking process.
    /// </remarks>
    public void StartCooking()
    {
        if (!isCooking)
        {
            cookingSound.Play();

            isCooking = true;
            StartCoroutine(CookSteak());
        }
    }

    private IEnumerator CookSteak()
    {
        yield return new WaitForSeconds(cookingTime);

        if (smokeEffect != null)
        {
            smokeEffect.transform.position = rawSteak.transform.position;
            smokeEffect.gameObject.SetActive(true);
            smokeEffect.Play();
            yield return new WaitForSeconds(FireTime);
            smokeEffect.Stop();
            smokeEffect.gameObject.SetActive(false);

        }

        rawSteak.SetActive(false);  
        cookedSteak.SetActive(true);  

        cookedSteak.transform.position = rawSteak.transform.position;
        cookedSteak.transform.rotation = rawSteak.transform.rotation;

        cookedSteak.transform.SetParent(rawSteak.transform.parent);

        cookingSound.Stop();

    }
}
