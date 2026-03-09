using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Coil_Head : MonoBehaviour
{
    [Header("Player & Detection")]
    public Transform player;
    public Transform eyes;
    public float viewRadius = 20f;
    public float viewAngle = 110f;
    public LayerMask targetMask = 1 << 6; // Player layer
    public LayerMask obstacleMask = 1 << 3; // Obstacles layer

    [Header("Movement")]
    public float patrolSpeed = 5f;
    public float chaseSpeed = 10f;
    public float wanderRadius = 15f;
    public float patrolWaitTime = 2f;

    [Header("Audio")]
    public AudioClip[] stopSounds;
    public AudioSource walkSound;
    public AudioSource audioSource;

    private Animator anim;
    private NavMeshAgent agent;
    private bool isVisible;
    private bool wasVisibleLastFrame;
    private bool playerInSight;

    private enum State { Patrol, Chase, Stop }
    private State currentState = State.Patrol;
    private Vector3 patrolTarget;
    private bool waitingAtPatrol;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) { Debug.LogError("NavMeshAgent missing!"); return; }
        if (eyes == null) eyes = transform;
        if (player == null) { Debug.LogError("Assign Player Transform!"); return; }

        agent.speed = patrolSpeed;
        agent.angularSpeed = 360f;
        agent.stoppingDistance = 1.5f;
        agent.autoBraking = false;

        SetRandomPatrolTarget();
    }

    void Update()
    {
        if (player == null) return;

        playerInSight = CanSeePlayer();

        if (!isVisible)
        {
            if (playerInSight && currentState != State.Chase) EnterChase();
            else if (!playerInSight && currentState != State.Patrol) EnterPatrol();

            UpdateMovement();
            if (walkSound) walkSound.UnPause();
        }
        else
        {
            if (!wasVisibleLastFrame) EnterStop();
            wasVisibleLastFrame = true;
            if (walkSound) walkSound.Pause();
        }

        wasVisibleLastFrame = isVisible;
    }

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - eyes.position).normalized;
        float dist = Vector3.Distance(player.position, eyes.position);

        if (dist > viewRadius) return false;
        if (Vector3.Angle(eyes.forward, dirToPlayer) > viewAngle * 0.5f) return false;

        RaycastHit hit;
        if (Physics.Raycast(eyes.position, dirToPlayer, out hit, viewRadius, obstacleMask | targetMask))
            return hit.collider.transform == player;

        return false;
    }

    void EnterChase() { currentState = State.Chase; agent.speed = chaseSpeed; }
    void EnterPatrol() { currentState = State.Patrol; agent.speed = patrolSpeed; }
    void EnterStop()
    {
        currentState = State.Stop;
        agent.ResetPath();
        agent.velocity = Vector3.zero;  // ← КЛЮЧЕВОЕ: мгновенная остановка импульса
        agent.isStopped = true;
        
        // Анимация и звук
        anim.Play("Coil_Head_Head", 0, 0f);
        if (stopSounds.Length > 0 && audioSource)
        {
            AudioClip clip = stopSounds[Random.Range(0, stopSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
        
        StartCoroutine(LookAtPlayer());
    }

    void UpdateMovement()
    {
        if (currentState == State.Stop) return;  // ← НЕ трогай, если остановлен (держит stop)

        if (agent.isStopped) agent.isStopped = false;

        if (currentState == State.Chase)
            agent.SetDestination(player.position);
        else if (currentState == State.Patrol)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f && !waitingAtPatrol)
                StartCoroutine(WaitAtPatrol());
        }
    }

    void SetRandomPatrolTarget()
    {
        Vector3 point = transform.position + Random.insideUnitSphere * wanderRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point, out hit, wanderRadius, -1))
        {
            patrolTarget = hit.position;
            agent.SetDestination(patrolTarget);
        }
    }

    IEnumerator WaitAtPatrol()
    {
        waitingAtPatrol = true;
        yield return new WaitForSeconds(patrolWaitTime);
        SetRandomPatrolTarget();
        waitingAtPatrol = false;
    }

    IEnumerator LookAtPlayer()
    {
        float elapsed = 0f;
        while (elapsed < 2f && isVisible)
        {
            Vector3 dir = (player.position - transform.position).normalized; dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), elapsed / 2f);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void OnBecameVisible() { isVisible = true; }
    void OnBecameInvisible() { isVisible = false; }
}