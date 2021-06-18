using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider m_EnemyHealthBar = default;
    public Slider Slider => m_EnemyHealthBar;

    private IHasHealthbar m_Enemy;

    private void Awake()
        => InitializeHealthbar();

    private void InitializeHealthbar()
    {
        if (TryGetComponent(out IHasHealthbar enemy))
        {
            m_Enemy = enemy;
            m_EnemyHealthBar.maxValue = m_Enemy.Health;
            MonoBehaviour enemyAsMono = (MonoBehaviour)m_Enemy;
            if (enemyAsMono.gameObject.TryGetComponent(out EnemyBase enemyBase))
            {
                enemyBase.GiveHealthBar(this);
            }
            else if (enemyAsMono.gameObject.TryGetComponent(out BossRanged boss))
            {
                boss.GiveHealthBar(this);
                m_EnemyHealthBar.value = enemy.Health;
            }
        }
        else
        {
            Debug.LogWarning($"IEnemy not found in {gameObject.name}");
        }
    }

    private void LateUpdate()
    {
        if (m_EnemyHealthBar.isActiveAndEnabled)
        {
            m_EnemyHealthBar.transform.LookAt(EnemyGlobal.Instance.Player);
        }
    }

    public void Activate()
        => m_EnemyHealthBar.gameObject.SetActive(true);

    public void Deactivate()
        => m_EnemyHealthBar.gameObject.SetActive(false);

    public void UpdateValue()
        => m_EnemyHealthBar.value = m_Enemy.Health;
}