using DG.Tweening;
using Essentials.Saving;
using MessagePack;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour, ISaveable
{
    private LevelMenuScript m_LevelMenu;
    [SerializeField] private GameObject m_Wall = default;
    private Vector3 wallOriginalPos;
    [SerializeField] private Text m_TutorialText = default;
    private bool pressedW = false;
    private bool pressedS = false;
    private bool pressedA = false;
    private bool pressedD = false;
    private bool m_CompletedFirst = false;
    private bool m_CompletedSecond = false;
    private bool m_CompletedThird = false;
    private bool m_CompletedFourth = false;
    private bool m_CompletedFifth = false;
    private bool m_CompletedSixth = false;
    //private bool m_CompletedSeventh = false;
    private bool m_CompletingTutorial = false;
    private bool m_CompletedLast = false;
    [SerializeField] GameObject m_Sword = default;
    [SerializeField] GameObject m_Dagger = default;
    [SerializeField]
    private GameObject spawnHealthPotion = default;
    [SerializeField] AudioSource m_AudioSource = default;
    private PlayerAttack playerCombatController;
    private DownMover downMover;

    [SerializeField]
    private InputActionsVar inputActions = default;

    private bool isEndingTutorial;
    //private WaitForFixedUpdate wallCloseFixedUpdate;

    private void Awake()
    {
        SaveSystem.Register(this);

        wallOriginalPos = m_Wall.transform.position;
        playerCombatController = FindObjectOfType<PlayerAttack>();
        m_LevelMenu = FindObjectOfType<LevelMenuScript>();
        //wallCloseFixedUpdate = new WaitForFixedUpdate();
        m_Wall.GetComponent<AudioSource>();
        downMover = m_Wall.GetComponent<DownMover>();
    }

    private void Start()
    {
        // Reset tutorial quest to false before game start, because it mysteriously becomes sometimes.
        CompleteTutorialQuest(false);
    }

    #region Saving
    public SaveData GetSave()
    {
        return new TutorialData(gameObject.name)
        {
            completedFifth = m_CompletedFifth
        };
    }

    public void Load(SaveData saveData)
    {
        if (saveData is TutorialData save)
        {
            Invoke(nameof(CompleteTutorialQuest), 0.5f);

            m_Wall.SetActive(false);
            m_TutorialText.gameObject.SetActive(false);
            if (spawnHealthPotion != null)
            {
                spawnHealthPotion.SetActive(false);
            }

            if (save.completedFifth) // If completed fifth stage, player already has a weapon.
            {
                m_Sword.SetActive(false);
                m_Dagger.SetActive(false);
            }
            else
            {
                m_Sword.SetActive(true);
                m_Dagger.SetActive(true);
            }
        }
    }

    [Serializable, MessagePackObject]
    public class TutorialData : SaveData
    {
        [Key("CompletedFifth")]
        public bool completedFifth;

        public TutorialData() { }

        public TutorialData(string objName)
        {
            this.objName = objName;
        }
    }
    #endregion

    public void SkipTutorial()
    {
        StartCoroutine(EndTutorialCoroutine(0));
    }

    public void StartTutorial()
    {
        m_CompletedFirst = false;
        m_CompletedSecond = false;
        m_CompletedThird = false;
        m_CompletedFourth = false;
        m_CompletedFifth = false;
        m_CompletedSixth = false;
        TutorialPhaseOne();
    }

    private void TutorialPhaseOne()
    {
        m_CompletingTutorial = true;
        m_TutorialText.text = "Firstly, let's try moving with the W, A, S and D keys.";
        m_Sword.SetActive(false);
        m_Dagger.SetActive(false);
    }

    private void Update()
    {
        if (m_CompletingTutorial)
        {
            TutorialPhaseTwo();
        }
    }

    private void TutorialPhaseTwo()
    {
        bool eKeyPressed = (Keyboard.current?.eKey.wasPressedThisFrame == true)
                            || (Gamepad.current?.buttonWest.wasPressedThisFrame == true);
        bool attackKeyPressed = (Mouse.current?.leftButton.wasPressedThisFrame == true)
                                 || (Gamepad.current?.leftShoulder.wasPressedThisFrame == true);

        if ((inputActions.CurrentInputDirection & InputActionsVar.InputDirection.Up) != 0 && m_CompletingTutorial)
        {
            pressedW = true;
        }
        if ((inputActions.CurrentInputDirection & InputActionsVar.InputDirection.Left) != 0 && m_CompletingTutorial)
        {
            pressedA = true;
        }
        if ((inputActions.CurrentInputDirection & InputActionsVar.InputDirection.Down) != 0 && m_CompletingTutorial)
        {
            pressedS = true;
        }
        if ((inputActions.CurrentInputDirection & InputActionsVar.InputDirection.Right) != 0 && m_CompletingTutorial)
        {
            pressedD = true;
        }

        if (pressedA && pressedD && pressedS && pressedW && m_CompletingTutorial && !m_CompletedFirst)
        {
            m_CompletedFirst = true;
            pressedA = false;
            pressedD = false;
            pressedS = false;
            pressedW = false;
            m_TutorialText.text = "Great! Now let's try sprinting by holding W and then Left Shift key at the same time.";
        }
        else if ((inputActions.CurrentInputDirection & InputActionsVar.InputDirection.Up) != 0
                  && ((Keyboard.current?.leftShiftKey.wasPressedThisFrame == true)
                  || (Gamepad.current?.leftStick.IsPressed() == true))
                  && m_CompletedFirst && m_CompletingTutorial
                  && !m_CompletedSecond)
        {

            m_CompletedSecond = true;
            m_TutorialText.text = "Great! Let's move on to attacking. Try attacking by pressing the Left Click. This is the basic attack.";
        }
        else if (((Mouse.current?.leftButton.wasPressedThisFrame == true)
                  || (Gamepad.current?.leftShoulder.wasPressedThisFrame == true))
                  && m_CompletedSecond && m_CompletingTutorial && !m_CompletedThird)
        {
            m_CompletedThird = true;
            EnableWeaponsBasedOnClass();
            m_TutorialText.text = "Now let's pick up a weapon. Different weapons do different damages. Walk up to the table and press the E key when prompted.";
        }

        else if (InventoryItemBase.CurrentlyEquippedItem == ItemType.None && m_CompletedThird && m_CompletingTutorial
                 && eKeyPressed
                 && !m_CompletedFourth)
        {
            m_CompletedFourth = true;
            m_TutorialText.text = "Good job! Now press I to open up your inventory and equip the weapon you just picked up by pressing F1-F5.";
        }
        else if (InventoryItemBase.CurrentlyEquippedItem != ItemType.None && m_CompletedFourth && m_CompletingTutorial && !m_CompletedLast && !m_CompletedFifth)
        {
            m_CompletedFifth = true;
            m_TutorialText.text = "You can also pickup certain objects with the E key such as the bottle on the another table.";
        }
        else if (PlayerAttack.IsCarryingThrowable && m_CompletedFifth && !m_CompletedSixth)
        {
            m_CompletedSixth = true;
            m_TutorialText.text = "You can throw object with Left Click or just drop it by pressing E again.";
        }
        else if ((eKeyPressed || attackKeyPressed) && m_CompletedSixth && m_CompletingTutorial)
        {
            SwitchToLastText();
        }
    }

    private void EnableWeaponsBasedOnClass()
    {
        if (playerCombatController.ActiveClass is WarriorClass)
        {
            m_Sword.SetActive(true);
            m_Dagger.SetActive(false);
        }
        else if (playerCombatController.ActiveClass is RogueClass)
        {
            m_Dagger.SetActive(true);
            m_Sword.SetActive(false);
        }
    }

    private void SwitchToLastText()
    {
        m_TutorialText.text = "Finally, you're ready to enter. Remember, when you level up, you unlock skills that can be activated from keys 1-3.";
        m_CompletedLast = true;
        if (isActiveAndEnabled)
        {
            StartCoroutine(EndTutorialCoroutine(7.5f));
        }
    }

    private IEnumerator EndTutorialCoroutine(float hideTextTime)
    {
        if (isEndingTutorial) yield break;
        isEndingTutorial = true;
        EnableWeaponsBasedOnClass();

        Invoke(nameof(HideText), hideTextTime);
        CompleteTutorialQuest(true);
        m_AudioSource.Play();

        if (downMover.gameObject.activeSelf)
        {
            downMover.StartCoroutine(downMover.Move(m_Wall.transform, -6, 9, Ease.Linear,
            () => m_AudioSource.Play(),
            () =>
            {
                m_AudioSource.Stop();
                m_Wall.SetActive(false);
            }));
        }
    }

    private void CompleteTutorialQuest()
    {
        QuestContext tutorialQuest = QuestManager.Instance.GetQuest(id: -1);
        tutorialQuest.Data.IsCompleted = true;
    }

    private static void CompleteTutorialQuest(bool complete)
    {
        QuestContext tutorialQuest = QuestManager.Instance.GetQuest(id: -1);
        tutorialQuest.Data.IsCompleted = complete;
    }

    private void HideText()
    {
        m_CompletingTutorial = false;
        m_TutorialText.enabled = false;
    }
}