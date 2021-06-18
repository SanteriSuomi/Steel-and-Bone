using System.Collections;
using UnityEngine;

public class RangedClass : ClassBase
{
    private PlayerRanged playerRanged;

    private void Awake() => playerRanged = FindObjectOfType<PlayerRanged>();

    protected override IEnumerator MainAttack()
    {
        DrainAttackStamina();
        PlayerAttack.IsAttacking = true;
        if (InventoryItemBase.CurrentlyEquippedItem == ItemType.None)
        {
            ResetAnimatorParamsExcept("Punch");
            yield return new WaitForSeconds(PUNCH_ATTACK_DELAY);
        }
        else
        {
            // TODO add ranged attack anims etc
            playerRanged.MainAttack();
            //int m_RandomAttack = Random.Range(0, 2);
            //if (m_RandomAttack == 0)
            //{
            //    ResetAnimatorParamsExcept("hasAttacked");
            //}
            //else if (m_RandomAttack == 1)
            //{
            //    ResetAnimatorParamsExcept("SecondaryAttack");
            //}
            yield return new WaitForSeconds(mainAttackDisableDelay);
        }

        ResetAnimatorParamsExcept("Idle");
        PlayerAttack.IsAttacking = false;
    }

    protected override void MainAttackAudio()
    {
        //TODO ranged attack audio
    }
}