using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Video Referenced https://www.youtube.com/watch?v=PmIPqGqp8UY

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.0f;
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.1f;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.1f;

    [SerializeField] bool lockCursor = true;

    float cameraPitch = 0.0f;
    CharacterController characterController = null;

    // For smoothing mouse and camera movement
    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        // Lock Cursor to middle of the screen and hide it
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouse();
        UpdateKeyboard();
    }

    // Update mouse movements for look controls
    void UpdateMouse()
    {
        // Get data for mouse movement
        Vector2 targetMouseDelta = new Vector2(
            Input.GetAxis("Mouse X"), 
            Input.GetAxis("Mouse Y")
            );

        currentMouseDelta = Vector2.SmoothDamp(
            currentMouseDelta,
            targetMouseDelta,
            ref currentMouseDeltaVelocity,
            mouseSmoothTime);

        // Apply Inverse of the delta to the camera pitch
        // to account for inverted y axis
        cameraPitch -= currentMouseDelta.y * mouseSensitivity;

        // Clamp camera range from -90 to 90 degrees so camera is not upside down
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        // Rotate around right vector by camera pitch
        playerCamera.localEulerAngles = Vector3.right * cameraPitch;

        // Rotate parent object on Y axis (Keeps forward vector in line with camera)
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateKeyboard()
    {
        // Get data for forward and backward movement
        Vector2 targetDir = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
            );

        targetDir.Normalize(); // Normalize the movement vector

        currentDir = Vector2.SmoothDamp(
            currentDir,
            targetDir,
            ref currentDirVelocity,
            moveSmoothTime);

        Vector3 velocity = (
            transform.forward *
            currentDir.y + 
            transform.right *
            currentDir.x) * 
            walkSpeed;

        // Apply velocity to character controller 
        characterController.Move(velocity * Time.deltaTime);


    }
}
