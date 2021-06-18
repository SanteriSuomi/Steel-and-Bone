using System.Collections;
using UnityEngine;

public class FootStepAudioController : MonoBehaviour
{
	public CharacterController m_CC;
	public AudioSource m_FootStep;
	public AudioClip m_FootStep1;
	public AudioClip m_FootStep2;
	public float m_StepInterval = 5f;
	private float m_StepCycle;
	private float m_NextStep;
	public bool m_Step1HasPlayed = false;
	public bool m_Step2HasPlayed = true;
	public StaminaBar m_StaminaBar;
	public AudioSource m_AudioExhaustedSource;
	public AudioClip m_ExhaustedAudio;
	public bool m_IsExhausted = false;

	[Header("Walk")]
	[SerializeField]
	private float footStepWalkMinVolume = 0.15f;
	[SerializeField]
	private float footStepWalkMaxVolume = 0.35f;
	[SerializeField]
	private float footStepWalkMinPitch = 0.85f;
	[SerializeField]
	private float footStepWalkMaxPitch = 1;

	[Header("Run")]
	[SerializeField]
	private float footStepRunMinVolume = 0.35f;
	[SerializeField]
	private float footStepRunMaxVolume = 0.55f;
	[SerializeField]
	private float footStepRunMinPitch = 0.95f;
	[SerializeField]
	private float footStepRunMaxPitch = 1.45f;

	[Header("Walk Backwards")]
	[SerializeField]
	private float footStepBackwardsMinVolume = 0.1f;
	[SerializeField]
	private float footStepBackwardsMaxVolume = 0.3f;
	[SerializeField]
	private float footStepBackwardsMinPitch = 0.8f;
	[SerializeField]
	private float footStepBackwardsMaxPitch = 0.95f;

	[SerializeField]
	private float defaultFootStepMultiplier = 1;
	public float DefaultFootStepMultiplier { get => defaultFootStepMultiplier; }
	public float FootStepMultiplier { get; set; } = 1;
	public bool IsMovingBackwards { get; set; }

	[SerializeField]
	private float footStepRunTriggerMaxVelocity = 6;
	[SerializeField]
	private float delayFootStepRunTime = 0.45f;
	private WaitForSeconds delayFootStepRun;
	[SerializeField]
	private float delayFootStepWalkTime = 0.65f;
	private WaitForSeconds delayFootStepWalk;
	private PlayerCharacterController charControllerScript;

	private void Awake()
	{
		delayFootStepRun = new WaitForSeconds(delayFootStepRunTime);
		delayFootStepWalk = new WaitForSeconds(delayFootStepWalkTime);
		charControllerScript = GetComponentInParent<PlayerCharacterController>();
	}

	private void Start()
	{
		m_StepCycle = 0f;
		m_NextStep = m_StepCycle / 2f;
		StartCoroutine(WalkRunExhaustedAudio());
	}

	private IEnumerator WalkRunExhaustedAudio()
	{
		while (enabled)
		{
			CheckExhaustion();

			float charControllerVelocitySqrMagnitude = m_CC.velocity.sqrMagnitude;
			bool isMoving = Mathf.Abs(charControllerVelocitySqrMagnitude) >= 0.1f;
			if (!m_IsExhausted && isMoving)
			{
				// Backwards
				if (IsMovingBackwards)
				{
					SetAudioSourceAndPlayAudio(footStepBackwardsMinVolume,
											   footStepBackwardsMaxVolume,
											   footStepBackwardsMinPitch,
											   footStepBackwardsMaxPitch);
					yield return delayFootStepWalk;
				}
				// Run
				else if (charControllerVelocitySqrMagnitude <= footStepRunTriggerMaxVelocity
					   && charControllerScript.CurrentCrouchState != CrouchState.Crouched)
				{
					SetAudioSourceAndPlayAudio(footStepRunMinVolume,
											   footStepRunMaxVolume,
											   footStepRunMinPitch,
											   footStepRunMaxPitch);
					yield return delayFootStepRun;
				}
				// Walk/Crouch
				else if (charControllerVelocitySqrMagnitude > footStepRunTriggerMaxVelocity
						|| charControllerScript.CurrentCrouchState == CrouchState.Crouched)
				{
					SetAudioSourceAndPlayAudio(footStepWalkMinVolume,
											   footStepWalkMaxVolume,
											   footStepWalkMinPitch,
											   footStepWalkMaxPitch);
					yield return delayFootStepWalk;
				}
			}

			yield return null;
		}
	}

	private void CheckExhaustion()
	{
		if (m_StaminaBar.CurrentStamina <= 7.5f && !m_IsExhausted)
		{
			m_IsExhausted = true;
			m_AudioExhaustedSource.volume = Random.Range(0.8f, 1f);

			if (m_AudioExhaustedSource.isPlaying)
			{
				m_AudioExhaustedSource.Stop();
			}

			m_AudioExhaustedSource.PlayOneShot(m_ExhaustedAudio);
			Invoke(nameof(PlayExhausted), 2.32f);
		}
		else if (m_StaminaBar.CurrentStamina > 13.5f)
		{
			m_AudioExhaustedSource.Stop();
		}
	}

	private void SetAudioSourceAndPlayAudio(float minVolume, float maxVolume, float minPitch, float maxPitch)
	{
		minVolume *= FootStepMultiplier;
		maxVolume *= FootStepMultiplier;

		m_FootStep.volume = Random.Range(minVolume, maxVolume);
		m_FootStep.pitch = Random.Range(minPitch, maxPitch);
		m_FootStep.PlayOneShot(m_FootStep1);
	}

	public void PlayExhausted()
	{
		m_IsExhausted = false;
	}
}