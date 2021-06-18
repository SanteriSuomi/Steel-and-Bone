using UnityEngine;
//using UnityEngine.SceneManagement;

/// <summary>
/// Helper class that provides data and methods for global use for enemies
/// </summary>
public class EnemyGlobal : Singleton<EnemyGlobal>
{
	public Transform Player { get; private set; }
	public PlayerAttack PlayerAttack { get; private set; }
	public SAMI_EnemyDamage EnemyDamage { get; private set; }
	public SAMI_PlayerStats PlayerStats { get; private set; }

	private PlayerCharacterController playerController;
	private CharacterController charController;

	private Collider[] playerColliders;
	public Collider[] GetPlayerColliders()
	{
		return playerColliders;
	}

	// As its dot product, higher value (from -1 to 1) means smaller fov.
	private float currentDotFieldOfViewThreshold = 0.9f;
	[SerializeField]
	private float maxFieldOfView = 0.7f;
	[SerializeField]
	private float minFieldOfView = 0.9f;

	public float CurrentMinDistanceForAttack { get; private set; }
	[SerializeField]
	private float maxDistanceForAttack = 7;
	[SerializeField]
	private float minDistanceForAttack = 4.2f;

	public float CurrentMinDistanceForAttackRanged { get; private set; }
	[SerializeField]
	private float maxDistanceForAttackRanged = 10;
	[SerializeField]
	private float minDistanceForAttackRanged = 5;

	public float CurrentMinDistanceForChase { get; private set; }
	[SerializeField]
	private float maxDistanceForChase = 11;
	[SerializeField]
	private float minDistanceForChase = 6.6f;

	[SerializeField]
	private float linecastStartOffsetDivide = 3;
	[SerializeField]
	private float linecastEndOffsetDivide = 3;

	private CrouchState playerCrouchState;
	public CrouchState PlayerCrouchState
	{
		get => playerCrouchState;
		set
		{
			playerCrouchState = value;
			switch (playerCrouchState)
			{
				case CrouchState.NotCrouched:
					PlayerNotCrouchEnemyState();
					break;

				case CrouchState.Crouched:
					PlayerCrouchEnemyState();
					break;
			}
		}
	}

	private void OnEnable()
	{
		ValidateReferences(Player);
		currentDotFieldOfViewThreshold = maxFieldOfView;
		CurrentMinDistanceForAttack = maxDistanceForAttack;
		CurrentMinDistanceForAttackRanged = maxDistanceForAttackRanged;
		CurrentMinDistanceForChase = maxDistanceForChase;
	}

	private void PlayerNotCrouchEnemyState()
	{
		currentDotFieldOfViewThreshold = maxFieldOfView;
		CurrentMinDistanceForAttack = maxDistanceForAttack;
		CurrentMinDistanceForChase = maxDistanceForChase;
		CurrentMinDistanceForAttackRanged = maxDistanceForAttackRanged;
	}

	private void PlayerCrouchEnemyState()
	{
		currentDotFieldOfViewThreshold = minFieldOfView;
		CurrentMinDistanceForAttack = minDistanceForAttack;
		CurrentMinDistanceForChase = minDistanceForChase;
		CurrentMinDistanceForAttackRanged = minDistanceForAttackRanged;
	}

	public bool PlayerIsInLineOfSightOf(Transform transform)
	{
		RaycastHit hit = Linecast(transform);
		float dot = DotProduct(transform);

		float minVelocityForDetection;
		float maxVelocityForDetection;
		if (playerController.CurrentCrouchState == CrouchState.NotCrouched)
		{
			minVelocityForDetection = 1;
			maxVelocityForDetection = 8;
		}
		else
		{
			minVelocityForDetection = 3;
			maxVelocityForDetection = 8;
		}

		return (hit.collider != null
			&& hit.collider.CompareTag("Player")
			&& dot >= currentDotFieldOfViewThreshold)
			|| (charController.velocity.sqrMagnitude >= minVelocityForDetection
			&& charController.velocity.sqrMagnitude <= maxVelocityForDetection);
	}

	private float DotProduct(Transform transform)
	{
		Vector3 directionToPlayer = (Player.position - transform.position).normalized;
		return Vector3.Dot(transform.forward, directionToPlayer);
	}

	private RaycastHit Linecast(Transform transform)
	{
		Vector3 startOffset = (transform.forward / linecastStartOffsetDivide) + Vector3.up;
		Vector3 endOffset = Vector3.up / linecastEndOffsetDivide;
		ValidateReferences(Player);
		Physics.Linecast(transform.position + startOffset, Player.position + endOffset, out RaycastHit hit);
		return hit;
	}

	private void ValidateReferences(Transform player)
	{
		if (player == null)
		{
			PlayerAttack playerAttack = FindObjectOfType<PlayerAttack>();
			Player = playerAttack.transform;
			PlayerAttack = playerAttack;
			playerColliders = Player.GetComponentsInChildren<Collider>();
			playerController = Player.GetComponent<PlayerCharacterController>();
			charController = Player.GetComponent<CharacterController>();
			EnemyDamage = FindObjectOfType<SAMI_EnemyDamage>();
			PlayerStats = FindObjectOfType<SAMI_PlayerStats>();
		}
	}

	public void SmoothLookAtPlayer(Transform transform, float speed, bool rotateVertically)
	{
		Vector3 lookDirection = (Player.position - transform.position).normalized;
		if (!rotateVertically)
		{
			lookDirection.y = 0;
		}

		Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, speed * Time.deltaTime);
	}
}