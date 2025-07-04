using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
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
    [SerializeField] private LayerMask attackLayerMask = -1;

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
    [SerializeField] public float walkSpeed = 3.5f;
    [SerializeField] public float runSpeed = 6f;
    [SerializeField] public bool useSetTargets = false;

    [Header("Target parameters")]
    [SerializeField] public GameObject[] targets;
    [SerializeField] public float searchRadius = 1;
    [SerializeField] public float semiTargetTime = 1;
    [SerializeField] public int currentTarget = 1;
    [SerializeField] public int patrolChangeTime = 5;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool showAttackRays = true;

    // State
    private Vector3 walkPoint;
    private bool walkPointSet;
    private bool alreadyAttacked;
    private bool isDead;
    private float lastPlayerSightTime;
    private Vector3 lastKnownPlayerPosition;
    private float nextUpdateSecond = 0f;
    private Vector3 randomOfSet;

    private enum AIState { Patrolling, Chasing, Attacking, Investigating, Dead }
    private AIState currentState = AIState.Patrolling;

    public bool IsPlayerInSight { get; private set; }
    public bool IsPlayerInAttackRange { get; private set; }

    private AITwoDimentionalAnimatorControllerMovement animatorMovement;

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player") ??
                                   GameObject.Find("First Person Player 1");

            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError($"Player not found for {gameObject.name}!");
        }

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (agent != null)
            agent.speed = walkSpeed;

        animatorMovement = GetComponent<AITwoDimentionalAnimatorControllerMovement>();

        if (muzzleFlashPrefab == null)
            Debug.LogWarning($"No muzzle flash prefab assigned to {gameObject.name}");
    }

    private void Update()
    {
        if (isDead || player == null) return;

        UpdateDetection();
        UpdateState();
        ExecuteState();
        UpdateAnimatorMovement();
    }

    private void UpdateDetection()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        bool inSightRange = distanceToPlayer <= sightRange;
        bool inAttackRange = distanceToPlayer <= attackRange;

        if (useLineOfSightCheck && inSightRange)
        {
            Vector3 rayOrigin = firePoint ? firePoint.position : transform.position + Vector3.up * 1.5f;
            Vector3 directionToPlayer = (player.position - rayOrigin).normalized;

            if (Physics.Raycast(rayOrigin, directionToPlayer, out RaycastHit hit, sightRange, ~playerLayer))
            {
                if (hit.collider.transform != player)
                {
                    inSightRange = false;
                    inAttackRange = false;
                }
            }
        }

        IsPlayerInSight = inSightRange;
        IsPlayerInAttackRange = inAttackRange;

        if (IsPlayerInSight)
        {
            lastPlayerSightTime = Time.time;
            lastKnownPlayerPosition = player.position;
        }
    }

    private void UpdateState()
    {
        if (isDead) { currentState = AIState.Dead; return; }

        if (IsPlayerInAttackRange)
            currentState = AIState.Attacking;
        else if (IsPlayerInSight)
            currentState = AIState.Chasing;
        else if (Time.time - lastPlayerSightTime < 5f)
            currentState = AIState.Investigating;
        else
            currentState = AIState.Patrolling;
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case AIState.Patrolling: Patrol(); break;
            case AIState.Chasing: ChasePlayer(); break;
            case AIState.Attacking: AttackPlayer(); break;
            case AIState.Investigating: Investigate(); break;
            case AIState.Dead: break;
        }
    }

    private void Patrol()
    {
        agent.speed = walkSpeed;

        if (useSetTargets && targets.Length > 0 && targets[0] != null)
        {
            if (nextUpdateSecond == Mathf.Floor(Time.time))
            {
                nextUpdateSecond = patrolChangeTime + Mathf.Floor(Time.time);
                currentTarget = (currentTarget + 1) % targets.Length;
            }

            agent.destination = targets[currentTarget].transform.position + randomOfSet;
        }
        else
        {
            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet)
            {
                agent.SetDestination(walkPoint);

                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    StartCoroutine(WaitAtWalkPoint());
            }
        }
    }

    private IEnumerator WaitAtWalkPoint()
    {
        walkPointSet = false;
        agent.ResetPath();
        yield return new WaitForSeconds(patrolWaitTime);
    }

    private void SearchWalkPoint()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkPointRange;
        randomDirection += transform.position;
        randomDirection.y = transform.position.y;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, walkPointRange, 1))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.speed = runSpeed;
        agent.SetDestination(player.position);
    }

    private void Investigate()
    {
        agent.speed = walkSpeed;
        agent.SetDestination(lastKnownPlayerPosition);

        if (!agent.pathPending && agent.remainingDistance < 1f)
            lastPlayerSightTime = 0;
    }

    private void AttackPlayer()
    {
        agent.ResetPath();

        Vector3 lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);

        if (!alreadyAttacked)
        {
            PerformRaycastAttack();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void PerformRaycastAttack()
    {
        Vector3 origin = firePoint ? firePoint.position : transform.position + transform.forward + Vector3.up * 1.5f;
        Vector3 direction = (player.position - origin).normalized;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxRaycastDistance, attackLayerMask))
        {
            if (hitEffect != null)
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

        if (showAttackRays)
        {
            Vector3 endPoint = origin + direction * maxRaycastDistance;
            Debug.DrawRay(origin, direction * maxRaycastDistance, Color.red, 1f);
        }

        ShowMuzzleFlash(origin, direction);
        PlaySound(attackSound);
    }

    private void ShowMuzzleFlash(Vector3 origin, Vector3 direction)
    {
        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, origin, Quaternion.LookRotation(direction));
            Destroy(flash, flashDuration);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDamageTaken(int damageAmount)
    {
        PlaySound(hurtSound);
        if (player != null)
        {
            lastPlayerSightTime = Time.time;
            lastKnownPlayerPosition = player.position;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }

    private void UpdateAnimatorMovement()
    {
        if (animatorMovement == null || agent == null) return;

        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);

        float mappedZ = MapVelocity(localVelocity.z, walkSpeed, runSpeed);
        float mappedX = MapVelocity(localVelocity.x, walkSpeed, runSpeed);

        animatorMovement.inputZ = mappedZ;
        animatorMovement.inputX = mappedX;
        animatorMovement.runPressed = currentState == AIState.Chasing;
    }

    private float MapVelocity(float value, float walkMax, float runMax)
    {
        float abs = Mathf.Abs(value);
        float sign = Mathf.Sign(value);

        if (abs < 0.01f) return 0f;

        if (abs <= walkMax)
            return sign * Mathf.Lerp(0f, 0.5f, abs / walkMax);
        else
        {
            float t = Mathf.InverseLerp(walkMax, runMax, abs);
            return sign * Mathf.Lerp(0.5f, 2f, t);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        if (walkPointSet)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(walkPoint, 1f);
        }

        if (lastKnownPlayerPosition != Vector3.zero)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(lastKnownPlayerPosition, Vector3.one);
        }

        if (player != null)
        {
            Gizmos.color = IsPlayerInAttackRange ? Color.red : (IsPlayerInSight ? Color.yellow : Color.gray);
            Gizmos.DrawLine(transform.position, player.position);
        }

        if (firePoint != null && showAttackRays)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(firePoint.position, transform.forward * maxRaycastDistance);
        }
    }
}
