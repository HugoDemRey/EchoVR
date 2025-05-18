using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementWithFriction : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float gravity = -9.81f;
    public float friction = 5f; // Friction factor when no input is given

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if grounded
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small value to keep grounded
        }

        // Input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(x, 0, z);
        input = Vector3.ClampMagnitude(input, 1f);

        // Convert to world space direction
        Vector3 move = transform.TransformDirection(input) * moveSpeed;

        // Apply horizontal movement
        velocity.x = move.x;
        velocity.z = move.z;

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply friction manually when grounded and no input
        if (isGrounded && input.magnitude < 0.01f)
        {
            velocity.x = Mathf.Lerp(velocity.x, 0, friction * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, 0, friction * Time.deltaTime);
        }

        // Move the character
        controller.Move(velocity * Time.deltaTime);
    }
}
