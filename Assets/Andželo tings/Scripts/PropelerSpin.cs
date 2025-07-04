using UnityEngine;

public class PropellerSpin : MonoBehaviour
{
    [Header("Rotation Settings")]
    public bool rotateX = false;
    public bool rotateY = true;
    public bool rotateZ = false;

    public float spinSpeedX = 90f;
    public float spinSpeedY = 90f;
    public float spinSpeedZ = 90f;

    private float currentRotationX = 0f;
    private float currentRotationY = 0f;
    private float currentRotationZ = 0f;

    void Update()
    {
        Spin();
    }

    public void Spin()
    {
        if (rotateX)
            currentRotationX += spinSpeedX * Time.deltaTime;

        if (rotateY)
            currentRotationY += spinSpeedY * Time.deltaTime;

        if (rotateZ)
            currentRotationZ += spinSpeedZ * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(currentRotationX, currentRotationY, currentRotationZ);
    }
}