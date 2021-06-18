using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RogueAbility : AbilityBase
{

    [SerializeField]
    private int stunAbilityCooldown = 12;
    [SerializeField]
    private int assasinationAbilityCooldown = 10;
    [SerializeField]
    private int teleportAbilityCooldown = 10;

    [SerializeField]
    private float assasinationStaminaDrain = 25;
    [SerializeField]
    private float stunStaminaDrain = 30;
    [SerializeField]
    private float teleportStaminaDrain = 50;


    private float distance = 200f;

    public override void UseAbilityLevel2()
    {
        if (NotNoughtStamina(assasinationStaminaDrain)) return;
        StartCoroutine(RogueAssasination());
        StartLevel2Cooldown(assasinationAbilityCooldown);
    }

    private IEnumerator RogueAssasination()
    {
        PlayerAttack.IsAttacking = true;
        PlayerAnimator.Instance.ResetAnimatorParamsExcept("RogueAssasination");
        m_Stam.DrainStamina(25);
        Invoke(nameof(StopAbility1), 1.8f);
        yield return null;
    }

    private void StopAbility1()
    {
        PlayerAttack.IsAttacking = false;
        PlayerAnimator.Instance.ResetAnimatorParamsExcept("Idle");
    }
    public override void UseAbilityLevel3()
    {
        if (NotNoughtStamina(stunStaminaDrain)) return;
        StartCoroutine(RogueStun());
        StartLevel3Cooldown(stunAbilityCooldown);
    }

    private IEnumerator RogueStun()
    {
        PlayerAttack.IsAttacking = true;
        PlayerAnimator.Instance.ResetAnimatorParamsExcept("RogueStun");
        m_Stam.DrainStamina(30);
        Invoke(nameof(StopStun), 1.3f);
        yield return null;
    }

    private void StopStun()
    {
        PlayerAttack.IsAttacking = false;
        PlayerAnimator.Instance.ResetAnimatorParamsExcept("Idle");
    }

    public override void UseAbilityLevel4()
    {
        if (NotNoughtStamina(teleportStaminaDrain)) return;
        StartLevel4Cooldown(teleportAbilityCooldown);
        Transform player = EnemyGlobal.Instance.Player;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit _hit, distance) && _hit.collider.tag == "Enemy")
        {                     
            Transform enemy = _hit.transform;
            Vector3 teleportPosition = enemy.position + Vector3.forward * 2;
            if(NavMesh.SamplePosition(teleportPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                m_Stam.DrainStamina(50);
                playerController.enabled = false;
                player.SetPositionAndRotation(hit.position, enemy.rotation);
                playerController.enabled = true;
            }            
        }
    }

    //Fix Rogue assasination ability

    //private void TeleportBehind()
    //{

    //}


}