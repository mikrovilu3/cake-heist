using UnityEngine;
using UnityEngine.InputSystem;

public class InputDetection : MonoBehaviour
{
    public MonoBehaviour mouseLookScript;  // Reference to Mouse Look script
    public MonoBehaviour controllerLookScript;  // Reference to Controller Look script

    private bool usingController = false; // Stores the last used input

    void Update()
    {
        CheckInputMethod();
        UpdateLookScripts();
    }

    void CheckInputMethod()
    {
        if (Gamepad.current != null)
        {
            if (Mathf.Abs(Gamepad.current.rightStick.ReadValue().x) > 0.1f ||
                Mathf.Abs(Gamepad.current.rightStick.ReadValue().y) > 0.1f)
            {
                usingController = true; // Only switch when movement is detected
            }
        }

        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            usingController = false; // Switch only when mouse movement occurs
        }
    }

    void UpdateLookScripts()
    {
        if (usingController)
        {
            controllerLookScript.enabled = true;
            mouseLookScript.enabled = false;
        }
        else
        {
            controllerLookScript.enabled = false;
            mouseLookScript.enabled = true;
        }
    }
}
