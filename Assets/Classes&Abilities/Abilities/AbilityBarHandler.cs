using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBarHandler : MonoBehaviour
{
    private PlayerAttack m_PlayerAttack;
    private SAMI_PlayerStats m_Levels;

    [SerializeField] private GameObject m_Abilitybar = default;
    [SerializeField] private RawImage[] m_Abilities = default;

    [Header("Warrior Icons")]
    [SerializeField] private Texture[] m_WarriorIcons = default;
    [SerializeField] private Texture[] m_GreyWarriorIcons = default;

    [Header("Rogue Icons")]
    [SerializeField] private Texture[] m_RogueIcons = default;
    [SerializeField] private Texture[] m_GreyRogueIcons = default;

    private void Awake()
    {
        m_PlayerAttack = FindObjectOfType<PlayerAttack>();
        m_Levels = FindObjectOfType<SAMI_PlayerStats>();
    }

    public void UpdateAbilityBar()
    {
        switch (m_PlayerAttack.ActiveClass)
        {
            case WarriorClass _:
                SetIcons(m_WarriorIcons[0], m_WarriorIcons[1], m_WarriorIcons[2], 
                         m_GreyWarriorIcons[0], m_GreyWarriorIcons[1], m_GreyWarriorIcons[2]);
                break;

            case RogueClass _:
                SetIcons(m_RogueIcons[0], m_RogueIcons[1], m_RogueIcons[2], 
                         m_GreyRogueIcons[0], m_GreyRogueIcons[1], m_GreyRogueIcons[2]);
                break;

            case RangedClass _:
                break;
        }
    }

    public void SetIcons(Texture colorIcon1, Texture colorIcon2, Texture colorIcon3,
                         Texture greyIcon1, Texture greyIcon2, Texture greyIcon3)
    {
        if (m_Levels.m_CurrentLevel >= 2)
        {
            m_Abilities[0].texture = colorIcon1;
        }
        else
        {
            m_Abilities[0].texture = greyIcon1;
        }

        if (m_Levels.m_CurrentLevel >= 3)
        {
            m_Abilities[1].texture = colorIcon2;
        }
        else
        {
            m_Abilities[1].texture = greyIcon2;
        }

        if (m_Levels.m_CurrentLevel >= 4)
        {
            m_Abilities[2].texture = colorIcon3;
        }
        else
        {
            m_Abilities[2].texture = greyIcon3;
        }
    }

    #region Show/Hide Abilitybar
    public void ShowAbilityBar()
    {
        m_Abilitybar.SetActive(true);
        m_Abilitybar.transform.DOScale(1, 0.45f)
                .SetRecyclable(true)
                .SetEase(Ease.OutExpo);
    }

    public void HideAbilityBar()
    {
        m_Abilitybar.transform.DOScale(0, 0.45f)
                .SetRecyclable(true)
                .SetEase(Ease.Flash)
                .OnComplete(() => m_Abilitybar.SetActive(false));
    }
    #endregion

    #region Change Ability colors with cooldown
    public void SetCooldownAbilityGrey(int index)
    {
        switch (m_PlayerAttack.ActiveClass)
        {
            case WarriorClass _:
                m_Abilities[index].texture = m_GreyWarriorIcons[index];
                break;

            case RogueClass _:
                m_Abilities[index].texture = m_GreyRogueIcons[index];
                break;

            case RangedClass _:
                break;
        }
    }

    public void SetCooldownAbilityColor(int index)
    {
        switch (m_PlayerAttack.ActiveClass)
        {
            case WarriorClass _:
                m_Abilities[index].texture = m_WarriorIcons[index];
                break;

            case RogueClass _:
                m_Abilities[index].texture = m_RogueIcons[index];
                break;

            case RangedClass _:
                break;
        }
    }
    #endregion
}