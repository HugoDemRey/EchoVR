using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class MenuManager : MonoBehaviour
{
    public GameObject menu; 
    public Button continueButton; 
    public Scrollbar volumeScrollbar;  
    public AudioListener audioListener;  


    void Start()
    {
        continueButton.onClick.AddListener(CloseMenu);


        menu.SetActive(false);
    }

    void Update()
    {
        InputDevice rightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool isMenuPressed) && isMenuPressed)
        {
            ShowMenu();
        }
    }

    public void ShowMenu()
    {
        menu.SetActive(true);

        menu.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
        menu.transform.rotation = Camera.main.transform.rotation;

    }

    public void CloseMenu()
    {
        menu.SetActive(false);
    }

    public void AdjustVolume()
    {
        float volume = volumeScrollbar.value;
        AudioListener.volume = volume;
    }
}