using System.Collections;
using UnityEngine;

public class WarriorClass : ClassBase
{
    protected override IEnumerator MainAttack()
    {
        //ResetAnimatorParamsExcept(string.Empty); // Reset all params so attack executed correctly each time
        DrainAttackStamina();
        PlayerAttack.IsAttacking = true;
        if (InventoryItemBase.CurrentlyEquippedItem == ItemType.None)
        {
            ResetAnimatorParamsExcept("Punch");
            yield return new WaitForSeconds(PUNCH_ATTACK_DELAY);
        }
        else
        {
            int m_RandomAttack = Random.Range(0, 2);
            if (m_RandomAttack == 0)
            {
                ResetAnimatorParamsExcept("hasAttacked");
            }
            else if (m_RandomAttack == 1)
            {
                ResetAnimatorParamsExcept("SecondaryAttack");
            }

            yield return new WaitForSeconds(mainAttackDisableDelay);
        }

        ResetAnimatorParamsExcept("Idle");
        PlayerAttack.IsAttacking = false;
    }

    protected override void MainAttackAudio()
    {
        //if (!PlayerAnimator.Instance.Animator.GetBool("hasAttacked")
        //    || !PlayerAnimator.Instance.Animator.GetBool("SecondaryAttack"))
        //{
        //    return;
        //}

        playerAttack.m_AudioSource.volume = Random.Range(0.8f, 1f);
        playerAttack.m_AudioSource.pitch = Random.Range(0.8f, 1.3f);
        playerAttack.m_AudioSource.Play();
    }
}