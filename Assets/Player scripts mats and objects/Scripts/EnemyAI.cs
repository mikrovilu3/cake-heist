using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private Transform firePoint;

    [Header("Detection")]
    [SerializeField] private LayerMask groundLayer = 1;
    [SerializeField] private LayerMask playerLayer = 1;
    [SerializeField] private float sightRange = 15f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float fieldOfView = 120f;

    [Header("Combat")]
    [SerializeField] private float timeBetweenAttacks = 2f;
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private LayerMask attackLayerMask = -1; // What can be hit by attacks

    [Header("Raycast Attack Settings")]
    [SerializeField] private float maxRaycastDistance = 15f;
    [SerializeField] private bool useLineOfSightCheck = true;

    [Header("Visual Effects")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Patrolling")]
    [SerializeField] private float walkPointRange = 10f;
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float runSpeed = 6f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool showAttackRays = true;

    // Private variables
    private Vector3 walkPoint;
    private bool walkPointSet;
    private bool alreadyAttacked;
    private bool isDead;
    private float lastPlayerSightTime;
    private Vector3 lastKnownPlayerPosition;

    // State management
    private enum AIState { Patrolling, Chasing, Attacking, Investigating, Dead }
    private AIState currentState = AIState.Patrolling;

    // Properties
    public bool IsPlayerInSight { get; private set; }
    public bool IsPlayerInAttackRange { get; private set; }

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        // Try to find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
                playerObj = GameObject.Find("First Person Player 1");

            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError($"Player not found for {gameObject.name}! Make sure player has 'Player' tag or is named 'First Person Player 1'");
        }

        // Get NavMeshAgent if not assigned
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        // Get AudioSource if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Set initial agent speed
        if (agent != null)
            agent.speed = walkSpeed;

        // Setup muzzle flash prefab check
        if (muzzleFlashPrefab == null)
        {
            Debug.LogWarning($"No muzzle flash prefab assigned to {gameObject.name}");
        }
    }

    private void Update()
    {
        if (isDead || player == null) return;

        UpdateDetection();
        UpdateState();
        ExecuteState();

    }

    private void UpdateDetection()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is in range
        bool inSightRange = distanceToPlayer <= sightRange;
        bool inAttackRange = distanceToPlayer <= attackRange;

        // Check line of sight if enabled
        if (useLineOfSightCheck && inSightRange)
        {
            Vector3 rayOrigin = firePoint != null ? firePoint.position : transform.position + Vector3.up * 1.5f;
            Vector3 directionToPlayer = (player.position - rayOrigin).normalized;

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, directionToPlayer, out hit, sightRange, ~playerLayer))
            {
                // If we hit something that's not the player, we don't have line of sight
                if (hit.collider.transform != player)
                {
                    inSightRange = false;
                    inAttackRange = false;
                }
            }
        }

        IsPlayerInSight = inSightRange;
        IsPlayerInAttackRange = inAttackRange;

        if (showDebugLogs && Time.frameCount % 30 == 0) // Log every 30 frames to avoid spam
        {
            Debug.Log($"Distance to player: {distanceToPlayer:F1}, InSight: {IsPlayerInSight}, InAttackRange: {IsPlayerInAttackRange}");
        }

        // Update last known player position
        if (IsPlayerInSight)
        {
            lastPlayerSightTime = Time.time;
            lastKnownPlayerPosition = player.position;
        }
    }

    private void UpdateState()
    {

        if (isDead)
        {
            currentState = AIState.Dead;
            return;
        }

        // Simplified state transitions
        if (IsPlayerInAttackRange)
        {
            currentState = AIState.Attacking;
        }
        else if (IsPlayerInSight)
        {
            currentState = AIState.Chasing;
        }
        else if (Time.time - lastPlayerSightTime < 5f)
        {
            currentState = AIState.Investigating;
        }
        else
        {
            currentState = AIState.Patrolling;
        }
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case AIState.Patrolling:
                Patrol();
                break;
            case AIState.Chasing:
                ChasePlayer();
                break;
            case AIState.Attacking:
                AttackPlayer();
                break;
            case AIState.Investigating:
                Investigate();
                break;
            case AIState.Dead:
                // Do nothing, enemy is dead
                break;
        }
    }

    private void Patrol()
    {
        if (agent == null) return;

        agent.speed = walkSpeed;

        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);

            // Check if we've reached the walk point
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                StartCoroutine(WaitAtWalkPoint());
            }
        }
    }

    private IEnumerator WaitAtWalkPoint()
    {
        walkPointSet = false;
        if (agent != null)
            agent.ResetPath();
        yield return new WaitForSeconds(patrolWaitTime);
    }

    private void SearchWalkPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkPointRange;
        randomDirection += transform.position;
        randomDirection.y = transform.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, walkPointRange, 1))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        if (agent == null || player == null) return;

        agent.speed = runSpeed;
        agent.SetDestination(player.position);

        if (showDebugLogs && Time.frameCount % 60 == 0)
        {
            Debug.Log($"Chasing player at {player.position}");
        }
    }

    private void Investigate()
    {
        if (agent == null) return;

        agent.speed = walkSpeed;
        agent.SetDestination(lastKnownPlayerPosition);

        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            lastPlayerSightTime = 0;
        }
    }

    private void AttackPlayer()
    {
        if (player == null) return;

        // Stop moving
        if (agent != null)
            agent.ResetPath();

        // Face the player
        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        if (showDebugLogs)
        {
            Debug.Log($"ATTACKING! AlreadyAttacked: {alreadyAttacked}");
        }

        if (!alreadyAttacked)
        {
            PerformRaycastAttack();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void PerformRaycastAttack()
    {
        if (player == null)
        {
            Debug.LogError("Cannot attack - no player found!");
            return;
        }

        Vector3 rayOrigin = firePoint != null ? firePoint.position : transform.position + transform.forward + Vector3.up * 1.5f;
        Vector3 rayDirection = (player.position - rayOrigin).normalized;

        if (showDebugLogs)
        {
            Debug.Log($"Performing raycast attack from {rayOrigin} towards {player.position}");
        }

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(rayOrigin, rayDirection, out hit, maxRaycastDistance, attackLayerMask);

        if (showAttackRays)
        {
            // Visual debug ray
            Vector3 endPoint = hitSomething ? hit.point : rayOrigin + rayDirection * maxRaycastDistance;
            Debug.DrawRay(rayOrigin, endPoint - rayOrigin, Color.red, 1f);
        }

        if (hitSomething)
        {
            if (showDebugLogs)
            {
                Debug.Log($"Raycast hit: {hit.collider.name} at distance {hit.distance}");
            }
   
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                }

        }
        else
        {
            if (showDebugLogs)
            {
                Debug.Log("Raycast attack missed - no hit detected");
            }
        }

        // Visual effects
        ShowMuzzleFlash(rayOrigin, rayDirection);

        // Play attack sound
        PlaySound(attackSound);
    }

    private void ShowMuzzleFlash(Vector3 origin, Vector3 direction)
    {
        if (muzzleFlashPrefab != null)
        {
            // Instantiate muzzle flash at fire point
            GameObject flashInstance = Instantiate(muzzleFlashPrefab, origin, Quaternion.LookRotation(direction));

            // Destroy the muzzle flash after specified duration
            Destroy(flashInstance, flashDuration);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        if (showDebugLogs)
        {
            Debug.Log("Attack reset - can attack again");
        }
    }


    // Event handler for when damage is taken
    private void OnDamageTaken(int damageAmount)
    {
        // Play hurt sound when damaged
        PlaySound(hurtSound);

        // Become aware of player when damaged
        if (player != null)
        {
            lastPlayerSightTime = Time.time;
            lastKnownPlayerPosition = player.position;
        }
    }

   
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Gizmos for debugging
    private void OnDrawGizmosSelected()
    {
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Sight range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Current target
        if (player != null)
        {
            Gizmos.color = IsPlayerInAttackRange ? Color.red : (IsPlayerInSight ? Color.yellow : Color.gray);
            Gizmos.DrawLine(transform.position, player.position);
        }

        // Walk point
        if (walkPointSet)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(walkPoint, 1f);
        }

        // Last known player position
        if (lastKnownPlayerPosition != Vector3.zero)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(lastKnownPlayerPosition, Vector3.one);
        }

        // Attack raycast visualization
        if (firePoint != null && showAttackRays)
        {
            Gizmos.color = Color.cyan;
            Vector3 rayOrigin = firePoint.position;
            Vector3 forward = transform.forward;
            Gizmos.DrawRay(rayOrigin, forward * maxRaycastDistance);
        }
       
    }

    // Clean up event subscriptions
    private void OnDestroy()
    {

    }
}