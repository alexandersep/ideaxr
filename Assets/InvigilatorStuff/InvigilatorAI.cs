using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InvigilatorAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround;//, whatIsPlayer;

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
    private float timeSinceLastNoise;

    private bool isRetreating = false;
    private float retreatStartTime;

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
        timeSinceLastNoise = 0f;
    }


    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        this.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (audioDetector == null) Debug.Log("AudioDetector is null");

        float distance = Vector3.Distance(transform.position, player.position);
        float loudness = audioDetector.GetAudioFromMicrophone();

        Debug.Log(loudness);

        if (currentState == EnemyState.PATROL)
        {
            Patroling();
        }

        if (distance <= hearingRadius && loudness > whisperThreshold)
        {
            timeSinceLastNoise = 0f; // Reset timer
            if (loudness > alertThreshold)
            {
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
            agent.SetDestination(new Vector3(player.position.x, player.position.y, player.position.z + 1.5f));
        }
    }

    void BecomeHostile()
    {
        if (currentState != EnemyState.ATTACK)
        {
            Debug.Log("Becoming hostile");
            currentState = EnemyState.ATTACK;
            agent.SetDestination(new Vector3(player.position.x, player.position.y, player.position.z + 1.5f));
        }
    }

    void StartRetreat()
    {
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
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
}
