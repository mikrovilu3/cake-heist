using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement2 : MonoBehaviour
{
    [Header("Movement Settings")]
    public CharacterController controller;
    public float speed = 12f;
    public float sprintSpeed = 20f;
    
    [Header("Acceleration Settings")]
    public float acceleration = 8f;
    public float deceleration = 10f;
    
    [Header("Physics")]
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    // Private variables
    private float currentSpeed = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGrounded; // Track previous ground state
    private float landingTimer = 0f; // Timer for landing animation
    public Animator animator;

    void Start()
    {
        // Initialize animator if not assigned
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (controller == null)
        {
            Debug.LogWarning("CharacterController is not assigned to PlayerMovement script.");
            return;
        }

        // Store previous ground state
        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0 && !Input.GetButton("Jump"))
        {
            velocity.y = -2f;
        }

        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Calculate movement direction
        Vector3 move = transform.right * x + transform.forward * z;
        
        // Determine target speed
        float targetSpeed = 0f;
        if (move.magnitude > 0.1f)
        {
            targetSpeed = isSprinting ? sprintSpeed : speed;
        }

        // Smoothly change current speed toward target speed
        if (currentSpeed < targetSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
            if (currentSpeed > targetSpeed)
                currentSpeed = targetSpeed;
        }
        else if (currentSpeed > targetSpeed)
        {
            currentSpeed -= deceleration * Time.deltaTime;
            if (currentSpeed < targetSpeed)
                currentSpeed = targetSpeed;
        }

        // Apply movement
        controller.Move(move.normalized * currentSpeed * Time.deltaTime);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            
            // Set jump animation
            if (animator != null)
            {
                // Reset all states first
                animator.SetBool("hasLanded", false);
                animator.SetBool("isFalling", false);
                // Then set jump
                animator.SetBool("hasJumped", true);
            }
        }

        // Handle animation states
        if (animator != null)
        {
            // Set moving boolean based on current speed
            animator.SetBool("isMoving", currentSpeed > 0.05f);
            
            // Check if falling (moving downward while not grounded)
            bool isFalling = !isGrounded && velocity.y < -0.1f;
            animator.SetBool("isFalling", isFalling);

            // Check if just landed (was not grounded, now grounded)
            if (!wasGrounded && isGrounded)
            {
                // Just landed - set landed state and reset others
                animator.SetBool("hasLanded", true);
                animator.SetBool("isFalling", false);
                animator.SetBool("hasJumped", false);
                landingTimer = 0f; // Reset landing timer
            }
            
            // Handle landing timer and reset
            if (animator.GetBool("hasLanded"))
            {
                landingTimer += Time.deltaTime;
                
                // Reset hasLanded after landing animation has had time to play (adjust timing as needed)
                if (landingTimer > 0.5f && isGrounded && velocity.y <= 0)
                {
                    animator.SetBool("hasLanded", false);
                    landingTimer = 0f;
                }
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}