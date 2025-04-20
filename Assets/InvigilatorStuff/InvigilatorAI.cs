using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InvigilatorAI : MonoBehaviour
{
    public bool isActive = true; // Flag to enable/disable the AI
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround;//, whatIsPlayer;

    private Animator animator;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // States
    public float sightRange;
    public bool playerInSightRange;

    private AudioDetector audioDetector;
    public float hearingRadius = 10f;
    public float whisperThreshold = 0.05f;
    public float alertThreshold = 0.15f;
    public float resetTime = 5f;
    public float retreatTime = 3f;
    public float retreatSpeed = 3f;
    public float idleTime = 1f;
    private float timeSinceLastNoise;
    public float patrolRadius = 10f;  
    public float maxDistanceFromPlayer = 15f;

    // Investigating
    public Vector3 standPoint = new Vector3(0.06f, 0.0f, 1.162f);

    private bool isRetreating = false;
    private float retreatStartTime;
    private bool isIdling = false;

    // pause functionality 
    private Vector3 savedDestination;
    private EnemyState savedState;
    private bool wasPatrolling;
    private bool wasIdling;
    private Coroutine currentCoroutine;

    public enum EnemyState
    {
        PATROL,
        INVESTIGATE,
        RETREAT,
        ATTACK
    };

    public EnemyState currentState = EnemyState.PATROL;

    void Start()
    {
        audioDetector = player.GetComponent<AudioDetector>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        timeSinceLastNoise = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            // If we just became inactive, save our state
            if (agent.hasPath)
            {
                savedDestination = agent.destination;
                agent.ResetPath();
            }
            return;
        }
        else if (!agent.hasPath && savedDestination != Vector3.zero)
        {
            // Restore path if we were paused
            agent.SetDestination(savedDestination);
            savedDestination = Vector3.zero;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        float loudness = audioDetector.GetAudioFromMicrophone();

        Debug.Log(loudness);

        if (distance <= hearingRadius && loudness > whisperThreshold)
        {
            timeSinceLastNoise = 0f; // Reset timer
            if (loudness > alertThreshold)
            {
                animator.SetBool("isPatrolling", false);
                BecomeHostile();
            }
            else
            {
                InvestigateNoise();
            }
        }
        else
        {
            timeSinceLastNoise += Time.deltaTime;
        }

        // Check if patrol destination is reached
        if (currentState == EnemyState.PATROL && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isIdling)
        {
            StartCoroutine(IdleBeforeNextPatrol());
        }

        // If no noise for resetTime, enter Retreating state
        if (timeSinceLastNoise >= resetTime && currentState != EnemyState.PATROL && !isRetreating)
        {
            StartRetreat();
        }

        // Handle retreating movement
        if (isRetreating)
        {
            float retreatProgress = Time.time - retreatStartTime;
            if (retreatProgress >= retreatTime)
            {
                isRetreating = false;
                currentState = EnemyState.PATROL;
                Patroling();
            }
        }
    }

    void InvestigateNoise()
    {
        if (currentState == EnemyState.PATROL)
        {
            Debug.Log("Investigating noise");
            currentState = EnemyState.INVESTIGATE;
            agent.SetDestination(standPoint);
            StartCoroutine(FacePlayerWhenArrived(false));
        }
    }

    void BecomeHostile()
    {
        if (currentState != EnemyState.ATTACK)
        {
            Debug.Log("Becoming hostile");
            currentState = EnemyState.ATTACK;
            agent.SetDestination(standPoint);
            StartCoroutine(FacePlayerWhenArrived(true));
        }
    }

    void StartRetreat()
    {
        animator.SetBool("isPatrolling", true);
        Debug.Log("Retreating");
        currentState = EnemyState.RETREAT;
        isRetreating = true;
        retreatStartTime = Time.time;

        // slow down as they leave, in case the player makes more noise
        agent.speed = retreatSpeed;
        Vector3 retreatDirection = transform.position - player.position;
        agent.SetDestination(transform.position + retreatDirection.normalized * 5f);
    }

    void Patroling()
    {
        // **Set Patrol Animation**
        animator.SetBool("isPatrolling", true);
        animator.SetBool("isIdling", false);
        if (isRetreating) return;
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
            agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

    }

    void SearchWalkPoint()
    {

        Vector3 potentialWalkPoint;
        int maxAttempts = 10; // Prevents infinite loops if a valid point isn't found

        for (int i = 0; i < maxAttempts; i++)
        {
            // Generate a random point within the patrol radius of the player
            float randomAngle = Random.Range(0f, 360f);
            float randomDistance = Random.Range(2f, patrolRadius);
            float randomX = Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance;
            float randomZ = Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance;

            potentialWalkPoint = new Vector3(player.position.x + randomX, transform.position.y, player.position.z + randomZ);

            // Check if the point is on valid ground and within max distance
            if (Physics.Raycast(potentialWalkPoint, -transform.up, 2f, whatIsGround) &&
                Vector3.Distance(potentialWalkPoint, player.position) <= maxDistanceFromPlayer)
            {
                walkPoint = potentialWalkPoint;
                walkPointSet = true;
                return;
            }
        }

        // If no valid point is found, keep the current walk point (prevents errors)
        walkPointSet = false;
    }

    // Modify all coroutine-starting methods to track the current coroutine
    IEnumerator IdleBeforeNextPatrol()
    {
        if (!isActive) yield break;

        isIdling = true;
        animator.SetBool("isIdling", true);
        animator.SetBool("isPatrolling", false);

        float timer = 0;
        while (timer < idleTime && isActive)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (isActive)
        {
            isIdling = false;
            Patroling();
        }
    }

    IEnumerator FacePlayerWhenArrived(bool doAttack)
    {
        if (!isActive) yield break;

        while ((agent.pathPending || agent.remainingDistance > agent.stoppingDistance) && isActive)
        {
            yield return null;
        }

        if (!isActive) yield break;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        float rotationSpeed = 5f;
        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f && isActive)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        if (isActive)
        {
            if (doAttack)
                animator.SetTrigger("isAttacking");
            else
            {
                animator.ResetTrigger("isAttacking");
                currentState = EnemyState.INVESTIGATE;
                animator.SetBool("isInvestigating", true);
            }
        }
    }

    // Add this to handle inspector changes
    private void OnValidate()
    {
        if (!Application.isPlaying) return;

        if (!isActive)
        {
            // Save current state
            savedState = currentState;
            wasPatrolling = animator.GetBool("isPatrolling");
            wasIdling = animator.GetBool("isIdling");

            // Stop movement
            if (agent != null && agent.enabled)
            {
                agent.isStopped = true;
            }

            // Stop any running coroutines
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }

            // Set to idle animation
            animator.SetBool("isPatrolling", false);
            animator.SetBool("isIdling", true);
        }
        else
        {
            // Restore state
            if (agent != null && agent.enabled)
            {
                agent.isStopped = false;
            }

            // Restore animations
            animator.SetBool("isPatrolling", wasPatrolling);
            animator.SetBool("isIdling", wasIdling);
        }
    }
}