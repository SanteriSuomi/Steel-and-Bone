using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }
    [SerializeField]
    private float spawnDeactivationDelay = 10;
    [SerializeField]
    private float projectileDamage = 10;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Invoke(nameof(Deactivate), spawnDeactivationDelay);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null 
            && !collision.collider.CompareTag("Player"))
        {
            if (collision.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(projectileDamage);
            }

            Invoke(nameof(Deactivate), spawnDeactivationDelay / 2);
        }
    }

    private void Deactivate()
    {
        ProjectilePool.Instance.Return(this);
    }
}