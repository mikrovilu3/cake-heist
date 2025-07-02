using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerLookScript : MonoBehaviour
{
    public float sensitivity = 100f;
    public Transform playerBody;
    private float xRotation = 0f;

    void Update()
    {
        if (Gamepad.current == null) return; // Avoid errors if no gamepad detected

        Vector2 lookInput = Gamepad.current.rightStick.ReadValue() * sensitivity * Time.deltaTime;
        float joystickX = lookInput.x;
        float joystickY = lookInput.y;

        // Adjust vertical rotation (prevent flipping)
        xRotation -= joystickY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply rotations
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * joystickX);
    }
}
