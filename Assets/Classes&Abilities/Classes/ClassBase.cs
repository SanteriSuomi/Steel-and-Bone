using System.Collections;
using UnityEngine;

public enum ClassType
{
    Warrior,
    Rogue,
    Ranged
}

public abstract class ClassBase : MonoBehaviour
{
    [SerializeField] [Tooltip("Used in serialization")]
    private ClassType classType = default;
    public ClassType ClassType => classType;
    [SerializeField]
    private AbilityBase abilityToActivate = default;
    [SerializeField]
    private AudioClip punchAttackAudioClip = default;
    protected GameObject classPickerCanvas;
    protected PlayerAttack playerAttack;
    protected StaminaBar stamina;
    [SerializeField]
    private float mainAttackAudioDelay = 1;
    [SerializeField]
    private float secondaryAttackAudioDelay = 1;
    [SerializeField]
    protected float mainAttackDisableDelay = 2.5f;
    protected const float PUNCH_ATTACK_DELAY = 1;
    protected const float PUNCH_ATTACK_AUDIO_DELAY = 0.4f;

    [SerializeField]
    private ItemType[] allowedItemTypes = default;
    public ItemType[] GetAllowedItemTypes() => allowedItemTypes;

    //[SerializeField]
    //private PickupType[] allowedPickups = default;
    //public PickupType[] GetAllowedPickupTypes() => allowedPickups;

    private void Start()
    {
        classPickerCanvas = GameObject.Find("ClassPickerCanvas");
        playerAttack = FindObjectOfType<PlayerAttack>();
        stamina = playerAttack.m_StaminaBar;
    }

    // Activate class
    public void Activate()
    {
        abilityToActivate.Activate(); // Activate the corresponding abilities
        classPickerCanvas.SetActive(false); // Disable class picker canvas
        OnActivate();
    }

    protected virtual void OnActivate() => playerAttack.ActiveClass = this;

    public void UseMainAttack()
    {
        if (InventoryItemBase.CurrentlyEquippedItem == ItemType.None)
        {
            Invoke(nameof(PunchAttackAudio), PUNCH_ATTACK_AUDIO_DELAY);
        }
        else
        {
            Invoke(nameof(MainAttackAudio), mainAttackAudioDelay);
        }

        StartCoroutine(MainAttack());
    }
    protected abstract IEnumerator MainAttack();

    public void UseSecondaryAttack()
    {
        Invoke(nameof(SecondaryActionAudio), secondaryAttackAudioDelay);
        if(stamina.CurrentStamina >= 15f)
        {
            SecondaryAction();
        }
    }

    public void SecondaryAction()
    {
        PlayerAttack.IsAttacking = true;
        stamina.DrainStamina(15f);
        ResetAnimatorParamsExcept("Block");
        Invoke(nameof(ChangeAttackState), 1.0f);
    }

    public void ChangeAttackState()
    {
        PlayerAttack.IsAttacking = false;
    }

    public void UseKickAttack() => StartCoroutine(KickAttack());
    protected virtual IEnumerator KickAttack()
    {
        DrainAttackStamina();
        ResetAnimatorParamsExcept("Kick");
        PlayerAttack.IsAttacking = true;
        playerAttack.m_FootCollider.enabled = true;
        Invoke(nameof(DisableFootCollider), 1.325f);
        playerAttack.m_Movement.m_speed = 0;

        yield return new WaitForSeconds(1.325f);

        ResetAnimatorParamsExcept("Idle");
        PlayerAttack.IsAttacking = false;
        playerAttack.m_Movement.m_speed = playerAttack.m_Movement.DefaultSpeed;
    }

    private void DisableFootCollider()
    {
        playerAttack.m_FootCollider.enabled = false;
    }

    protected abstract void MainAttackAudio();

    protected virtual void PunchAttackAudio()
    {
        playerAttack.m_AudioSource.volume = Random.Range(0.8f, 1f);
        playerAttack.m_AudioSource.pitch = Random.Range(0.8f, 1.2f);
        playerAttack.m_AudioSource.PlayOneShot(punchAttackAudioClip);
    }

    protected virtual void SecondaryActionAudio()
    {
    }

    protected void DrainAttackStamina()
        => playerAttack.m_StaminaBar.DrainStamina(playerAttack.attackStaminaDrain);

    protected static void ResetAnimatorParamsExcept(string nameOfParameterToEnable)
    {
        PlayerAnimator.Instance.ResetAnimatorParamsExcept(nameOfParameterToEnable);
    }
}