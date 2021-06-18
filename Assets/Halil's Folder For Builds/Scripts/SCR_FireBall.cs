using UnityEngine;

public class SCR_FireBall : MonoBehaviour
{
    [SerializeField]
    private float speed = 110;
    private Transform player;
    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        player = EnemyGlobal.Instance.Player;
    }

    private void Start()
    {
        Invoke(nameof(SelfDestruct), 5);
        rigidBody.AddRelativeForce((player.position - transform.position + Vector3.up / 2) * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}