using Essentials.Saving;
using MessagePack;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyBase : MonoBehaviour, IDamageable, IHasHealthbar, ISaveable
{
    public float Health => m_Healthpoint;

    private EnemyHealthBar healthbar;
    public void GiveHealthBar(EnemyHealthBar healthbar)
    {
        this.healthbar = healthbar;
    }

    private Transform player;
    public Transform Waypoint;
    private SAMI_PlayerStats m_PlayerStats;
    public Collider EnemyCollider;
    private Collider[] patrolOverlapSphereResults = new Collider[5];

    public int m_ExpToGive;
    private NavMeshAgent agent;
    private Animator animator;
    protected AudioSource audiosource;
    public AudioClip enemyHitSound;
    public AudioClip enemyHitSoundTwo;
    public AudioClip enemyFootStep;
    public AudioClip enemyKickedSound;
    public AudioClip enemyDeathSound;
    public AudioClip fireBallSound;
    [SerializeField] private Collider m_EnemySword = default;

    public bool inSafeHouse = false;
    public bool isCaught = false;

    public bool isHit = false;
    public bool isDead = false;
    public bool isWaiting = false;
    public bool playerHasAttacked = false;
    public bool isAttacking = false;
    public bool firstHitSound = false;
    public bool secondHitSound = false;
    public bool hasPlayed = false;

    private bool playerIsInSight;
    private bool isRunningPatrol;
    private bool m_IsStunned;

    public float m_Healthpoint = 30f;
    public PlayerAttack playerAttack;
    public GameObject m_HealthPotion;
    public float m_DropChance = 0.5f;
    public SAMI_EnemyDamage m_Damage;
    private ShootFireBall shootFireBall;

    private Vector3 startingPosition;
    private Vector3 currentPatrolPosition;
    private NavMeshPath currentPath;

    [SerializeField]
    private float lookAtSpeed = 5;
    [SerializeField]
    private float attackStateSpeed = 0;
    [SerializeField]
    private float chaseStateSpeed = 2;
    [SerializeField]
    private float patrolStateSpeed = 1;
    [SerializeField]
    private float patrolWanderDistance = 5;
    [SerializeField]
    private float patrolNavMeshCheckDistance = 10;
    [SerializeField]
    private float maxPatrolWanderDistance = 12.5f;
    [SerializeField]
    private Vector2 patrolIdleTimeRange = new Vector2(2, 5);
    [SerializeField]
    private int patrolIdlePercentChance = 35;

    private Rigidbody[] rigidbodies;
    private Collider[] colliders;

    protected virtual void Awake()
    {
        SaveSystem.Register(this);

        startingPosition = transform.position;
        currentPatrolPosition = startingPosition;
        rigidbodies = GetComponentsInChildren<Rigidbody>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
        shootFireBall = GetComponentInChildren<ShootFireBall>();
    }

    #region Saving
    public SaveData GetSave()
    {
        return new EnemyData(gameObject.name)
        {
            isDead = isDead,
            health = m_Healthpoint,
            healthbarValue = healthbar.Slider.value,
            expToGive = m_ExpToGive,
            startingPosition = startingPosition,
            healthbarIsActive = healthbar.Slider.gameObject.activeSelf
        };
    }

    public void Load(SaveData saveData)
    {
        if (saveData is EnemyData save)
        {
            isDead = save.isDead;
            if (!isDead)
            {
                enabled = true;
                agent.enabled = true;

                m_Healthpoint = save.health;
                healthbar.Slider.value = save.healthbarValue;
                m_ExpToGive = save.expToGive;
                startingPosition = save.startingPosition;
                if (save.healthbarIsActive)
                {
                    healthbar.Activate();
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    [Serializable, MessagePackObject]
    public class EnemyData : SaveData
    {
        [Key("IsDead")]
        public bool isDead;
        [Key("Health")]
        public float health;
        [Key("HealthbarValue")]
        public float healthbarValue;
        [Key("ExpGiveAmount")]
        public int expToGive;
        [Key("StartingPosition")]
        public Vector3 startingPosition;
        [Key("HealthbarActive")]
        public bool healthbarIsActive;

        public EnemyData() { }

        public EnemyData(string objName)
        {
            this.objName = objName;
        }
    }
    #endregion

    private void Start()
    {
        player = EnemyGlobal.Instance.Player;
        playerAttack = EnemyGlobal.Instance.PlayerAttack;
        m_Damage = EnemyGlobal.Instance.EnemyDamage;
        m_PlayerStats = EnemyGlobal.Instance.PlayerStats;
    }

    private void Update()
    {
        if (isDead)
        {
            // Do nothing intentionally
        }
        else if (m_Healthpoint <= 0)
        {
            DeathEvent();
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            UpdateState(distanceToPlayer, EnemyGlobal.Instance.CurrentMinDistanceForAttackRanged);
            healthbar.UpdateValue();
        }
    }

    protected virtual void DeathEvent()
    {
        if (Random.Range(0f, 1f) <= m_DropChance)
        {
            Instantiate(m_HealthPotion, transform.position, transform.rotation);
        }

        EnemyAttackColliderDisable();
        healthbar.Deactivate();
        m_PlayerStats.AddExperience(m_ExpToGive);
        StartCoroutine(EnableRagdoll(false));
        StartCoroutine(EnemyDeath());
        SkeletonKillAchievement();
    }

    private static void SkeletonKillAchievement()
    {
        StatManager.Update(Stat.ST_SKELETON_KILLS, 1);

        // 10 kill achievement
        Achievement skelAch10 = Achievements.Get("SKELETON_SLAYER_10");
        if (StatManager.Get(skelAch10.AssociatedStat) >= skelAch10.StatThreshold
            && !AchievementManager.IsAchieved(skelAch10).GetValueOrDefault())
        {
            AchievementManager.Activate(skelAch10);
        }

        // 20 kill achievement
        Achievement skelAch20 = Achievements.Get("SKELETON_DESTROYER_20");
        if (StatManager.Get(skelAch20.AssociatedStat) >= skelAch20.StatThreshold
            && !AchievementManager.IsAchieved(skelAch20).GetValueOrDefault())
        {
            AchievementManager.Activate(skelAch20);
        }
    }

    #region Ragdoll Methods
    private IEnumerator EnableRagdoll(bool isKinematic)
    {
        animator.enabled = false;
        Collider[] playerColliders = EnemyGlobal.Instance.GetPlayerColliders();
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            int colliderLoopOffset = i + 1; // Offset collider by 1 to ignore the main collider
            ActivateColliders(playerColliders, colliderLoopOffset, true);
            colliders[colliderLoopOffset].enabled = true;

            rigidbodies[i].isKinematic = isKinematic;
        }

        yield break;
    }

    private IEnumerator DisableRagdoll()
    {
        animator.enabled = true;
        Collider[] playerColliders = EnemyGlobal.Instance.GetPlayerColliders();
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            int colliderLoopOffset = i + 1; // Offset collider by 1 to ignore the main collider
            ActivateColliders(playerColliders, colliderLoopOffset, false);
            colliders[colliderLoopOffset].enabled = false;

            rigidbodies[i].isKinematic = true;
        }

        colliders[0].enabled = true;

        yield break;
    }

    private void ActivateColliders(Collider[] playerColliders, int i, bool activate)
    {
        for (int j = 0; j < playerColliders.Length; j++)
        {
            Physics.IgnoreCollision(colliders[i], playerColliders[j], activate);
        }
    }
    #endregion

    protected virtual void UpdateState(float distanceToPlayer, float minDistanceForAttack)
    {
        if (m_IsStunned) return;

        if (isAttacking)
        {
            EnemyGlobal.Instance.SmoothLookAtPlayer(transform, lookAtSpeed, false);
            return;
        }

        SetAnimBasedOnVelocity();

        if (distanceToPlayer <= minDistanceForAttack)
        {
            playerIsInSight = EnemyGlobal.Instance.PlayerIsInLineOfSightOf(transform);
            if (playerIsInSight || isHit)
            {
                StopCoroutine(nameof(ChasePlayer));
                StopCoroutine(nameof(StartPatrolling));

                StartCoroutine(AttackingPlayer(distanceToPlayer));
                StartCoroutine(SpecialAttack());
            }
        }
        else if (distanceToPlayer <= EnemyGlobal.Instance.CurrentMinDistanceForChase)
        {
            playerIsInSight = EnemyGlobal.Instance.PlayerIsInLineOfSightOf(transform);
            if (playerIsInSight || isHit)
            {
                StopCoroutine(nameof(AttackingPlayer));
                StopCoroutine(nameof(SpecialAttack));
                StopCoroutine(nameof(StartPatrolling));

                StartCoroutine(ChasePlayer());
            }
        }
        else if (distanceToPlayer > EnemyGlobal.Instance.CurrentMinDistanceForChase
            && !isRunningPatrol)
        {
            StopCoroutine(nameof(AttackingPlayer));
            StopCoroutine(nameof(SpecialAttack));
            StopCoroutine(nameof(ChasePlayer));

            StartCoroutine(StartPatrolling());
        }
    }

    private void SetAnimBasedOnVelocity()
    {
        if (agent.velocity.sqrMagnitude > 0.4f)
        {
            animator.SetBool("Walk", true);
            animator.SetBool("Idle", false);
        }
        else
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
        }
    }

    private IEnumerator AttackingPlayer(float distanceToPlayer)
    {
        if (!m_IsStunned)
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }

        isWaiting = false;
        isRunningPatrol = false;

        if (distanceToPlayer >= 1.6f)
        {
            agent.speed = attackStateSpeed;
        }
        else
        {
            agent.speed = 0;
        }

        animator.SetBool("Attack", true);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Kicked", false);
        animator.SetBool("isHit", false);

        yield return new WaitForSeconds(2.3f);
        isAttacking = false;
    }

    protected virtual IEnumerator SpecialAttack()
    {
        yield break;
    }

    private IEnumerator ChasePlayer()
    {
        playerHasAttacked = true;
        isWaiting = false;
        isRunningPatrol = false;

        agent.speed = chaseStateSpeed;
        NavMesh.SamplePosition(player.position, out NavMeshHit hit, 2, NavMesh.AllAreas);
        Vector3 destination = hit.position;
        if (!gameObject.name.Contains("MINIBOSS"))
        {
            Vector3 offset = (player.position - transform.position).normalized / 1.5f;
            destination += offset;
        }

        TrySetDestination(destination);

        animator.SetBool("Walk", true);
        animator.SetBool("Idle", false);
        animator.SetBool("isHit", false);
        animator.SetBool("Kicked", false);
        animator.SetBool("Attack", false);
        yield return null;
    }

    private IEnumerator StartPatrolling()
    {
        isRunningPatrol = true;
        int random = Random.Range(0, 100);
        if (random <= patrolIdlePercentChance) // Idle
        {
            float timeThreshold = Time.realtimeSinceStartup
                + Random.Range(patrolIdleTimeRange.x, patrolIdleTimeRange.y);
            while (!playerIsInSight
                   && Time.realtimeSinceStartup < timeThreshold)
            {
                yield return null;
            }
        }
        else // Patrol
        {
            if (Vector3.Distance(transform.position, currentPatrolPosition) < 0.5f)
            {
                GetNewPatrolPosition();
            }

            TrySetDestination(currentPatrolPosition);
            while (!playerIsInSight
                   && currentPath.status != NavMeshPathStatus.PathComplete)
            {
                yield return null;
            }
        }

        isRunningPatrol = false;
    }

    private void GetNewPatrolPosition()
    {
        Vector2 randomPoint = Random.insideUnitCircle * patrolWanderDistance;
        Vector3 randomVectorInRadius = new Vector3(transform.position.x + randomPoint.x,
                                                   transform.position.y,
                                                   transform.position.z + randomPoint.y);

        bool hasNavMeshPosition = NavMesh.SamplePosition(randomVectorInRadius, out NavMeshHit hit, patrolNavMeshCheckDistance, NavMesh.AllAreas);
        if (Physics.OverlapSphereNonAlloc(hit.position + Vector3.up, 1, patrolOverlapSphereResults) > 0)
        {
            for (int i = 0; i < patrolOverlapSphereResults.Length; i++)
            {
                if (patrolOverlapSphereResults[i] != null
                    && patrolOverlapSphereResults[i].gameObject.CompareTag("Wall"))
                {
                    SetPatrolStateTo(startingPosition);
                    return;
                }
            }
        }

        float distanceFromOrigin = Vector3.Distance(startingPosition, hit.position);
        if (!hasNavMeshPosition
            || distanceFromOrigin > maxPatrolWanderDistance
            || hit.position == Vector3.positiveInfinity
            || hit.position == Vector3.negativeInfinity
            || hit.position == Vector3.zero
            || Mathf.Abs(hit.position.sqrMagnitude) < 0.1f)
        {
            SetPatrolStateTo(startingPosition);
            return;
        }

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            SetPatrolStateTo(hit.position);
        }
    }

    private void SetPatrolStateTo(Vector3 patrolPos)
    {
        agent.speed = patrolStateSpeed;
        currentPatrolPosition = patrolPos;
    }

    private void TrySetDestination(Vector3 goal)
    {
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.SetDestination(goal);
            currentPath = agent.path;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword") && !isHit)
        {
            StartCoroutine(GetHit());
        }
        else if (other.gameObject.CompareTag("Foot"))
        {
            StartCoroutine(GetKicked());
        }
        else if (other.gameObject.CompareTag("Fist"))
        {
            StartCoroutine(nameof(GetPunched));
        }
        else if (other.gameObject.CompareTag("Throwable")
            && !PlayerAttack.IsCarryingThrowable
            && Viljo_Actions.CanDoDamage)
        {
            StartCoroutine(nameof(GetHitByThrowable));
        }
        else if (other.gameObject.CompareTag("RogueStun"))
        {
            StartCoroutine(nameof(Stunned));
        }
        else if (other.gameObject.CompareTag("RogueAssasination"))
        {
            Vector3 playerForward = other.GetComponentInParent<PlayerAttack>().transform.forward;
            Vector3 enemyForward = transform.forward;
            if (Vector3.Dot(enemyForward, playerForward) >= 0.5)
            {
                StartCoroutine(EnemyDeathByAbility());
            }
            else
            {
                StartCoroutine(GetAssasinationFront());
            }
        }
    }

    public IEnumerator GetHit()
    {
        Hit();
        if (!WarriorAbility.IsUsingCombo)
        {
            yield return new WaitForSeconds(0.65f);
        }

        hasPlayed = false;
        isHit = false;
    }

    public void GetHitNoSlow()
    {
        Hit();
        hasPlayed = false;
        isHit = false;
    }

    private void Hit()
    {
        healthbar.Activate();
        if (!hasPlayed && !firstHitSound && m_Healthpoint > 0)
        {
            audiosource.pitch = Random.Range(0.8f, 1f);
            audiosource.PlayOneShot(enemyHitSound, 0.55f);
            firstHitSound = true;
            secondHitSound = false;
        }
        else if (!hasPlayed && !secondHitSound && m_Healthpoint > 0)
        {
            audiosource.pitch = Random.Range(0.8f, 1f);
            audiosource.PlayOneShot(enemyHitSoundTwo, 0.55f);
            firstHitSound = false;
            secondHitSound = true;
        }

        hasPlayed = true;
        TakeDamage(playerAttack.m_DamageDone);
        playerHasAttacked = true;
        isHit = true;
        agent.speed = 0f;
        animator.SetBool("isHit", true);
        animator.SetBool("Kicked", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Attack", false);
        isRunningPatrol = false;
    }

    private IEnumerator GetPunched()
    {
        animator.speed = 1.3f;
        healthbar.Activate();
        if (!hasPlayed && !firstHitSound && m_Healthpoint > 0)
        {
            audiosource.pitch = Random.Range(0.8f, 1f);
            audiosource.PlayOneShot(enemyHitSound, 0.55f);
            firstHitSound = true;
            secondHitSound = false;
        }
        else if (!hasPlayed && !secondHitSound && m_Healthpoint > 0)
        {
            audiosource.pitch = Random.Range(0.8f, 1f);
            audiosource.PlayOneShot(enemyHitSoundTwo, 0.55f);
            firstHitSound = false;
            secondHitSound = true;
        }

        hasPlayed = true;
        TakeDamage(playerAttack.m_DamageDone / 3.5f);
        playerHasAttacked = true;
        isHit = true;
        isRunningPatrol = false;
        agent.speed = 0f;
        animator.SetBool("isHit", true);
        animator.SetBool("Kicked", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Attack", false);
        yield return new WaitForSeconds(0.425f);
        animator.speed = 1;
        hasPlayed = false;
        isHit = false;
    }

    private IEnumerator Stunned()
    {
        animator.speed = 1.3f;
        healthbar.Activate();
        if (!hasPlayed && !firstHitSound && m_Healthpoint > 0)
        {
            audiosource.pitch = Random.Range(0.8f, 1f);
            audiosource.PlayOneShot(enemyHitSound, 0.55f);
            firstHitSound = true;
            secondHitSound = false;
        }
        else if (!hasPlayed && !secondHitSound && m_Healthpoint > 0)
        {
            audiosource.pitch = Random.Range(0.8f, 1f);
            audiosource.PlayOneShot(enemyHitSoundTwo, 0.55f);
            firstHitSound = false;
            secondHitSound = true;
        }

        hasPlayed = true;
        TakeDamage(playerAttack.m_DamageDone / 3.5f);
        playerHasAttacked = true;
        isHit = true;
        isRunningPatrol = false;
        agent.speed = 0f;
        m_IsStunned = true;
        animator.SetBool("Stunned", true);
        animator.SetBool("isHit", false);
        animator.SetBool("Kicked", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Attack", false);
        animator.SetBool("GetUp", false);
        yield return new WaitForSeconds(2.13f);
        animator.SetBool("Stunned", false);
        animator.SetBool("isHit", false);
        animator.SetBool("Kicked", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Attack", false);
        animator.SetBool("GetUp", true);
        yield return new WaitForSeconds(1.6f);
        m_IsStunned = false;
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < 0.3f)
        {
            StartCoroutine(AttackingPlayer(distance));
        }
        else
        {
            StartCoroutine(ChasePlayer());
        }

        animator.speed = 1;
        hasPlayed = false;
        isHit = false;
    }
    private IEnumerator GetAssasinationFront()
    {
        animator.speed = 1.3f;
        healthbar.Activate();
        if (!hasPlayed && !firstHitSound && m_Healthpoint > 0)
        {
            audiosource.pitch = Random.Range(0.8f, 1f);
            audiosource.PlayOneShot(enemyHitSound, 0.55f);
            firstHitSound = true;
            secondHitSound = false;
        }
        else if (!hasPlayed && !secondHitSound && m_Healthpoint > 0)
        {
            audiosource.pitch = Random.Range(0.8f, 1f);
            audiosource.PlayOneShot(enemyHitSoundTwo, 0.55f);
            firstHitSound = false;
            secondHitSound = true;
        }

        hasPlayed = true;
        TakeDamage(playerAttack.m_DamageDone + 10);
        playerHasAttacked = true;
        isHit = true;
        agent.speed = 0f;
        isRunningPatrol = false;
        animator.SetBool("isHit", true);
        animator.SetBool("Kicked", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Attack", false);
        yield return new WaitForSeconds(0.425f);
        animator.speed = 1;
        hasPlayed = false;
        isHit = false;
    }
    private IEnumerator GetHitByThrowable()
    {
        animator.speed = 1.3f;
        healthbar.Activate();
        if (!hasPlayed && !firstHitSound && m_Healthpoint > 0)
        {
            audiosource.pitch = Random.Range(0.8f, 1f);
            audiosource.PlayOneShot(enemyHitSound, 0.55f);
            firstHitSound = true;
            secondHitSound = false;
        }
        else if (!hasPlayed && !secondHitSound && m_Healthpoint > 0)
        {
            audiosource.pitch = Random.Range(0.8f, 1f);
            audiosource.PlayOneShot(enemyHitSoundTwo, 0.55f);
            firstHitSound = false;
            secondHitSound = true;
        }

        hasPlayed = true;
        TakeDamage(2);
        playerHasAttacked = true;
        isHit = true;
        agent.speed = 0f;
        isRunningPatrol = false;
        animator.SetBool("isHit", true);
        animator.SetBool("Kicked", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Attack", false);
        yield return new WaitForSeconds(0.425f);
        animator.speed = 1;
        hasPlayed = false;
        isHit = false;
    }

    private IEnumerator GetKicked()
    {
        healthbar.Activate();
        animator.speed = 0.85f;
        playerHasAttacked = true;
        isHit = true;

        TakeDamage((float)playerAttack.m_DamageDone / 4);
        agent.speed = 0f;
        isRunningPatrol = false;
        animator.SetBool("Kicked", true);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        animator.SetBool("isHit", false);
        animator.SetBool("Attack", false);

        yield return new WaitForSeconds(1.2f);
        animator.speed = 1;
        isHit = false;
    }

    private IEnumerator EnemyDeath()
    {
        playerHasAttacked = false;
        isDead = true;

        animator.SetBool("Dead", true);
        animator.SetBool("Attack", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Kicked", false);
        animator.SetBool("isHit", false);

        agent.enabled = false;
        EnemyCollider.enabled = false;
        yield return null;
        enabled = false;
    }

    private IEnumerator EnemyDeathByAbility()
    {
        playerHasAttacked = false;
        isDead = true;
        audiosource.pitch = Random.Range(0.8f, 1f);
        audiosource.PlayOneShot(enemyHitSound, 0.55f);
        animator.SetBool("Dead", true);
        animator.SetBool("Attack", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Kicked", false);
        animator.SetBool("isHit", false);
        healthbar.Deactivate();
        agent.enabled = false;
        EnemyCollider.enabled = false;
        yield return null;
        enabled = false;
    }

    public void EnteringSafeHouse()
    {
        playerHasAttacked = false;
        inSafeHouse = true;
    }

    public void LeavingSafeHouse()
    {
        inSafeHouse = false;
    }

    public void ShootFireBall()
    {
        shootFireBall.FireBall();
        audiosource.PlayOneShot(fireBallSound);
    }

    public void EnemyAttackColliderEnable()
    {
        if (!isHit)
        {
            m_EnemySword.enabled = true;
        }
    }

    public void EnemyAttackColliderDisable()
    {
        if (m_EnemySword != null)
        {
            m_EnemySword.enabled = false;
        }
    }

    // Used by animation event
    public void EnemeyFootStep()
    {
        audiosource.volume = Random.Range(0.75f, 1f);
        audiosource.pitch = Random.Range(0.9f, 1f);
        audiosource.PlayOneShot(enemyFootStep);
    }

    // Used by animation event
    public void EnemeyKickedSound()
    {
        if (!hasPlayed && m_Healthpoint > 0)
        {
            audiosource.PlayOneShot(enemyKickedSound, 0.8f);
        }
    }

    public void TakeDamage(float damage)
    {
        m_Healthpoint -= damage;
    }

    public EnemyBase GetEnemy()
    {
        return this;
    }

    #if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, EnemyGlobal.Instance.CurrentMinDistanceForChase);
        Gizmos.DrawWireSphere(transform.position, EnemyGlobal.Instance.CurrentMinDistanceForAttack);
    }
    #endif
}