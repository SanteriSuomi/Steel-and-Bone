using Essentials.Saving;
using MessagePack;
using System;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour, ISaveable
{
	private float m_startsStamina = 100;
	public float CurrentStamina { get; set; }
	public Slider m_sliderStamina;
	private GameObject levelPickerMenu;
	[SerializeField]
	private float staminaDrainMultiplier = 10;
	[SerializeField]
	private float staminaRegenMultiplier = 8.5f;
	private bool IsPaused => Mathf.Abs(Time.timeScale) <= 0.01f;

	private void Awake()
	{
		SaveSystem.Register(this);

		CurrentStamina = m_startsStamina;
		m_sliderStamina = GameObject.Find("StaminaBar").GetComponent<Slider>();
		levelPickerMenu = GameObject.Find("LevelPicker");
	}

	#region Saving
	public SaveData GetSave()
	{
	    return new PlayerStaminaData(gameObject.name)
		{
			stamina = CurrentStamina,
			staminaSliderValue = m_sliderStamina.value,
			staminaDrainMultiplier = staminaDrainMultiplier,
			staminaRegenMultiplier = staminaRegenMultiplier
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is PlayerStaminaData save)
		{
			CurrentStamina = save.stamina;
			m_sliderStamina.value = save.staminaSliderValue;
			staminaDrainMultiplier = save.staminaDrainMultiplier;
			staminaRegenMultiplier = save.staminaRegenMultiplier;
		}
	}

	[Serializable, MessagePackObject]
	public class PlayerStaminaData : SaveData
	{
		[Key("Stamina")]
		public float stamina;
		[Key("StaminaSliderValue")]
		public float staminaSliderValue;
		[Key("StaminaDrainMultiplier")]
		public float staminaDrainMultiplier;
		[Key("StaminaRegenMultiplier")]
		public float staminaRegenMultiplier;

		public PlayerStaminaData() { }

		public PlayerStaminaData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion

	private void Update()
	{
		if (!IsPaused
			&& !levelPickerMenu.activeSelf
			&& !AbilityBase.IsExecutingAbility)
		{
			CurrentStamina = Mathf.Clamp(CurrentStamina, 0, 100);
			RegenStamina();
		}
	}

	private void RegenStamina()
	{
		CurrentStamina += 0.2f * staminaRegenMultiplier * Time.deltaTime;
		m_sliderStamina.value = CurrentStamina;
	}

	public void DrainStamina(float amount)
	{
		if (levelPickerMenu.activeSelf) return;
		CurrentStamina -= amount * staminaDrainMultiplier * Time.deltaTime;
		m_sliderStamina.value = CurrentStamina;
	}
}