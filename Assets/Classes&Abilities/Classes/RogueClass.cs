using System.Collections;
using UnityEngine;

public class RogueClass : ClassBase
{
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
            ResetAnimatorParamsExcept("RogueStab");
            yield return new WaitForSeconds(mainAttackDisableDelay);
        }

        ResetAnimatorParamsExcept("Idle");
        PlayerAttack.IsAttacking = false;
    }

    protected override void MainAttackAudio()
    {
        // TODO
    }
}