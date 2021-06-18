using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WarriorAbility : AbilityBase
{
	public static bool IsUsingCombo { get; private set; }

	[SerializeField]
	ParticleSystem m_WarriorSlamParticles = default;
	private Coroutine chargeCoroutine;
	private WaitForFixedUpdate chargeFixedUpdate;
	private Coroutine cameraFovChange;
    private AudioSource m_AudioSource; 

	[Header("Combo")]
	[SerializeField]
	private float comboAbilityDisableDuration = 2.5f;
	[SerializeField]
	private int comboAbilityCooldown = 7;
	[SerializeField]
	private float comboStaminaDrain = 50;

	[Header("Charge")]
	[SerializeField]
	private int chargeAbilityCooldown = 10;
	[SerializeField]
	private float chargeSpeedMultiplier = 1.5f;
	[SerializeField]
	private float chargeGravityMultiplier = 5;
	[SerializeField]
	private float chargeStaminaDrain = 0.75f;
	[SerializeField]
	private float chargeDamageMultiplier = 1.4f;
	[SerializeField]
	private float chargeCameraFovChange = 120;
	[SerializeField]
	private float chargeCameraFovChangeSpeed = 2;
	[SerializeField]
	private float chargePlayerDamageAmount = 10;

	[Header("Slam")]
	[SerializeField]
	private float slamAbilityDisableDuration = 3.75f;
	[SerializeField]
	private int slamAbilityCooldown = 12;
	[SerializeField]
	private float slamRadius = 4;
	[SerializeField]
	private float slamDamageMultiplier = 3;
	[SerializeField]
	private float slamBossDamageExtraMultiplier = 1.5f;
	[SerializeField]
	private float slamKnockbackMultiplier = 2;
	[SerializeField]
	private float slamKnockbackMoveSpeed = 10;
	[SerializeField]
	private float slamMinEdgeBreakDistance = 0.1f;
	[SerializeField]
	private float slamStaminaDrain = 60;

	private void Awake()
	{
		IsUsingCombo = false;
		chargeFixedUpdate = new WaitForFixedUpdate();
        m_AudioSource = GetComponent<AudioSource>();
	}

	public override void UseAbilityLevel2()
	{
		if (NotNoughtStamina(comboStaminaDrain)) return;
		DisableAbilitiesFor(comboAbilityDisableDuration);
		StartLevel2Cooldown(comboAbilityCooldown);
		StartCoroutine(WarriorCombo());
	}

	private IEnumerator WarriorCombo()
	{
		PlayerAttack.IsAttacking = true;
		IsUsingCombo = true;
		Invoke(nameof(DisableWarriorCombo), 2.7f);
		PlayerAnimator.Instance.ResetAnimatorParamsExcept("WarriorCombo");
		m_Stam.DrainStamina(comboStaminaDrain);
		yield return null;
	}

	private void DisableWarriorCombo() 
	{
		playerController.m_SwordAttack.DisablePlayerSwordTrigger();
		PlayerAnimator.Instance.ResetAnimatorParamsExcept("Idle");
		PlayerAttack.IsAttacking = false;
		IsUsingCombo = false;
	}

	public override void UseAbilityLevel3()
	{
		if (NotNoughtStamina(chargeStaminaDrain * 3)) return;
		StartLevel3Cooldown(chargeAbilityCooldown);
		if (chargeCoroutine != null)
		{
			StopCoroutine(chargeCoroutine);
		}

		chargeCoroutine = StartCoroutine(Charge());
	}

	private IEnumerator Charge()
	{
		IsExecutingAbility = true;
		PlayerAttack.IsAttacking = true;

		if (cameraFovChange != null)
		{
			StopCoroutine(cameraFovChange);
		}

		cameraFovChange = StartCoroutine(SmoothFovChange(chargeCameraFovChange));

		// Multiply gravity so character doesnt start flying during charge
		playerController.m_gravity *= chargeGravityMultiplier;
		playerController.Collided = false;
		while (!playerController.Collided && m_Stam.CurrentStamina >= 1)
		{
			// Force these states during charge
			playerAnimator.SetBool("Idle", false);
			playerAnimator.SetBool("Sprint", true);

			Vector3 forward = new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z).normalized;
			float sprintSpeed = playerController.m_sprint * chargeSpeedMultiplier;
			SprintData chargeSprintData = new SprintData(forward, sprintSpeed, chargeStaminaDrain, false, true, false);
			playerController.Sprint(chargeSprintData);

			yield return chargeFixedUpdate;
		}

		playerController.m_gravity = originalGravity;

		StopCoroutine(cameraFovChange);
		cameraFovChange = StartCoroutine(SmoothFovChange(originalCameraFov));

		DamagePlayer();
		DealDamageInRadius(m_Player.transform.position, slamRadius, chargeDamageMultiplier);

		PlayerAnimator.Instance.ResetAnimatorParamsExcept("Idle");
		PlayerAttack.IsAttacking = false;
		IsExecutingAbility = false;
	}

	private void DamagePlayer()
	{
		if (playerHealth.m_CurrentHealth >= chargePlayerDamageAmount + 1)
		{
            m_AudioSource.Play();
            playerHealth.m_CurrentHealth -= chargePlayerDamageAmount;
		}
	}

	private IEnumerator SmoothFovChange(float toValue)
	{
		while (enabled)
		{
			playerCamera.fieldOfView = Mathf.MoveTowards(playerCamera.fieldOfView, toValue,
				chargeCameraFovChangeSpeed * Time.deltaTime);
			yield return null;
		}
	}

	public override void UseAbilityLevel4()
	{
		if (NotNoughtStamina(slamStaminaDrain)) return;
		DisableAbilitiesFor(slamAbilityDisableDuration);
		StartLevel4Cooldown(slamAbilityCooldown);

		PlayerAttack.IsAttacking = true;
		PlayerAnimator.Instance.ResetAnimatorParamsExcept("Idle");
		m_Stam.DrainStamina(slamStaminaDrain);
		playerAnimator.SetTrigger("WarriorSlam");
		playerAnimator.SetBool("Idle", false);
	}

	// Gets called from the warrior slam animation event
	public void WarriorSlam()
	{
		m_WarriorSlamParticles.Play();
		DealDamageInRadius(m_Player.transform.position, slamRadius, slamDamageMultiplier);
		Invoke(nameof(OnDisableWarriorSlamParticleEffects), 0.3f);
	}

	private void OnDisableWarriorSlamParticleEffects()
	{
		m_WarriorSlamParticles.Stop();
	}

	private void DealDamageInRadius(Vector3 atPosition, float inRadius, float damageMultiplier)
	{
		Collider[] hitResults = Physics.OverlapSphere(atPosition, inRadius);
		for (int i = 0; i < hitResults.Length; i++)
		{
			if (hitResults[i].TryGetComponent(out IDamageable enemy))
			{
				DamageEnemy(enemy, damageMultiplier);
			}
		}
	}

	private void DamageEnemy(IDamageable enemy, float damageMultiplier)
	{
		if (enemy.GetEnemy() == null) // Enemy is the boss
		{
			enemy.TakeDamage(m_PlayerAttack.m_DamageDone * damageMultiplier * slamBossDamageExtraMultiplier);
		}
		else // Enemy is a normal enemy
		{
			enemy.TakeDamage(m_PlayerAttack.m_DamageDone * damageMultiplier);
		}

		Transform enemyTransform;
		EnemyBase enemyBase = enemy.GetEnemy();
		if (enemyBase == null)
		{
			enemyTransform = (enemy as MonoBehaviour).transform;
		}
		else
		{
			enemyTransform = enemy.GetEnemy().transform;
		}

		Vector3 enemyPosition = enemyTransform.position;
		Vector3 offset = (enemyPosition - m_Player.transform.position).normalized;
		offset.y = 0; // Do not move enemy in Y direction
		Vector3 targetPosition = enemyPosition + (offset * slamKnockbackMultiplier);

		StartCoroutine(KnockbackEnemy(enemy.GetEnemy(), targetPosition));
	}

	private IEnumerator KnockbackEnemy(EnemyBase enemy, Vector3 targetPosition)
	{
		if (enemy == null) yield break;

		StartCoroutine(enemy.GetHit());
		while (!SBUtils.PosApproxEqual(enemy.transform.position, targetPosition, 0.01f))
		{
			// Make sure enemy does not get knocked out of navmesh.
			NavMesh.FindClosestEdge(enemy.transform.position, out NavMeshHit hit, NavMesh.AllAreas);
			if (hit.distance < slamMinEdgeBreakDistance)
			{
				break;
			}

			enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition,
				slamKnockbackMoveSpeed * Time.deltaTime);
			yield return null;
		}
	}
}