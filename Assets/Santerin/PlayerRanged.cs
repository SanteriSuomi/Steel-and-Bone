using System.Collections;
using UnityEngine;

public class PlayerRanged : MonoBehaviour
{
    [SerializeField]
    private Transform rangedAttackStart = default;
    private WaitForSeconds mainAttackWaitForSeconds;
    [SerializeField]
    private float projectileSpeedMultiplier = 15;
    [SerializeField]
    private float shootCooldown = 2;
    private bool canShoot = true;

    private void Awake()
    {
        mainAttackWaitForSeconds = new WaitForSeconds(shootCooldown);
    }

    public void MainAttack()
    {
        if (canShoot)
        {
            StartCoroutine(MainAttackCoroutine());
        }
    }

    private IEnumerator MainAttackCoroutine()
    {
        canShoot = false;
        Projectile projectile = ProjectilePool.Instance.Get();
        projectile.transform.position = rangedAttackStart.position;
        projectile.transform.rotation = rangedAttackStart.rotation;
        projectile.Rigidbody.velocity = rangedAttackStart.forward * projectileSpeedMultiplier;
        yield return mainAttackWaitForSeconds;
        canShoot = true;
    }
}