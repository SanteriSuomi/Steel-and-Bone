using Essentials.Saving;
using MessagePack;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour, ISaveable
{
	public static bool IsBlocking { get; set; }
	public static bool IsAttacking { get; set; }
	public static bool IsCarryingThrowable { get; set; }

	public AnimationClip m_SwordAttack;
	public Animator m_Animator;
	public AnimationEvent m_ActivateTrigger;
	public Collider m_SwordTrigger;
	public int m_DamageDone = 15;
	public int DefaultDamageDone { get; private set; }
	public PlayerCharacterController m_Movement;
	public Collider m_FootCollider;
	public bool m_HasShield = false;
	public bool m_HasShieldUp = false;
	public bool m_HasToggledShield = false;
	public GameObject m_Shield;
	public GameObject m_PlayerHand;
	public StaminaBar m_StaminaBar;
	public float attackStaminaDrain = 10;
	public Collider m_LightAttack;
	public Rigidbody m_PlayerRigidBody;
	public BossRanged m_BossRanged;
	public bool m_IsMoving = false;
	public GameObject m_KeyA;
	public Image m_Key;
	public GameObject m_KeyParentInCanvas;
	public AudioSource m_AudioSource;
	public AudioClip m_SwordSlashSFX;
	public AudioSource m_FootStepLeft;
	public AudioSource m_FootStepRight;
	public bool AudioOne;
	public bool AudioTwo;
	public GameObject m_BossDoorKeyGameObject;
	[SerializeField] SAMI_PlayerStats m_PlayerStats;
	[SerializeField] GameObject m_LevelPicker;
	[SerializeField] public float m_MeleeStaminaDrain = 30f;
	[SerializeField]
	private float maxPlayerVelocityForAttack = 6;
	[SerializeField]
	private float minPlayerVelocityForAttack = 1;
	[SerializeField]
	private float sprintAnimatorSpeedMultiplier = 1.25f;
	[SerializeField]
	private ClassPickerClassInfo[] classPickers = default;
	[SerializeField]
	private LevelMenuScript levelMenuScript = default;
	private int classPickerLoadIndex;
	private bool hasDoorKey;
	public bool hasBossDoorKey;

	[SerializeField]
	private InputActionsVar inputActions = default;

	public AbilityBase ActiveAbility { get; set; } // Currently active abilities
	public ClassBase ActiveClass { get; set; } // Currently active class (attacks could be different depending on the class)

	private void Awake()
	{
		SaveSystem.Register(this);

		IsBlocking = false;
		IsAttacking = false;
		IsCarryingThrowable = false;
		m_Movement = GetComponent<PlayerCharacterController>();
		m_StaminaBar = GetComponent<StaminaBar>();
		m_Animator = GetComponentInChildren<Animator>();
		m_BossRanged = FindObjectOfType<BossRanged>();
		m_KeyA = GameObject.FindGameObjectWithTag("Key");
		m_Key = GameObject.Find("KeyImage").GetComponent<Image>();
		m_Key.gameObject.SetActive(false);
		m_BossDoorKeyGameObject = GameObject.FindGameObjectWithTag("BossDoorKey");
		m_PlayerStats = FindObjectOfType<SAMI_PlayerStats>();
		m_LevelPicker = GameObject.Find("LevelPicker");
		DefaultDamageDone = m_DamageDone;
	}

	private void OnEnable()
	{
		inputActions.InputActions.Player.Sprint.performed += OnSprintPerformed;
		inputActions.InputActions.Player.Sprint.canceled += OnSprintCanceled;
		inputActions.InputActions.Player.Block.performed += OnBlockPerformed;
		inputActions.InputActions.Player.Block.canceled += OnBlockCanceled;
		inputActions.InputActions.Player.Kick.performed += OnKickPerformed;
		inputActions.InputActions.Player.Kick.canceled += OnKickCanceled;
	}

	#region Input Events
	private bool isSprinting;
	private bool isBlocking;
	private bool isKicking;

	private void OnSprintPerformed(InputAction.CallbackContext context)
	{
		isSprinting = true;
	}

	private void OnSprintCanceled(InputAction.CallbackContext context)
	{
		isSprinting = false;
	}

	private void OnBlockPerformed(InputAction.CallbackContext context)
	{
		isBlocking = true;
	}

	private void OnBlockCanceled(InputAction.CallbackContext context)
	{
		isBlocking = false;
	}

	private void OnKickPerformed(InputAction.CallbackContext context)
	{
		isKicking = true;
	}

	private void OnKickCanceled(InputAction.CallbackContext context)
	{
		isKicking = false;
	}
	#endregion

	#region Saving
	public SaveData GetSave()
	{
		return new PlayerAttackData(gameObject.name)
		{
			currentClassType = ActiveClass.ClassType,
			damageDone = m_DamageDone,
			defaultDamageDone = DefaultDamageDone,
			attackStaminaDrain = attackStaminaDrain,
			meleeStaminaDrain = m_MeleeStaminaDrain,
			hasBossDoorKey = hasBossDoorKey,
			hasDoorKey = hasDoorKey
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is PlayerAttackData save)
		{
			m_DamageDone = save.damageDone;
			DefaultDamageDone = save.defaultDamageDone;
			attackStaminaDrain = save.attackStaminaDrain;
			m_MeleeStaminaDrain = save.meleeStaminaDrain;
			hasBossDoorKey = save.hasBossDoorKey;
			if (save.hasDoorKey)
			{
				m_KeyParentInCanvas.SetActive(true);
				m_KeyA.SetActive(false);
			}

			switch (save.currentClassType)
			{
				case ClassType.Warrior:
					(WarriorClass warriorClass, WarriorAbility warriorAbility) = ClassAbilityGetter.Instance.Warrior;
					ActiveClass = warriorClass;
					ActiveAbility = warriorAbility;
					classPickerLoadIndex = 1;
					break;

				case ClassType.Rogue:
					(RogueClass rogueClass, RogueAbility rogueAbility) = ClassAbilityGetter.Instance.Rogue;
					ActiveClass = rogueClass;
					ActiveAbility = rogueAbility;
					classPickerLoadIndex = 0;
					break;

				case ClassType.Ranged:
					(RangedClass rangedClass, RangedAbility rangedAbility) = ClassAbilityGetter.Instance.Ranged;
					ActiveClass = rangedClass;
					ActiveAbility = rangedAbility;
					classPickerLoadIndex = 2;
					break;

				default:
#if UNITY_EDITOR
					Debug.LogWarning("Couldn't load active class and ability!");
#endif
					break;
			}

			Invoke(nameof(FinishLoad), 1);
		}
	}

	private void FinishLoad()
	{
		levelMenuScript.Confirm();
		classPickers[classPickerLoadIndex].OnPointerClick(new PointerEventData(EventSystem.current));
		classPickers[0].transform.root.gameObject.SetActive(false); // Class Picker Canvas disable
	}

	[Serializable, MessagePackObject]
	public class PlayerAttackData : SaveData
	{
		[Key("ClassType")]
		public ClassType currentClassType;
		[Key("DamageDone")]
		public int damageDone;
		[Key("DefaultDamageDone")]
		public int defaultDamageDone;
		[Key("AttackStaminaDrain")]
		public float attackStaminaDrain;
		[Key("MeleeStaminaDrain")]
		public float meleeStaminaDrain;
		[Key("HasDoorKey")]
		public bool hasDoorKey;
		[Key("HasBossDoorKey")]
		public bool hasBossDoorKey;

		public PlayerAttackData() { }

		public PlayerAttackData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion

	private void Update()
	{
		if (m_LevelPicker.activeSelf) return; // Disable attack during ability picking

		float playerVelocity = m_Movement.CharacterController.velocity.sqrMagnitude;
		if (playerVelocity >= maxPlayerVelocityForAttack
			|| playerVelocity <= minPlayerVelocityForAttack)
		{
			Attacks();
			Abilities();
		}

		if (IsAttacking) return;

		#region WASD Move anims
		if ((inputActions.CurrentInputDirection & InputActionsVar.InputDirection.Up) != 0
			&& isSprinting
			&& m_StaminaBar.CurrentStamina > 0.5f
			&& !IsAttacking)
		{
			WalkAnims(sprintAnimatorSpeedMultiplier, "Sprint");
		}
		else if ((inputActions.CurrentInputDirection & InputActionsVar.InputDirection.Up) != 0)
		{
			WalkAnims(1, "Forward");
		}
		else if ((inputActions.CurrentInputDirection & InputActionsVar.InputDirection.Down) != 0)
		{
			ResetAnimatorParamsExcept("Backwards");
		}
		else if ((inputActions.CurrentInputDirection & InputActionsVar.InputDirection.Right) != 0)
		{
			ResetAnimatorParamsExcept("Right");
		}
		else if ((inputActions.CurrentInputDirection & InputActionsVar.InputDirection.Left) != 0)
		{
			ResetAnimatorParamsExcept("Left");
		}
		else
		{
			ResetAnimatorParamsExcept("Idle");
		}
		#endregion
	}

	private void WalkAnims(float speed, string elseRunParam)
	{
		m_Animator.speed = speed;
		if (m_Movement.CurrentCrouchState == CrouchState.Crouched)
		{
			ResetAnimatorParamsExcept("Crouched");
		}
		else if (InventoryItemBase.CurrentlyEquippedItem == ItemType.None)
		{
			ResetAnimatorParamsExcept("UnarmedWalk");
		}
		else
		{
			ResetAnimatorParamsExcept(elseRunParam);
		}

		m_Animator.speed = 1;
	}

	private void ResetAnimatorParamsExcept(string nameOfParameterToEnable)
	{
		PlayerAnimator.Instance.ResetAnimatorParamsExcept(nameOfParameterToEnable);
	}

	// Player attack
	private void Attacks()
	{
		if (IsAttacking) return;

		if (((Mouse.current?.leftButton.wasPressedThisFrame == true)
			|| (Gamepad.current?.rightShoulder.wasPressedThisFrame == true))
			//isAttacking
			&& m_StaminaBar.CurrentStamina >= m_MeleeStaminaDrain
			&& !IsCarryingThrowable)
		{
			//isAttacking = false;
			ActiveClass.UseMainAttack();
		}
		else if (isBlocking
				 && m_StaminaBar.CurrentStamina >= 15f
				 && InventoryItemBase.CurrentlyEquippedItem != ItemType.None
				 && !AbilityBase.IsExecutingAbility
				 && !IsBlocking)
		{
			ActiveClass.UseSecondaryAttack();
		}
		else if (isKicking
				 && m_StaminaBar.CurrentStamina >= 25f)
		{
			ActiveClass.UseKickAttack();
		}
	}

	// Player abilities
	private void Abilities()
	{
		if (AbilityBase.IsExecutingAbility
			|| InventoryItemBase.CurrentlyEquippedItem == ItemType.None)
		{
			return;
		}

		// Level 2
		if (((Keyboard.current?.digit1Key.wasPressedThisFrame == true)
			 || (Gamepad.current?.dpad.left.wasPressedThisFrame == true))
			 && m_PlayerStats.m_CurrentLevel >= 2
			 && !AbilityBase.IsDoingAbilityLevel2)
		{
			ActiveAbility.UseAbilityLevel2();
		}
		// Level 3
		else if (((Keyboard.current?.digit2Key.wasPressedThisFrame == true)
				  || (Gamepad.current?.dpad.up.wasPressedThisFrame == true))
				  && m_PlayerStats.m_CurrentLevel >= 3
				  && !AbilityBase.IsDoingAbilityLevel3)
		{
			ActiveAbility.UseAbilityLevel3();
		}
		// Level 4
		else if (((Keyboard.current?.digit3Key.wasPressedThisFrame == true)
				  || (Gamepad.current?.dpad.right.wasPressedThisFrame == true))
				  && m_PlayerStats.m_CurrentLevel >= 4
				  && !AbilityBase.IsDoingAbilityLevel4)
		{
			ActiveAbility.UseAbilityLevel4();
		}
	}

	#region Key Methods
	private void FixedUpdate()
	{
		RotateMazeKey();
	}

	private void RotateMazeKey()
	{
		if (m_KeyParentInCanvas.activeSelf)
		{
			m_Key.transform.Rotate(0, 1f, 0);
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.CompareTag("Key"))
		{
			QuestContext mazeKeyQuest = QuestManager.Instance.GetQuest(0);
			mazeKeyQuest.Data.IsCompleted = true;
			hasDoorKey = true;
			m_KeyParentInCanvas.SetActive(true);
			m_KeyA.SetActive(false);
		}
		else if (collision.gameObject.CompareTag("BossDoorKey"))
		{
			QuestContext findDemonKeyQuest = QuestManager.Instance.GetQuest(3);
			findDemonKeyQuest.Data.IsCompleted = true;
			hasBossDoorKey = true;
			collision.gameObject.SetActive(false);
		}
	}
	#endregion

	// Public because used from animation events?
	public void EnablePlayerSwordTrigger()
	{
		m_SwordTrigger.enabled = true;
	}

	public void DisablePlayerSwordTrigger()
	{
		m_SwordTrigger.enabled = false;
	}

	private void OnDisable()
	{
		inputActions.InputActions.Player.Sprint.performed -= OnSprintPerformed;
		inputActions.InputActions.Player.Sprint.canceled -= OnSprintCanceled;
		inputActions.InputActions.Player.Block.performed -= OnBlockPerformed;
		inputActions.InputActions.Player.Block.canceled -= OnBlockCanceled;
		inputActions.InputActions.Player.Kick.performed -= OnKickPerformed;
		inputActions.InputActions.Player.Kick.canceled -= OnKickCanceled;
	}
}