using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CrouchState
{
    NotCrouched,
    Crouched
}

public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField] public float m_speed = 6.0f;
    public float DefaultSpeed { get; private set; }
    [SerializeField] private float crouchSpeed = 3.5f;
    [SerializeField] public float m_rotateSpeed = 6.0f;
    [SerializeField] public float m_jumpspeed = 8.0f;
    [SerializeField] public float m_gravity = 20.0f;
    [SerializeField] public float m_sprint;
    [SerializeField] private float runSprintDrain = 0.5f;
    [SerializeField] private Vector3 moveDirection = Vector3.zero;
    [SerializeField] CharacterController m_controller;
    public CharacterController CharacterController => m_controller;
    [SerializeField] StaminaBar m_stam;
    public Animator m_Animator;
    public PlayerAttack m_SwordAttack;
    public bool m_AttackingMain = false;
    private bool mLockPickup = false;
    public Aleksi_Inventory _Inventory;
    public GameObject Hand;
    public GameObject DaggerPos;
    public Aleksi_HUD Hud;
    public Collider m_SwordTrigger;
    private GameObject levelPickerMenu;
    private SAMI_PlayerHealth m_PlayerHealth;
    private WaitForSeconds collisionDetectionWFS;
    private FootStepAudioController footStepsController;
    [SerializeField]
    private float crouchFootStepsMultiplier = 0.6f;
    [SerializeField]
    private float backwardsMovingMultiplier = 0.6f;
    [SerializeField]
    private string[] collisionNames = default;
    [SerializeField]
    private Vector3 crouchOffset = new Vector3(0, 0.5f, 0);
    [SerializeField]
    private float crouchHeightOffset = 0.2f;
    [SerializeField]
    private Vector3 daggerHandPosition = new Vector3(-0.1981f, 0.3129f, 0.0285f);
    [SerializeField]
    private Vector3 daggerHandRotation = new Vector3(232.922f, -170.145f, -222.527f);
    [SerializeField]
    private float jumpStaminaDrain = 17.5f;
    private IInventoryItem mItemToPickup = null;
    private WaitForSeconds disableMessagePanelWFS;
    private Coroutine checkItemTypeCoroutine;
    [SerializeField]
    private float jumpCooldownDuration = 0.75f;
    private float jumpTimer;
    [SerializeField]
    private InputActionsVar inputActions = default;
    private Vector2 inputMoveVector;
    private bool isJumping;
    private bool isSprinting;
    private bool isCheckingItems;

    [SerializeField]
    private float moveStartingIncreaseMultiplier = 1.5f;
    private Coroutine moveStartingMultiplierCoroutine;
    private float moveStartingMultiplier;

    private CrouchState currentCrouchState = CrouchState.NotCrouched;
    public CrouchState CurrentCrouchState
    {
        get => currentCrouchState;
        private set
        {
            currentCrouchState = value;
            // Update crouch state for enemies
            EnemyGlobal.Instance.PlayerCrouchState = CurrentCrouchState;
        }
    }

    private void Awake()
    {
        enabled = true; // enabled because it mysteriously deactivates when starting play mode.
        DefaultSpeed = m_speed;
        m_controller = GetComponent<CharacterController>();
        m_stam = GetComponent<StaminaBar>();
        m_Animator = GetComponentInChildren<Animator>();
        Hud = FindObjectOfType<Aleksi_HUD>();
        _Inventory = FindObjectOfType<Aleksi_Inventory>();
        levelPickerMenu = GameObject.Find("LevelPicker");
        footStepsController = FindObjectOfType<FootStepAudioController>();
        m_PlayerHealth = FindObjectOfType<SAMI_PlayerHealth>();
        disableMessagePanelWFS = new WaitForSeconds(0.05f);
        collisionDetectionWFS = new WaitForSeconds(0.5f);
    }

    private void OnEnable()
    {
        _Inventory.ItemUsed += Inventory_ItemUsed;
        inputActions.InputActions.Player.Move.performed += OnMovePerformed;
        inputActions.InputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.InputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.InputActions.Player.Pickup.performed += OnPickupPerformed;
        inputActions.InputActions.Player.Sprint.performed += OnSprintPerformed;
        inputActions.InputActions.Player.Sprint.canceled += OnSprintCanceled;
        inputActions.InputActions.Player.Crouch.performed += OnCrouchPerformed;
        inputActions.InputActions.Player.Crouch.canceled += OnCrouchCanceled;
    }

    #region Input Events
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        inputMoveVector = context.ReadValue<Vector2>();
        moveStartingMultiplierCoroutine = StartCoroutine(MoveStartingMultiplierCoroutine());
    }

    private IEnumerator MoveStartingMultiplierCoroutine()
    {
        while (enabled)
        {
            moveStartingMultiplier += Time.deltaTime * moveStartingIncreaseMultiplier;
            moveStartingMultiplier = Mathf.Clamp(moveStartingMultiplier, 0, 1);
            yield return null;
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        inputMoveVector = Vector2.zero;
        StopCoroutine(moveStartingMultiplierCoroutine);
        moveStartingMultiplier = 0;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (m_stam.CurrentStamina >= jumpStaminaDrain
            && jumpTimer >= jumpCooldownDuration)
        {
            jumpTimer = 0;
            m_stam.DrainStamina(jumpStaminaDrain);
            moveDirection.y = m_jumpspeed;
            StartCoroutine(Jumping());
        }
    }

    private void OnPickupPerformed(InputAction.CallbackContext context)
    {
        if (mItemToPickup != null)
        {
            _Inventory.AddItem(mItemToPickup, (false, 0), true, false);
            Hud.CloseMessagePanel();
            mItemToPickup = null;
        }
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        isSprinting = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }

    private void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        DetermineCrouchState();
    }

    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        DetermineCrouchState();
    }

    private void DetermineCrouchState()
    {
        switch (CurrentCrouchState)
        {
            case CrouchState.NotCrouched:
                m_controller.center += crouchOffset;
                m_controller.height -= crouchHeightOffset;
                footStepsController.FootStepMultiplier = crouchFootStepsMultiplier;
                CurrentCrouchState = CrouchState.Crouched;
                break;

            case CrouchState.Crouched:
                m_controller.center -= crouchOffset;
                m_controller.height += crouchHeightOffset;
                footStepsController.FootStepMultiplier = footStepsController.DefaultFootStepMultiplier;
                CurrentCrouchState = CrouchState.NotCrouched;
                break;
        }
    }
    #endregion

    #region Ability Collision Detection
    // Public variable for collision detection / charge warrior ability
    public bool Collided { get; set; }

    private bool isCheckingCollisions;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isCheckingCollisions
            && m_controller.velocity.sqrMagnitude > 1)
        {
            isCheckingCollisions = true;
            StartCoroutine(CheckCollisions(hit));
        }
    }

    private IEnumerator CheckCollisions(ControllerColliderHit hit)
    {
        string hitName = hit.collider.name.ToLower();
        for (int i = 0; i < collisionNames.Length; i++)
        {
            if (hitName.Contains(collisionNames[i]))
            {
                Collided = true;
                yield return collisionDetectionWFS;
                Collided = false;
                break;
            }

            yield return null;
        }

        isCheckingCollisions = false;
    }
    #endregion

    public void Inventory_ItemUsed(object sender, InventoryEventArgs e)
    {
        IInventoryItem item = e.Item;
        if (item is null) return;

        if (item.ItemType == ItemType.HealthPotion)
        {
            if (m_PlayerHealth.m_CurrentHealth < m_PlayerHealth.m_HealthPoints)
            {
                MonoBehaviour healthDropMono = item as MonoBehaviour;
                if (healthDropMono != null)
                {
                    SAMI_HealthDrop healthDrop = healthDropMono.GetComponent<SAMI_HealthDrop>();
                    m_PlayerHealth.m_CurrentHealth += healthDrop.HealAmount;
                    _Inventory.RemoveItem(item);
                }
            }

            return;
        }

        if (Hand.transform.childCount == 0)
        {
            GameObject goItem = (item as MonoBehaviour).gameObject;
            goItem.SetActive(true);
            goItem.transform.parent = Hand.transform;
            if (item.ItemType == ItemType.Dagger) // Use a different item pos/rot for dagger so it aligns correctly in player's hand
            {
                goItem.transform.parent = DaggerPos.transform;
                goItem.transform.localPosition = daggerHandPosition;
                goItem.transform.localRotation = Quaternion.Euler(daggerHandRotation);
            }
            else
            {
                goItem.transform.localPosition = (item as InventoryItemBase).PickPosition;
                goItem.transform.localEulerAngles = (item as InventoryItemBase).PickRotation;
            }
        }
        else
        {
            Hand.transform.GetChild(0).gameObject.SetActive(false);
            Hand.transform.GetChild(0).transform.parent = null;

            GameObject goItem = (item as MonoBehaviour).gameObject;
            goItem.SetActive(true);
            goItem.transform.parent = Hand.transform;
            goItem.transform.localPosition = (item as InventoryItemBase).PickPosition;
            goItem.transform.localEulerAngles = (item as InventoryItemBase).PickRotation;
        }
    }

    private void OnTriggerStay(Collider other) // Item detection
    {
        if (other.TryGetComponent(out IInventoryItem item))
        {
            if (mLockPickup)
            {
                return;
            }

            if (!isCheckingItems)
            {
                StartCoroutine(CheckItemType(item));

                if (checkItemTypeCoroutine != null)
                {
                    StopCoroutine(checkItemTypeCoroutine);
                }

                checkItemTypeCoroutine = StartCoroutine(DisableMessagePanel());
            }
        }
    }

    private IEnumerator DisableMessagePanel()
    {
        yield return disableMessagePanelWFS;
        Hud.CloseMessagePanel();
    }

    private IEnumerator CheckItemType(IInventoryItem item)
    {
        isCheckingItems = true;
        ItemType[] allowedWeapons = m_SwordAttack.ActiveClass.GetAllowedItemTypes();
        CheckAllowedItem(item, allowedWeapons);
        yield return null;
        isCheckingItems = false;
    }

    private void CheckAllowedItem<T>(IInventoryItem item, T[] items) where T : Enum
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Equals(item.ItemType))
            {
                ActivatePickup(item);
                break;
            }
        }
    }

    private void ActivatePickup(IInventoryItem item)
    {
        mItemToPickup = item;
        Hud.OpenMessagePanel("");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInventoryItem _))
        {
            Hud.CloseMessagePanel();
            mItemToPickup = null;
        }
    }

    private void Update()
    {
        jumpTimer += Time.deltaTime;
        if (AbilityBase.IsExecutingAbility || levelPickerMenu.activeSelf) return; // During ability execution or ability picking, don't allow moving
        
        if (m_controller.isGrounded)
        {
            Vector3 inputValue = new Vector3(inputMoveVector.x, 0, inputMoveVector.y);
            Vector3 input = inputValue;
            moveDirection = inputValue;
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= CrouchStateSpeed();

            bool isMovingBackwards = IsMovingBackwards(input);
            footStepsController.IsMovingBackwards = isMovingBackwards;
            if (isMovingBackwards)
            {
                // Slower when moving backwards
                moveDirection *= backwardsMovingMultiplier;
            }
            else if (isJumping)
            {
                moveDirection.y = m_jumpspeed;
            }
        }
        else
        {
            moveDirection = new Vector3(inputMoveVector.x, moveDirection.y, inputMoveVector.y);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection.x *= m_speed;
            moveDirection.z *= m_speed;
            
        }

        AddGravity();
        Move(moveDirection * Time.deltaTime);

        if (isSprinting
            && m_stam.CurrentStamina >= 7.5f
            && !footStepsController.m_IsExhausted
            && !PlayerAttack.IsAttacking)
        {
            Vector3 sprintDirection = new Vector3(inputMoveVector.x, moveDirection.y, inputMoveVector.y);

            if (!IsMovingBackwards(sprintDirection)
                && !IsMovingSideways(sprintDirection))
            {
                SprintData runSprintData = new SprintData(sprintDirection, m_sprint, runSprintDrain, true, false, true);
                Sprint(runSprintData);
            }
        }
    }

    private bool IsMovingBackwards(Vector3 input)
    {
        return input.z < 0;
    }

    private bool IsMovingSideways(Vector3 input)
    {
        return input.x > 0.5f || input.x < -0.5f;
    }

    private float CrouchStateSpeed()
    {
        switch (CurrentCrouchState)
        {
            case CrouchState.NotCrouched:
                return m_speed;

            case CrouchState.Crouched:
                return crouchSpeed;
        }

        return 0;
    }

    public void Sprint(SprintData data)
    {
        moveDirection = data.Direction;
        if (data.TransformDirection)
        {
            moveDirection = transform.TransformDirection(moveDirection);
        }

        if (data.AddGravity)
        {
            AddGravity();
        }

        m_stam.DrainStamina(data.StaminaDrain);
        moveDirection.x *= data.SprintMultiplier;
        moveDirection.z *= data.SprintMultiplier;

        if (data.MultiplyByDelta)
        {
            Move(moveDirection * Time.deltaTime);
        }
        else
        {
            Move(moveDirection);
        }
    }

    private void Move(Vector3 motion)
    {
        motion *= moveStartingMultiplier;
        m_controller.Move(motion);
    }

    private void AddGravity()
    {
        moveDirection.y -= m_gravity * Time.deltaTime;
    }

    private IEnumerator Jumping()
    {
        PlayerAnimator.Instance.ResetAnimatorParamsExcept("Jumping");

        isJumping = true;
        yield return null;
        isJumping = false;

        yield return new WaitForSeconds(2);
        PlayerAnimator.Instance.ResetAnimatorParamsExcept("Idle");
    }

    private void OnDisable()
    {
        _Inventory.ItemUsed -= Inventory_ItemUsed;
        inputActions.InputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.InputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.InputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.InputActions.Player.Pickup.performed -= OnPickupPerformed;
        inputActions.InputActions.Player.Sprint.performed -= OnSprintPerformed;
        inputActions.InputActions.Player.Sprint.canceled -= OnSprintCanceled;
        inputActions.InputActions.Player.Crouch.performed -= OnCrouchPerformed;
        inputActions.InputActions.Player.Crouch.canceled -= OnCrouchCanceled;
    }
}