using Essentials.Saving;
using MessagePack;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossRanged : MonoBehaviour, IDamageable, IHasHealthbar, ISaveable
{
	public float Health => m_BossHealth;

	public EnemyHealthBar Healthbar { get; private set; }
	public void GiveHealthBar(EnemyHealthBar healthbar)
	{
		Healthbar = healthbar;
	}

	public Transform m_PlayerTransform;
	public Animator m_Animator;
	public Transform m_RightHand;
	public BossAoeSpawner bossAoeSpawner;
	public float meleeRange = 6f;
	public float m_SpinRange = 2.5f;
	public int m_BossHealth = 2000;
	public PlayerAttack m_SwordAttack;
	public bool m_MeleeAttacking = false;
	public bool m_MeleeMode = true;
	public bool m_AreaAttack;
	public bool m_IsDead = false;
	public bool m_TimeToAttackArea;
	public bool m_IsInRadius;
	public bool m_SpinAttack = false;
	public bool m_EnteredBossRoom = false;
	public Rigidbody m_PlayerRigidbody;
	public Transform m_BossTransform;
	public Vector3 m_PlayerVector3;
	public bool m_HasSingleAttacked;
	public PlayerCharacterController m_VeikkosCC;
	public SAMI_ImpactReceiver m_ImpactReceiver;
	public float radius = 7.0F;
	public float power = 500000.0F;
	public float AoeRange = 0.2f;
	public NavMeshAgent m_NavMeshAgent;
	public Collider m_BossSwordTrigger;
	public Collider m_BossJumpTrigger;
	public Collider m_BossCollider;
	public Collider m_BossDeadCollider;
	public Rigidbody m_BossRigidBody;
	public Collider m_BossSpin;
	//public BOSSRandomizer m_BossRandomizer;
	public bool m_PerformSpin = false;
	public bool m_PerformingSpin = false;
	public AudioSource m_SourceAudio;
	public bool m_AudioPlayed = false;
	public AudioSource m_SpinAudio;
	public AudioClip m_Slash;
	public AudioClip m_SpinningBlade;
	public AudioSource m_LandBoss;
	public AudioClip m_BossLandingSFX;
	public float m_ChaseRange = 14f;
	public Animator m_FinalDoor;
	public Animator m_FinalDoorB;
	public Collider m_FinalDoorCollider;
	[SerializeField]
	private SAMI_BossDoor bossDoor = default;
	[SerializeField]
	private GameObject endZoneDoor = default;

	private void Awake()
	{
		SaveSystem.Register(this);

		m_AreaAttack = false;
		m_TimeToAttackArea = false;
		m_HasSingleAttacked = false;
		m_SourceAudio = GetComponent<AudioSource>();
		m_VeikkosCC = FindObjectOfType<PlayerCharacterController>();
		m_PlayerTransform = m_VeikkosCC.transform;
		m_PlayerRigidbody = m_PlayerTransform.GetComponent<Rigidbody>();
		m_SwordAttack = FindObjectOfType<PlayerAttack>();
		m_ImpactReceiver = FindObjectOfType<SAMI_ImpactReceiver>();
	}

	#region Saving
	public SaveData GetSave()
	{
		return new BossData(gameObject.name)
		{
			position = transform.position,
			isDead = m_IsDead,
			health = m_BossHealth
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is BossData save)
		{
			m_IsDead = save.isDead;
			if (m_IsDead)
			{
				transform.position = save.position;
				DeathEvent();
			}

			m_BossHealth = save.health;
		}
	}

	[Serializable, MessagePackObject]
	public class BossData : SaveData
	{
		[Key("Position")]
		public Vector3 position;
		[Key("IsDead")]
		public bool isDead;
		[Key("Health")]
		public int health;

		public BossData() { }

		public BossData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Sword"))
		{
			int damageDone = m_SwordAttack.m_DamageDone + 7;
			TakeDamage(damageDone);
			m_SourceAudio.Play();
		}
		else if (other.gameObject.CompareTag("LightAttackSword"))
		{
			m_BossHealth -= m_SwordAttack.m_DamageDone - 7;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Pillar"))
		{
			ChasePlayer();
		}
		else
		{
			m_MeleeMode = true;
		}
	}

	//private IEnumerator PlayBossAudio()
	//{
	//	m_AudioPlayed = true;
	//	m_SourceAudio.Play();
	//	yield return new WaitForSeconds(1f);
	//	m_AudioPlayed = false;
	//}

	private void Update()
	{
		if (m_IsDead) return;

		if (m_BossHealth <= 0 && !m_IsDead)
		{
			DeathEvent();
		}

		if (Vector3.Distance(transform.position, m_PlayerRigidbody.position) > AoeRange && m_BossHealth <= 400 && !m_AreaAttack
			&& !m_TimeToAttackArea && !m_IsDead && m_EnteredBossRoom)
		{
			StartCoroutine(BossAreaAttack());
			//m_BossRandomizer.Randomizer();

		}
		else if (Vector3.Distance(transform.position, m_PlayerRigidbody.position) < m_SpinRange && !m_MeleeAttacking
			&& m_MeleeMode && !m_IsDead && !m_SpinAttack && !m_PerformingSpin && m_EnteredBossRoom)
		{
			StartCoroutine(SpinAttack());

		}
		else if (Vector3.Distance(transform.position, m_PlayerRigidbody.position) < meleeRange && !m_MeleeAttacking
			&& m_MeleeMode && !m_IsDead && !m_SpinAttack && m_EnteredBossRoom)
		{
			StartCoroutine(AttackingPlayer());
		}
		else if (Vector3.Distance(transform.position, m_PlayerRigidbody.position) < m_ChaseRange && !m_MeleeAttacking
			&& m_MeleeMode && !m_IsDead && !m_SpinAttack && m_EnteredBossRoom)
		{
			ChasePlayer();
		}

		//if(Vector3.Distance(transform.position, m_PLayer.position) > meleeRange && m_BossHealth <= 1001 && m_HasSingleAttacked == false)
		//{
		//    StartCoroutine(BossAttackSingle());
		//}
		m_PlayerVector3 = m_PlayerRigidbody.transform.position;
	    Healthbar.UpdateValue();
	}

	private void DeathEvent()
	{
		m_IsDead = true;
		m_Animator.SetBool("IsDead", true);
		m_Animator.SetBool("AreaAttack", false);
		m_Animator.SetBool("SingleAttack", false);
		m_Animator.SetBool("Idling", false);
		m_Animator.SetBool("Spin", false);
		m_NavMeshAgent.enabled = false;
		m_BossCollider.enabled = false;
		m_BossRigidBody.isKinematic = false;
		m_BossRigidBody.useGravity = true;
		m_BossRigidBody.mass = 1;
		m_BossRigidBody.constraints = RigidbodyConstraints.None;
		m_BossDeadCollider.enabled = true;
		bossDoor.SetBossFightState(new BossFightState(openDoor: true, enableCollider: false, enableBossMusic: false, isLoad: false));
		Collider[] bossDoorColliders = bossDoor.GetComponentsInChildren<Collider>();
		for (int i = 0; i < bossDoorColliders.Length; i++)
		{
			bossDoorColliders[i].enabled = false;
		}

		Healthbar.Deactivate();
		CompleteQuestAchievements();
		endZoneDoor.SetActive(false);
	}

	private static void CompleteQuestAchievements()
	{
		QuestContext killDemonQuest = QuestManager.Instance.GetQuest(5);
		killDemonQuest.Data.IsCompleted = true;
		AchievementManager.Activate(Achievements.Get("DEMON_SLAYER"));
	}

	//IEnumerator BossAttackSingle()
	//{
	//    m_HasSingleAttacked = true;
	//    Vector3 lookVector = m_PLayer.position - transform.position;
	//    lookVector.y = transform.position.y;
	//    Quaternion rot = Quaternion.LookRotation(lookVector);
	//    transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);
	//    m_Animator.SetBool("AreaAttack", false);
	//    m_Animator.SetBool("SingleAttack", true);
	//    m_Animator.SetBool("Idling", false);        
	//    print("Shooting");
	//    yield return new WaitForSeconds(2.26f);
	//    m_Animator.SetBool("AreaAttack", false);
	//    m_Animator.SetBool("SingleAttack", false);
	//    m_Animator.SetBool("Idling", true);
	//    yield return new WaitForSeconds(5f);
	//    m_HasSingleAttacked = false;
	//}

	//void SingleBossAttack()
	//{
	//    Instantiate(m_BossProjecticle, m_SingleProjectileShootPoint.position, m_SingleProjectileShootPoint.rotation);
	//}

	//IEnumerator BossAreaAttackWait()
	//{
	//	yield return new WaitForSeconds(40f);
	//	m_TimeToAttackArea = true;
	//}

	private IEnumerator SpinAttack()
	{
		m_PerformingSpin = true;
		m_SpinAttack = true;
		m_MeleeAttacking = true;
		m_Animator.SetBool("AreaAttack", false);
		m_Animator.SetBool("SingleAttack", false);
		m_Animator.SetBool("Idling", false);
		m_Animator.SetBool("IsDead", false);
		m_Animator.SetBool("Spin", true);
		m_SpinAudio.PlayOneShot(m_SpinningBlade);
		yield return new WaitForSeconds(0.8f);
		m_BossSpin.enabled = true;
		yield return new WaitForSeconds(0.4f);
		m_BossSpin.enabled = false;
		yield return new WaitForSeconds(0.25f);
		m_MeleeAttacking = false;
		m_SpinAttack = false;
		m_Animator.SetBool("AreaAttack", false);
		m_Animator.SetBool("SingleAttack", false);
		m_Animator.SetBool("Idling", true);
		m_Animator.SetBool("IsDead", false);
		m_Animator.SetBool("Spin", false);
		yield return new WaitForSeconds(6f);
		m_PerformingSpin = false;
	}

	private IEnumerator AttackingPlayer()
	{
		m_MeleeAttacking = true;
		m_NavMeshAgent.speed = 0f;
		Vector3 targetPosition = new Vector3(m_PlayerRigidbody.position.x, transform.position.y, m_PlayerRigidbody.position.z);
		transform.LookAt(targetPosition);
		m_Animator.SetBool("AreaAttack", false);
		m_Animator.SetBool("SingleAttack", true);
		m_Animator.SetBool("Idling", false);
		m_Animator.SetBool("IsDead", false);
		m_Animator.SetBool("Spin", false);
		yield return new WaitForSeconds(0.8f);
		m_BossSwordTrigger.enabled = true;
		m_SpinAudio.PlayOneShot(m_Slash);
		yield return new WaitForSeconds(0.4f);
		m_BossSwordTrigger.enabled = false;
		yield return new WaitForSeconds(0.25f);
		m_MeleeAttacking = false;
		m_Animator.SetBool("AreaAttack", false);
		m_Animator.SetBool("SingleAttack", false);
		m_Animator.SetBool("Idling", true);
		m_Animator.SetBool("IsDead", false);
		m_Animator.SetBool("Spin", false);
	}

	public void ChasePlayer()
	{
		if (m_NavMeshAgent.enabled)
		{
			m_NavMeshAgent.destination = m_PlayerRigidbody.position;
			m_NavMeshAgent.speed = 2f;
		}

		m_Animator.SetBool("AreaAttack", false);
		m_Animator.SetBool("SingleAttack", false);
		m_Animator.SetBool("Idling", true);
		m_Animator.SetBool("IsDead", false);
		m_Animator.SetBool("Spin", false);
	}

	private IEnumerator BossAreaAttack()
	{
		m_MeleeMode = false;
		m_AreaAttack = true;
		BossRangedAttack();
		m_TimeToAttackArea = true;
		yield return new WaitForSeconds(1f);
		m_Animator.SetBool("AreaAttack", true);
		m_Animator.SetBool("SingleAttack", false);
		m_Animator.SetBool("Idling", false);
		m_Animator.SetBool("Spin", false);
		m_Animator.SetBool("IsDead", false);
		if (m_NavMeshAgent.enabled)
		{
			m_NavMeshAgent.destination = m_PlayerRigidbody.position;
			m_NavMeshAgent.speed = 8f;
		}

		m_BossJumpTrigger.enabled = true;
		yield return new WaitForSeconds(2.26f);
		m_MeleeMode = true;
		m_BossJumpTrigger.enabled = false;
		m_Animator.SetBool("AreaAttack", false);
		m_Animator.SetBool("SingleAttack", false);
		m_Animator.SetBool("Idling", true);
		m_Animator.SetBool("IsDead", false);
		m_Animator.SetBool("Spin", false);
		yield return new WaitForSeconds(10f);
		m_AreaAttack = false;
		m_TimeToAttackArea = false;
	}

	public void LandAttack()
	{
		m_LandBoss.PlayOneShot(m_BossLandingSFX);
	}

	private void BossRangedAttack()
	{
		bossAoeSpawner.StartMeteorSwarm();
		Vector3 explosionPos = transform.position;
		foreach (Collider hit in Physics.OverlapSphere(explosionPos, radius))
		{
			var dir = hit.transform.position - explosionPos;
			var force = Mathf.Clamp(power / 3, 0, 60);
			m_ImpactReceiver.AddImpact(dir, force);
		}
	}

	public void EnteringBossRoom()
	{
		m_EnteredBossRoom = true;
	}

	public void TakeDamage(float damage)
	{
		m_BossHealth -= Mathf.CeilToInt(damage);
	}

	public EnemyBase GetEnemy()
	{
		return null;
	}

	#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, meleeRange);
		Gizmos.DrawWireSphere(transform.position, m_SpinRange);
		Gizmos.DrawWireSphere(transform.position, m_ChaseRange);
	}
	#endif
}