using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public float mouseSensitivity = 100f;
    public Transform playerBody;
    float xRotation = 0f;
    public bool isInMenu = false;
    private MenuManager menu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menu = GameObject.Find("Canvases").GetComponent<MenuManager>(); 
    }

    // Update is called once per frame
    void Update()
    {   
        
        if (isInMenu) 
        {
            Cursor.lockState = CursorLockMode.Confined;
        } else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (isInMenu)
            return;

        float MouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= MouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * MouseX);
    }
}
