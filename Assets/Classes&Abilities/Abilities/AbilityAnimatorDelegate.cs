using UnityEngine;

/// <summary>
/// This script is a middleman for abilities that want to access player's animator.
/// </summary>
public class AbilityAnimatorDelegate : MonoBehaviour
{
	[SerializeField] private Transform m_HeadPosition = default;
	[SerializeField] private Transform m_OrgParent = default;
	private WarriorAbility warriorAbility;
	private PlayerAttack m_SwordAttack;
    private SAMI_EnemyDamage m_Enemydamage;
    private SAMI_PlayerHealth m_PlayerHP;
	private Camera m_MainCamera;
	public Vector3 m_OriginalCameraRotation;
	public Vector3 m_OriginalCameraPosition;
	public AudioClip m_ComboSounds;
	public AudioClip m_SlamSound;
	[SerializeField]
	private Collider m_FistCollider = default;
	[SerializeField]
	private Vector3 comboCameraMoveAmount = new Vector3(0, 0, 0.3f);
    public AudioSource m_KickSource;
	[SerializeField]
	private Collider m_SwordTrigger = default;
    private Collider m_RogueAbility1 = default;
    private bool canMoveComboCamera = true;
    [SerializeField] private Collider m_RogueStun;

	private void Awake()
	{
		warriorAbility = FindObjectOfType<WarriorAbility>();
		m_SwordAttack = FindObjectOfType<PlayerAttack>();
		m_MainCamera = Camera.main;
		m_OriginalCameraRotation = -transform.localRotation.eulerAngles;
		m_OriginalCameraPosition = m_MainCamera.transform.localPosition;
        m_Enemydamage = FindObjectOfType<SAMI_EnemyDamage>();
        m_PlayerHP = FindObjectOfType<SAMI_PlayerHealth>();
		warriorAbility.SetAudioSource(GetComponent<AudioSource>()); // Give the player ability audiosource to ability scripts
        m_RogueStun = GameObject.Find("RogueStun").GetComponent<Collider>();
        m_RogueAbility1 = GameObject.Find("RogueAssasinationTrigger").GetComponent<Collider>();
	}

	#region Warrior Animator Events
	public void ChangeCameraPosition()
	{
		m_MainCamera.transform.SetParent(m_HeadPosition);
	}

    public void ChangeCameraPositionToOrginal()
    {
        m_MainCamera.transform.SetParent(m_OrgParent);
        m_MainCamera.transform.localPosition = m_OriginalCameraPosition;
        m_MainCamera.transform.localRotation = Quaternion.Euler(m_OriginalCameraRotation);
    }

	public void WarriorSlam()
	{
		warriorAbility.WarriorSlam();
	}

	public void StopWarriorSlamAnim()
	{
		m_MainCamera.transform.SetParent(m_OrgParent);
		m_MainCamera.transform.localRotation = Quaternion.Euler(m_OriginalCameraRotation);
		m_MainCamera.transform.localPosition = m_OriginalCameraPosition;
		PlayerAttack.IsAttacking = false;
	}

	public void WarriorSlamSound()
	{
		warriorAbility.PlayAudioClip(m_SlamSound, 0.85f, 0.5f, 0.75f);
	}

	public void WarriorComboMoveCamera()
	{
		if (canMoveComboCamera)
		{
			canMoveComboCamera = false;
			m_MainCamera.transform.localPosition += comboCameraMoveAmount;
		}
	}

	public void WarriorComboMoveCameraBack()
	{
		m_MainCamera.transform.localPosition -= comboCameraMoveAmount;
		Invoke(nameof(CanMoveAgain), 0.5f);
	}

	private void CanMoveAgain()
	{
		canMoveComboCamera = true;
	}

	public void WarriorComboSounds()
	{
		warriorAbility.PlayAudioClip(m_ComboSounds, 0.5f, 0.55f, 1);
	}  
    
    public void IsBlocking()
    {
        PlayerAttack.IsBlocking = true;
    }

    public void IsntBlocking()
    {
        PlayerAttack.IsBlocking = false;        
    }

	public void EnablePlayerSwordTrigger()
	{
		m_SwordTrigger.enabled = true;
	}

	public void DisablePlayerSwordTrigger()
	{
		m_SwordTrigger.enabled = false;
	}
    #endregion

    #region Rogue Animator Events
    //public void EnableWeaponCollider()
    //{

    //}

    //public void DisableWeaponCollider()
    //{

    //}
    public void EnablePlayerRogueTrigger()
    {
        m_RogueAbility1.enabled = true;
    }

    public void DisablePlayerRogueTrigger()
    {
        m_RogueAbility1.enabled = false;
    }

    public void EnableStunCollider()
    {
        m_RogueStun.enabled = true;
    }

    public void DisableStunCollider()
    {
        m_RogueStun.enabled = false;
    }
    #endregion

    #region Global Animator Events
    public void ActivatePunchTrigger()
	{
		m_FistCollider.enabled = true;
	}

	public void DeActivatePunchTrigger()
	{
		m_FistCollider.enabled = false;
	}

    public void KickAudio()
    {        
        m_KickSource.Play();
    }

    public void StopKickAudio()
    {
        m_KickSource.Stop();
    }

    protected static void ResetAnimatorParamsExcept(string nameOfParameterToEnable)
    {
        PlayerAnimator.Instance.ResetAnimatorParamsExcept(nameOfParameterToEnable);
    }
    #endregion
}