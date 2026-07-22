using GLTFast.Schema;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum DogState
{
    Patrolling,
    Alerted,
    Chasing
}



public class DogPatrolBehaviour : MonoBehaviour
{
    public Monster monster;
    DogState state = DogState.Patrolling;

    public Transform player;

    public Transform[] patrolPoints;

    public float waitAtPoint = 1f;

    private NavMeshAgent agent;

    private int patrolIndex;

    //private bool patrolling = true;

    private bool becomingHostile;

    private bool alerted;

    //Animator animator;

    bool waitingForNextPoint;

    [Header("Detection")]
    public float detectionRadius = 6f;
    public float visionAngle = 90f;
    public LayerMask playerLayer;

    bool playerDetected;

    void Start()
    {

        //animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"{name} has no NavMeshAgent!");
            enabled = false;
            return;
        }

        Debug.Log("Agent found!");
        Debug.Log("On NavMesh: " + agent.isOnNavMesh);
        Debug.Log("Stopped: " + agent.isStopped);
        Debug.Log("Patrol point: " + patrolPoints[0].position);

        if (patrolPoints.Length > 0)
        {
            bool success = agent.SetDestination(patrolPoints[0].position);
            Debug.Log("SetDestination: " + success);
        }
    }

    void Update()
    {
        Debug.Log("Current State: " + state);
        if (state != DogState.Patrolling)
            return;
        Debug.Log(
            $"Remaining: {agent.remainingDistance} " +
            $"Velocity: {agent.velocity.magnitude} " +
            $"HasPath: {agent.hasPath}");

        if (!playerDetected)
        {
            Collider[] hits = Physics.OverlapSphere(
                transform.position,
                detectionRadius,
                playerLayer);

            foreach (Collider hit in hits)
            {
                Vector3 dir = hit.transform.position - transform.position;
                dir.y = 0f;

                float angle = Vector3.Angle(transform.forward, dir);

                if (angle <= visionAngle * 0.5f)
                {
                    playerDetected = true;
                    Debug.Log("PLAYER DETECTED");
                    break;
                }

                Debug.Log(hit.name + " Layer: " + LayerMask.LayerToName(hit.gameObject.layer));
            }
        }
        monster.Animator.SetBool("Chasing", agent.velocity.sqrMagnitude > 0.01f);

        if (!waitingForNextPoint && !agent.pathPending && agent.remainingDistance < 0.25f)
        {
            StartCoroutine(NextPatrolPoint());
        }
    }

    IEnumerator NextPatrolPoint()
    {
        waitingForNextPoint = true;

        yield return new WaitForSeconds(waitAtPoint);

        if (state != DogState.Patrolling)
        {
            waitingForNextPoint = false;
            yield break;
        }

        patrolIndex++;

        if (patrolIndex >= patrolPoints.Length)
            patrolIndex = 0;

        agent.SetDestination(patrolPoints[patrolIndex].position);

        waitingForNextPoint = false;
    }

    public void Alert()
    {
        Debug.Log("Alert called. State = " + state);

        if (!playerDetected)
        {
            Debug.Log("Player not detected yet");
            return;
        }

        if (state != DogState.Patrolling)
        {
            Debug.Log("Not patrolling");
            return;
        }

        StartCoroutine(AlertRoutine());
    }

    IEnumerator AlertRoutine()
    {
        Debug.Log("AlertRoutine started");

        //StopAllCoroutines();

        state = DogState.Alerted;

        Debug.Log("State = Alerted");

        agent.isStopped = true;

        float timer = 0f;

        Quaternion start = transform.rotation;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        Quaternion end = Quaternion.LookRotation(dir);

        while (timer < 1f)
        {
            timer += Time.deltaTime;

            transform.rotation =
                Quaternion.Slerp(start, end, timer);

            yield return null;
        }

        Debug.Log("Finished turning");

        yield return new WaitForSeconds(1f);

        Debug.Log("Starting chase");

        state = DogState.Chasing;

        monster.StartChasing(player);
    }

    public void ResumePatrol()
    {
        StartCoroutine(ResumePatrolRoutine());
    }

    IEnumerator ResumePatrolRoutine()
    {
        state = DogState.Patrolling;
        playerDetected = false;
        waitingForNextPoint = false;

        monster.StopChasing();

        agent.isStopped = true;
        agent.ResetPath();

        yield return null;

        agent.Warp(transform.position);

        patrolIndex = FindNearestPatrolPoint();

        agent.isStopped = false;
        agent.SetDestination(patrolPoints[patrolIndex].position);
    }

    int FindNearestPatrolPoint()
    {
        int nearest = 0;

        float best = Mathf.Infinity;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float d = Vector3.Distance(
                transform.position,
                patrolPoints[i].position);

            if (d < best)
            {
                best = d;
                nearest = i;
            }
        }

        return nearest;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}