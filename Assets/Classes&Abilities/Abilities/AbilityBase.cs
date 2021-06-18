using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class AbilityBase : MonoBehaviour
{
	public static bool IsExecutingAbility { get; protected set; } // Enable whilst ability is playing, disable while it is not

	protected Animator playerAnimator;
	protected PlayerAttack m_PlayerAttack;
	protected PlayerCharacterController playerController;
	protected StaminaBar m_Stam;
	protected GameObject m_Player;
	protected Camera playerCamera;
	protected GameObject inventorySlot1;
	protected CharacterController m_CharacterController;
	protected AudioSource abilityAudSrc;
	protected SAMI_PlayerHealth playerHealth;
	protected AbilityBarHandler abilityBarHandler;
	protected float originalGravity;
	protected float originalCameraFov;

	private void Awake()
	{
		IsExecutingAbility = false;
		IsDoingAbilityLevel2 = false;
		IsDoingAbilityLevel3 = false;
		IsDoingAbilityLevel4 = false;
	}

	private void Start()
	{
		m_Player = GameObject.FindGameObjectWithTag("Player");
		playerAnimator = m_Player.GetComponentInChildren<Animator>();
		m_PlayerAttack = m_Player.GetComponent<PlayerAttack>();
		m_Stam = m_Player.GetComponent<StaminaBar>();
		playerController = m_Player.GetComponent<PlayerCharacterController>();
		originalGravity = playerController.m_gravity;
		playerCamera = m_Player.GetComponentInChildren<Camera>();
		originalCameraFov = playerCamera.fieldOfView;
		inventorySlot1 = GameObject.Find("Inventory").transform.GetChild(0).gameObject; // Get the first slot of the inventory
		m_CharacterController = playerController.GetComponent<CharacterController>();
		playerHealth = m_Player.GetComponent<SAMI_PlayerHealth>();
		abilityBarHandler = FindObjectOfType<AbilityBarHandler>();
	}

	public void SetAudioSource(AudioSource audSrc)
		=> abilityAudSrc = audSrc;

	protected bool CantUseAbility // Conditions that cancel ability use
	{
		get => inventorySlot1.activeSelf
			|| IsExecutingAbility
			|| m_Stam.CurrentStamina <= 50
			|| InventoryItemBase.CurrentlyEquippedItem == ItemType.None
			|| PlayerAttack.IsAttacking
			|| PlayerAttack.IsBlocking;
	}

	public virtual void Activate() => m_PlayerAttack.ActiveAbility = this;

	/// <summary>
	/// Enable and then disable "IsExecutingAbilit" variable on delay. 
	/// Use when you don't want player to move during a set duration while carrying out an ability.
	/// </summary>
	/// <param name="delayTime"></param>
	protected void DisableAbilitiesFor(float delayTime)
	{
		IsExecutingAbility = true;
		Invoke(nameof(DelayIsExecutingFalse), delayTime);
	}

	private void DelayIsExecutingFalse() => IsExecutingAbility = false;

	#region Ability Cooldown Methods
	protected void StartLevel2Cooldown(int time)
	{
		StartCoroutine(Level2CooldownCoroutine(time));
	}
	private IEnumerator Level2CooldownCoroutine(int time)
	{
		abilityBarHandler.SetCooldownAbilityGrey(0);
		IsDoingAbilityLevel2 = true;
		yield return new WaitForSeconds(time);
		IsDoingAbilityLevel2 = false;
		abilityBarHandler.SetCooldownAbilityColor(0);
	}
	public static bool IsDoingAbilityLevel2 { get; private set; }

	protected void StartLevel3Cooldown(int time)
	{
		StartCoroutine(Level3CooldownCoroutine(time));
	}
	private IEnumerator Level3CooldownCoroutine(int time)
	{
		abilityBarHandler.SetCooldownAbilityGrey(1);
		IsDoingAbilityLevel3 = true;
		yield return new WaitForSeconds(time);
		IsDoingAbilityLevel3 = false;
		abilityBarHandler.SetCooldownAbilityColor(1);
	}
	public static bool IsDoingAbilityLevel3 { get; private set; }

	protected void StartLevel4Cooldown(int time)
	{
		StartCoroutine(Level4CooldownCoroutine(time));
	}
	private IEnumerator Level4CooldownCoroutine(int time)
	{
		abilityBarHandler.SetCooldownAbilityGrey(2);
		IsDoingAbilityLevel4 = true;
		yield return new WaitForSeconds(time);
		IsDoingAbilityLevel4 = false;
		abilityBarHandler.SetCooldownAbilityColor(2);
	}
	public static bool IsDoingAbilityLevel4 { get; private set; }
	#endregion

	/// <summary>
	/// Same as "DisableExecuteDelay" except it's using animator bool for delay 
	/// (delay until the specified bool returns false).
	/// </summary>
	/// <param name="animatorBool"></param>
	protected void DisableExecuteBool(string animatorBool)
		=> StartCoroutine(DelayWhileBoolFalse(animatorBool));

	private IEnumerator DelayWhileBoolFalse(string animatorBool)
	{
		IsExecutingAbility = true;

		while (!playerAnimator.GetBool(animatorBool))
		{
			yield return null;
		}

		IsExecutingAbility = false;
	}

	protected bool NotNoughtStamina(float amountOfStaminaDrain)
	{
		if (m_Stam.CurrentStamina < amountOfStaminaDrain)
		{
			StaminaPopupManager.Instance.Activate();
			return true;
		}

		return false;
	}

	public abstract void UseAbilityLevel2();
	public abstract void UseAbilityLevel3();
	public abstract void UseAbilityLevel4();

	public void PlayAudioClip(AudioClip clip, float minPitch, float minVolume, float maxVolume)
	{
		abilityAudSrc.pitch = Random.Range(minPitch, maxVolume);
		abilityAudSrc.volume = Random.Range(minVolume, maxVolume);
		abilityAudSrc.PlayOneShot(clip);
	}
}