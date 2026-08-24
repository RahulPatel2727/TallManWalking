// using UnityEngine;

// // Keeps the test camera following the player.
// public class PlayerCameraFollow : MonoBehaviour
// {
//     [SerializeField] private Transform target; // Player transform to follow.
//     [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -5f); // Camera position relative to player.
//     [SerializeField] private float followSpeed = 8f; // How quickly the camera follows.

//     private void LateUpdate()
//     {
//         if (target == null)
//         {
//             return;
//         }

//         // Calculate the camera's desired position behind the player.
//         Vector3 targetPosition = target.position + offset;

//         // Smoothly follow the player after movement is finished for the frame.
//         transform.position = Vector3.Lerp(
//             transform.position,
//             targetPosition,
//             followSpeed * Time.deltaTime
//         );

//         // Keep the camera looking toward the player.
//         transform.LookAt(target);
//     }
// }

using UnityEngine;

// Keeps a stable runner camera with smooth forward follow and subtle side follow.
public class PlayerCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // Player to follow.

    [Header("Camera Position")]
    [SerializeField] private float height = 3f; // Camera height above the player.
    [SerializeField] private float distance = 5f; // Fixed distance behind the player.
    [SerializeField] private float sideFollowAmount = 0.12f; // Small amount of left/right follow.

    [Header("Follow Smoothing")]
    [SerializeField] private float forwardFollowSpeed = 10f; // Smooth forward tracking.
    [SerializeField] private float sideFollowSpeed = 2f; // Very gentle side tracking.

    [Header("Look")]
    [SerializeField] private float lookHeight = 1f; // Height the camera looks toward.

    private float cameraX;

    private void Start()
    {
        if (target == null)
        {
            return;
        }

        // Start the camera directly behind the player.
        cameraX = target.position.x;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Follow forward movement strongly.
        float targetZ = target.position.z - distance;

        float newZ = Mathf.Lerp(
            transform.position.z,
            targetZ,
            forwardFollowSpeed * Time.deltaTime
        );

        // Follow left/right only slightly.
        float targetX = target.position.x * sideFollowAmount;

        cameraX = Mathf.Lerp(
            cameraX,
            targetX,
            sideFollowSpeed * Time.deltaTime
        );

        // Keep camera height fixed.
        float newY = target.position.y + height;

        transform.position = new Vector3(
            cameraX,
            newY,
            newZ
        );

        // Look forward toward the player without rotating the camera sideways.
        Vector3 lookTarget = new Vector3(
            target.position.x * 0.2f,
            target.position.y + lookHeight,
            target.position.z + 3f
        );

        Vector3 direction = lookTarget - transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}