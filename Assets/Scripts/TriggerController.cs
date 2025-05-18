using UnityEngine;
using UnityEngine.InputSystem;

public class TriggerController : MonoBehaviour
{

    public InputActionReference triggerAction;


    private void OnEnable()
    {
        triggerAction.action.performed += PrintTriggerPressed;
    }

    private void PrintTriggerPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger Pressed");
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
