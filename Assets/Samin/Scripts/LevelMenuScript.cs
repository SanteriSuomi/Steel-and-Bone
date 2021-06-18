using UnityEngine;
using UnityEngine.UI;

public class LevelMenuScript : MonoBehaviour
{
	public Text m_AttackLevel;
	public Text m_DefenceLevel;
	public SAMI_PlayerStats m_PlayerStats;
	public int m_RegisterButtonClick;
	public int m_MaxButtonClick = 5;
	public StaminaBar m_StaminaScript;
	public GameObject m_LevelSelector;
	public Text m_SkillsLeft;
	public int m_AbilityCounter;
	public bool m_SkipTutorial = false;
	public Aleksi_CursorHide AleksinScripti;
	private Tutorial m_Tutorial;
	[SerializeField] Camera m_MainCamera = default;    
	//[SerializeField] Camera m_LevelPickerCamera;
	[SerializeField] private GameObject healthAndStaminaBars = default;
	[SerializeField]
	private QuestDisplayController questDisplayController = default;

	private void Awake()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;
		m_AbilityCounter = 5;
		m_AttackLevel.text = "Attack Level: 1";
		m_DefenceLevel.text = "Defence Level: 1";
		m_Tutorial = FindObjectOfType<Tutorial>();
	}

	private void Start()
	{
		// Set health and stamina bars disabled in start so scripts can find it at awake
		healthAndStaminaBars.SetActive(false);
	}

	public void OnAttackPlus()
	{
		if (m_RegisterButtonClick < m_MaxButtonClick)
		{
			m_PlayerStats.m_AttackLevel++;
			m_RegisterButtonClick++;
			m_AttackLevel.text = "Attack Level: " + m_PlayerStats.m_AttackLevel;
			m_AbilityCounter--;
		}
	}

	private void Update()
	{
		if (m_RegisterButtonClick < 0)
		{
			m_RegisterButtonClick = 0;
		}

		m_SkillsLeft.text = "Ability Points Left: " + m_AbilityCounter;
	}

	//public void onAttackMinus()
	//{
	//	if (m_RegisterButtonClick < m_MaxButtonClick)
	//	{
	//		m_PlayerStats.m_AttackLevel--;
	//		m_RegisterButtonClick--;
	//		m_AttackLevel.text = "Attack Level: " + m_PlayerStats.m_AttackLevel;
	//	}
	//}

	public void onDefencePlus()
	{
		if (m_RegisterButtonClick < m_MaxButtonClick)
		{
			m_PlayerStats.m_DefenceLevel++;
			m_RegisterButtonClick++;
			m_DefenceLevel.text = "Defence Level: " + m_PlayerStats.m_DefenceLevel;
			m_AbilityCounter--;
		}
	}

	//public void onDefenceMinus()
	//{
	//	if (m_RegisterButtonClick < m_MaxButtonClick)
	//	{
	//		m_PlayerStats.m_DefenceLevel--;
	//		m_RegisterButtonClick--;
	//		m_DefenceLevel.text = "Defence Level: " + m_PlayerStats.m_DefenceLevel;
	//	}
	//}

	public void Confirm()
	{
		m_LevelSelector.SetActive(false);
		//Cursor.visible = false;
		//Cursor.lockState = CursorLockMode.Locked;
		AleksinScripti.LevelStats = true;
		m_MainCamera.gameObject.SetActive(true);
		//m_LevelPickerCamera.gameObject.SetActive(false);
		healthAndStaminaBars.SetActive(true);
		questDisplayController.ActivateQuestDisplay();

		if (!m_SkipTutorial)
		{
            m_Tutorial.StartTutorial();
		}
	}

	public void SkipTutorial()
	{
		//m_Tutorial.StartTutorial();
		m_Tutorial.SkipTutorial();
		m_SkipTutorial = true;
	}

	public void Reset()
	{
		m_PlayerStats.m_DefenceLevel = 1;
		m_PlayerStats.m_AttackLevel = 1;
		m_AbilityCounter = 5;
		m_RegisterButtonClick = 0;
		m_DefenceLevel.text = "Defence Level: " + m_PlayerStats.m_DefenceLevel;
		m_AttackLevel.text = "Attack Level: " + m_PlayerStats.m_AttackLevel;
	}
}