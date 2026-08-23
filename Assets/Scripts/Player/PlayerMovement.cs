using UnityEngine;
using UnityEngine.InputSystem;

// Handles player movement, gravity, and movement animation state.
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f; // Forward speed while input is held.

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f; // Downward acceleration.

    private CharacterController characterController;
    private Animator animator;

    private float verticalVelocity; // Current vertical speed from gravity.

    private void Awake()
    {
        // Cache required components once at startup.
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Gravity stays active even when the player is stopped.
        ApplyGravity();

        // Check whether the Mac mouse left button is currently held.
        bool isMoving = Mouse.current != null &&
                        Mouse.current.leftButton.isPressed;

        Vector3 movement = Vector3.zero;

        if (isMoving)
        {
            // Move forward while the mouse button is held.
            movement.z = moveSpeed;
        }

        // Add gravity to the vertical movement.
        movement.y = verticalVelocity;

        // Move the player using the CharacterController.
        characterController.Move(movement * Time.deltaTime);

        // Switch between Idle and Run.
        animator.SetBool("IsMoving", isMoving);
    }

    private void ApplyGravity()
    {
        // Keep the player grounded with a small downward force.
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        // Apply downward acceleration.
        verticalVelocity += gravity * Time.deltaTime;
    }
}