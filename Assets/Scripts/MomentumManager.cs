using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using Vector3 = UnityEngine.Vector3;

public class ClimbEndGravityHandler : MonoBehaviour
{
    private ClimbProvider climbProvider;
    private LocomotionState previousPhase;

    private Vector3 lastVelocity;
    public CharacterController characterController;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        climbProvider = GetComponent<ClimbProvider>();

        if (climbProvider == null)
        {
            Debug.LogError("ClimbProvider not found on this GameObject!");
        }

        rb = characterController.GetComponent<Rigidbody>();
        previousPhase = climbProvider.locomotionState;
    }

    void Update() {


        if (climbProvider == null)
            return;

        isGrounded = characterController.isGrounded;


        LocomotionState currentPhase = climbProvider.locomotionState;

        if (rb.isKinematic) rb.isKinematic = false;
        if (isGrounded){
            rb.isKinematic = true;
        }


        // Check if we just transitioned to Done
        if (previousPhase != LocomotionState.Ended && currentPhase == LocomotionState.Ended){
            OnClimbFinished();
        } else {
            lastVelocity = characterController.velocity;
        }

        previousPhase = currentPhase;

    }

     void OnClimbFinished()
    {
        // Your logic here
        Debug.Log("Climb finished! Object velocity: " + lastVelocity);
        rb.AddForce(lastVelocity, ForceMode.VelocityChange);

    }
}

