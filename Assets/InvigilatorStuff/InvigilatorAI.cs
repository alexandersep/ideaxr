using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InvigilatorAI : MonoBehaviour
{
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


    // private void Awake()
    // {
    //     player = GameObject.Find("Player").transform;
    //     agent = GetComponent<NavMeshAgent>();
    //     this.enabled = true;
    // }

    // Update is called once per frame
    void Update()
    {

        if (audioDetector == null) Debug.Log("AudioDetector is null");

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

    IEnumerator IdleBeforeNextPatrol()
    {
        isIdling = true;

        // **Set Idle Animation**
        animator.SetBool("isIdling", true);
        animator.SetBool("isPatrolling", false);

        Debug.Log("Enemy is idling...");
        yield return new WaitForSeconds(idleTime);
        isIdling = false;
        Patroling();
    }


    IEnumerator FacePlayerWhenArrived(bool doAttack)
    {
        // Wait until the enemy reaches the standPoint
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // Rotate towards the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // Prevent vertical tilting
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        float rotationSpeed = 5f; // Adjust for smoothness
        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        if (doAttack)
        {
            // **Set Attack Animation**
            animator.SetTrigger("isAttacking");
        }
        else
        {
            animator.ResetTrigger("isAttacking");
            currentState = EnemyState.INVESTIGATE;
            animator.SetBool("isInvestigating", true);
        }
    }
}