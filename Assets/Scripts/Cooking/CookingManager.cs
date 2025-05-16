using UnityEngine;
using System.Collections;

public class CookingManager : MonoBehaviour
{
    public GameObject rawSteak;       
    public GameObject cookedSteak;     
    public GameObject pan;             
    public GameObject fireCamp;        
    public AudioSource cookingSound;  
    public Transform steakParent;
    public ParticleSystem smokeEffect;  

    private bool isCooking = false;    
    private float cookingTime = 7f;
    private float FireTime = 1f;


    void Start()
    {
        cookedSteak.SetActive(false);
    }

    void Update()
    {
        if (isCooking && rawSteak.activeSelf)
        {
            StartCooking();
        }
    }

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

        cookingSound.Stop();

    }

    public void PlaceSteakInPan()
    {
        rawSteak.SetActive(true);  
        cookedSteak.SetActive(false);  
        rawSteak.transform.position = steakParent.position; 
        rawSteak.transform.rotation = steakParent.rotation; 
    }
}
