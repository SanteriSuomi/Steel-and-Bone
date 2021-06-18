using UnityEngine;

#pragma warning disable // Temp disable warnings
public class SAMI_HealthDrop : MonoBehaviour
{
	[SerializeField]
	private float healAmount = 50;
	public float HealAmount => healAmount;
	private SAMI_PlayerHealth m_Health;
	private SAMI_PlayerStats playerStats;
	public AudioSource m_AudioSource;
	public AudioClip m_HealthPickup;
	private MeshRenderer m_MeshRenderer;
	private CapsuleCollider m_Collider;
	private Vector3Int savedPositionKey;

	private void Awake()
	{
		m_MeshRenderer = GetComponent<MeshRenderer>();
		m_Collider = GetComponent<CapsuleCollider>();
	}

	private void OnEnable()
	{
		m_Health = InteractableComponents.Instance.PlayerHealth;
		playerStats = InteractableComponents.Instance.PlayerStats;

		savedPositionKey = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
		ObjectSaveManager.Instance?.AddHealthPotionPosition(savedPositionKey, transform.position);
	}

	private void OnDisable()
	{
		ObjectSaveManager.Instance?.RemoveHealthPotionPosition(savedPositionKey);
	}

	//private void Start()
	//{
	//	Invoke(nameof(DestroyPotion), 10);
	//}

	//private void OnTriggerEnter(Collider other)
	//{
	//	if (other.gameObject.CompareTag("Player")
	//		&& m_Health.m_CurrentHealth < m_Health.m_HealthPoints)
	//	{
	//		m_AudioSource.Play();
	//		m_Health.m_CurrentHealth += healAmount;
	//		m_Health.m_CurrentHealth = Mathf.Clamp(m_Health.m_CurrentHealth, 0, m_Health.m_HealthPoints);
	//		m_Collider.enabled = false;
	//		m_MeshRenderer.enabled = false;
	//		Invoke(nameof(DestroyPotion), 1);
	//	}
	//}

	//private void DestroyPotion()
	//{
	//	Destroy(gameObject);
	//}
}