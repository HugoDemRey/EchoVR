using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;
using UnityEngine.XR;

/// <summary>
/// Manages the UI menu for a game, providing functionality to show, hide, and interact
/// with the menu elements such as volume controls and buttons.
/// </summary>
public class MenuManager : MonoBehaviour
{
    /// <summary>
    /// Represents the GameObject menu UI element that can be shown or hidden.
    /// </summary>
    public GameObject menu;

    /// <summary>
    /// Button in the menu that allows the player to continue
    /// by closing the menu and resuming the game.
    /// </summary>
    public Button continueButton;

    /// <summary>
    /// Scrollbar UI component used to control the volume level in the game menu.
    /// </summary>
    public Scrollbar volumeScrollbar;

    /// <summary>
    /// An AudioListener component used to manage and capture the audio environment
    /// within the scene. Provides the ability to control and monitor audio playback
    /// settings, such as enabling or disabling audio listening.
    /// </summary>
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

    /// <summary>
    /// Displays the game menu by setting the menu UI element to active.
    /// Positions and rotates the menu relative to the camera's position and orientation.
    /// </summary>
    public void ShowMenu()
    {
        menu.SetActive(true);

        menu.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
        menu.transform.rotation = Camera.main.transform.rotation;

    }

    /// <summary>
    /// Closes the game menu by deactivating the menu UI element.
    /// Used to resume gameplay or exit the menu interface.
    /// </summary>
    public void CloseMenu()
    {
        menu.SetActive(false);
    }

    /// <summary>
    /// Adjusts the game's volume level based on the value of the volume scrollbar.
    /// </summary>
    public void AdjustVolume()
    {
        float volume = volumeScrollbar.value;
        AudioListener.volume = volume;
    }
}