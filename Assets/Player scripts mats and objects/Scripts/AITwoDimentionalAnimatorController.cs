using UnityEngine;

public class AITwoDimentionalAnimatorControllerMovement : MonoBehaviour
{
    Animator animator;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;

    [Header("Movement Settings")]
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    public float maximumWalkVelocity = 0.5f;
    public float maximumRunVelocity = 2.0f;

    [Header("AI Input (Set These Externally)")]
    [Tooltip("Forward/backward movement: -1 to 1")]
    public float inputZ = 0f;

    [Tooltip("Left/right movement: -1 to 1")]
    public float inputX = 0f;

    [Tooltip("If true, use run speed")]
    public bool runPressed = false;

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
        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

        ChangeVelocity(inputZ, inputX, currentMaxVelocity);
        LockOrResetVelocity(currentMaxVelocity);

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

    void LockOrResetVelocity(float currentMaxVelocity)
    {
        velocityZ = Mathf.Clamp(velocityZ, -currentMaxVelocity, currentMaxVelocity);
        velocityX = Mathf.Clamp(velocityX, -currentMaxVelocity, currentMaxVelocity);
    }
}
