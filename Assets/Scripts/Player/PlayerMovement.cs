using UnityEngine;
using UnityEngine.InputSystem;

// Handles forward movement, steering, gravity, and movement animation.
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f; // Base forward movement speed.
    [SerializeField] private float steeringRange = 200f; // Horizontal drag needed for full steering.
    [SerializeField] private float turnSmoothSpeed = 540f; // How quickly the character turns.
    [SerializeField] private float maxTurnAngle = 90f; // Maximum visual left/right turn.

    [Header("Forward Bias")]
    [SerializeField] private float minimumForwardFactor = 0.6f; // Keeps the player moving forward while steering hard.

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f; // Downward acceleration.

    private CharacterController characterController;
    private Animator animator;

    private float verticalVelocity; // Current vertical speed from gravity.

    private float targetTurnAngle; // Desired visual facing angle.
    private float currentTurnAngle; // Smoothed visual facing angle.

    private Vector2 pressStartPosition; // Pointer position when the hold begins.
    private bool wasHolding; // Tracks whether the pointer is currently held.

    private void Awake()
    {
        // Cache required components once at startup.
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Gravity works even when the player is stopped.
        ApplyGravity();

        bool isHolding = Pointer.current != null &&
                         Pointer.current.press.isPressed;

        Vector3 movement = Vector3.zero;

        if (isHolding)
        {
            // Update steering from the current drag.
            UpdateSteering();

            // Keep turning smooth instead of snapping instantly.
            currentTurnAngle = Mathf.MoveTowards(
                currentTurnAngle,
                targetTurnAngle,
                turnSmoothSpeed * Time.deltaTime
            );

            // Convert steering into a -1 to +1 value.
            float steeringAmount = currentTurnAngle / maxTurnAngle;

            // Reduce forward speed slightly during hard steering.
            float forwardFactor = Mathf.Lerp(
                1f,
                minimumForwardFactor,
                Mathf.Abs(steeringAmount)
            );

            // Always keep forward movement.
            float forwardMovement = moveSpeed * forwardFactor;

            // Add left/right movement based on steering.
            float sidewaysMovement = moveSpeed * steeringAmount;

            movement.x = sidewaysMovement;
            movement.z = forwardMovement;

            // Keep the character facing the direction of the steering.
            Quaternion targetRotation =
                Quaternion.Euler(0f, currentTurnAngle, 0f);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSmoothSpeed * Time.deltaTime
            );
        }
        else
        {
            // Return steering to center when the player releases.
            targetTurnAngle = 0f;
            currentTurnAngle = Mathf.MoveTowards(
                currentTurnAngle,
                0f,
                turnSmoothSpeed * Time.deltaTime
            );

            wasHolding = false;

            // Gradually return the character to its forward-facing direction.
            Quaternion targetRotation =
                Quaternion.Euler(0f, currentTurnAngle, 0f);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSmoothSpeed * Time.deltaTime
            );
        }

        // Add gravity to vertical movement.
        movement.y = verticalVelocity;

        // Move the player using the CharacterController.
        characterController.Move(movement * Time.deltaTime);

        // Switch between Idle and Run.
        animator.SetBool("IsMoving", isHolding);
    }

    private void UpdateSteering()
    {
        if (Pointer.current == null)
        {
            return;
        }

        Vector2 currentPosition =
            Pointer.current.position.ReadValue();

        // Capture the starting pointer position when the hold begins.
        if (!wasHolding)
        {
            pressStartPosition = currentPosition;
            wasHolding = true;
        }

        // Measure horizontal drag from the starting point.
        float horizontalDrag =
            currentPosition.x - pressStartPosition.x;

        // Convert drag distance to -1 to +1.
        float steeringAmount =
            Mathf.Clamp(horizontalDrag / steeringRange, -1f, 1f);

        // Convert steering amount to the allowed turn angle.
        targetTurnAngle = steeringAmount * maxTurnAngle;
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