using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLookScript : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float controllerSensitivity = 150f; // Tweak as needed for controller look speed
    public Transform playerBody;
    public bool inMenu = false;

    public Animator animator;

    float xRotation = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Get animator if not assigned
        if (animator == null && playerBody != null)
            animator = playerBody.GetComponent<Animator>();

    }

    void Update()
    {
        // Combine mouse and controller input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        float controllerX = Input.GetAxis("RightJoystickHorizontal") * controllerSensitivity * Time.deltaTime;
        float controllerY = Input.GetAxis("RightJoystickVertical") * controllerSensitivity * Time.deltaTime;

        float totalX = mouseX + controllerX;
        float totalY = mouseY + controllerY;

        // Calculate input direction for turn (-1, 0, or 1)
        float turnInput = 0f;
        if (totalX > 0.01f) turnInput = 1f;
        else if (totalX < -0.01f) turnInput = -1f;

        if (inMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        xRotation -= totalY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * totalX);
    }
}