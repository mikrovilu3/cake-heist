using UnityEngine;

public class TwoDimentionalAnimatorControllerMovement : MonoBehaviour
{
    Animator animator;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;

    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    public float maximumWalkVelocity = 0.5f;
    public float maximumRunVelocity = 2.0f;

    int VelocityZHash;
    int VelocityXHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        VelocityZHash = Animator.StringToHash("Velocity Z");
        VelocityXHash = Animator.StringToHash("Velocity X");
    }

    void Update()
    {
        // Controller-compatible input
        float inputZ = Input.GetAxis("Vertical");   
        float inputX = Input.GetAxis("Horizontal"); 
        bool runPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Fire3");

        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

        ChangeVelocity(inputZ, inputX, currentMaxVelocity);
        LockOrResetVelocity(inputZ, inputX, currentMaxVelocity);

        // Sync velocity values with Animator
        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityXHash, velocityX);
    }

    void ChangeVelocity(float inputZ, float inputX, float currentMaxVelocity)
    {
        // Z axis
        if (inputZ > 0 && velocityZ < currentMaxVelocity)
            velocityZ += Time.deltaTime * acceleration;
        else if (inputZ < 0 && velocityZ > -currentMaxVelocity)
            velocityZ -= Time.deltaTime * acceleration;
        else
            Decelerate(ref velocityZ);

        // X axis
        if (inputX > 0 && velocityX < currentMaxVelocity)
            velocityX += Time.deltaTime * acceleration;
        else if (inputX < 0 && velocityX > -currentMaxVelocity)
            velocityX -= Time.deltaTime * acceleration;
        else
            Decelerate(ref velocityX);
    }

    void Decelerate(ref float velocity)
    {
        if (velocity > 0)
        {
            velocity -= Time.deltaTime * deceleration;
            if (velocity < 0.05f)
                velocity = 0;
        }
        else if (velocity < 0)
        {
            velocity += Time.deltaTime * deceleration;
            if (velocity > -0.05f)
                velocity = 0;
        }
    }

    void LockOrResetVelocity(float inputZ, float inputX, float currentMaxVelocity)
    {
        velocityZ = Mathf.Clamp(velocityZ, -currentMaxVelocity, currentMaxVelocity);
        velocityX = Mathf.Clamp(velocityX, -currentMaxVelocity, currentMaxVelocity);
    }
}