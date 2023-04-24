
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamagable
{
    public bool dummy;

    [Header("Core")]
    [SerializeField] float health;
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float dmgToPlayer = 5f;
    [SerializeField] bool fernkampf;
    [Space]
    [Header("Components")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask whatIsGround, whatIsPlayer;
    [SerializeField] AnimatorOverrideController[] overrideControllers;
    [SerializeField] Transform groundCheck;
    [Space]
    [Header("Idle-State")]
    [SerializeField] float idlePhaseTimer = 2f;

    [Space]
    [Header("Patroling-State")]
    [SerializeField] float sightRange;
    [SerializeField] Transform[] wayPoints;

    [Space]
    [Header("Attack-State")]
    [SerializeField] GameObject projectile;
    [SerializeField] Transform attackPos;
    [SerializeField] float attackRangeNavMesh = 2.1f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float timeBetweenAttacks;
    [Space]
    [Header("Rest")]
    [SerializeField] GameObject[] skins;

    Transform zwischenSpeicherAttackPos;
    Transform target;
    Transform player;

    new CapsuleCollider collider;
    RuntimeAnimatorController cc;

    Animator anim;
    Rigidbody rb;
    HandleAllAudio audio;

    int wayPointIndex = 0;

    bool alreadyAttacked, idle, isRunning, isDead;
    bool playerInSightRange, playerInAttackRange;

    bool deadSequence, chase, playerInAttack, tpMarked, isGrounded;

    public bool teleport;

    private void Awake()
    {

        player = GameObject.Find("Player").transform;

        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider>();
        if (dummy) agent.enabled = false;
    }

    private void Start()
    {
        skins[0].SetActive(false);
        skins[Random.Range(0, skins.Length)].SetActive(true);

        UpdateDestination();
        audio = HandleAllAudio.instance;

        zwischenSpeicherAttackPos = attackPos;
        anim.runtimeAnimatorController = overrideControllers[Random.Range(0, overrideControllers.Length)];
        cc = anim.runtimeAnimatorController;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, .3f);

        // if (isGrounded) agent.enabled = true;
        // else agent.enabled = false;

        if (teleport) return;


        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        if (isDead)
        {
            chase = false;
            collider.enabled = false;
            if (!deadSequence)
            {
                agent.isStopped = true;
                anim.SetTrigger("dead");
                deadSequence = true;
            }
            return;
        }



        if (idle || playerInAttackRange)
        {
            isRunning = false;
        }
        else
        {
            if (!chase)
                isRunning = true;
        }

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRangeNavMesh, whatIsPlayer);
        playerInAttack = Physics.CheckSphere(attackPos.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange && !dummy) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();

        anim.SetBool("isRunning", isRunning);
        anim.SetBool("inChase", chase);
    }

    void HandleMovement(Vector3 destination)
    {
        rb.velocity = destination * movementSpeed;

    }

    private void Patroling()
    {
        if (wayPoints.Length > 0)
        {
            chase = false;
            if (Vector3.Distance(transform.position, wayPoints[wayPointIndex].position) < 1.5 && !idle)
            {
                StartCoroutine(IdlePhase());
            }
        }
    }

    IEnumerator IdlePhase()
    {
        idle = true;
        yield return new WaitForSeconds(idlePhaseTimer);
        iterateWaypointIndex();
        UpdateDestination();
        yield return new WaitForSeconds(1f);
        idle = false;
    }

    void iterateWaypointIndex()
    {

        wayPointIndex++;
        if ((wayPointIndex) == wayPoints.Length) wayPointIndex = 0;


    }

    void UpdateDestination()
    {

        agent.SetDestination(wayPoints[wayPointIndex].position);
        //HandleMovement(wayPoints[wayPointIndex].position);

    }

    private void ChasePlayer()
    {
        if (!isDead)
            chase = true;

        agent.SetDestination(player.position);
        //HandleMovement(player.position);
    }

    private void AttackPlayer()
    {
        chase = false;
        agent.SetDestination(transform.position);
        //HandleMovement(transform.position);

        transform.LookAt(player.position);

        if (!alreadyAttacked)
        {
            if (fernkampf)
            {
                //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                //rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            }
            else
            {
                anim.SetTrigger("attack");
                if (playerInAttack)
                {
                    HandleAllAudio.instance.Play_enemy_hit(transform);
                    player.TryGetComponent<IDamagable>(out IDamagable dmg);
                    dmg.Damage(dmgToPlayer);
                }
            }
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void AllowAttack()
    {
        alreadyAttacked = true;
    }

    void IDamagable.Damage(float dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            HandleAllAudio.instance.Play_enemy_e_die(transform);
            isDead = true;
            Invoke(nameof(DestroyEnemy), 0.5f);
        }
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject, 5f);
    }

    public void ActivateAttackPossibility()
    {
        attackPos = zwischenSpeicherAttackPos;
    }

    public void DeactivateAttackPossibility()
    {
        attackPos = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRangeNavMesh);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    public void MarkTeleport()
    {
        tpMarked = true;
        HandleTPSymbol.instance.rdyTobeGreen = true;
        Invoke(nameof(ResetTeleport), 5f);
    }

    public void ResetTeleport()
    {
        tpMarked = false;
        HandleTPSymbol.instance.rdyTobeGreen = false;
    }

    public bool GetIfRdy()
    {
        return tpMarked;
    }

    public void PlayRunSound()
    {
        audio.Play_enemy_run(this.transform);
    }

    public void PlayHitSound()
    {
        audio.Play_enemy_hit(this.transform);
    }

}
