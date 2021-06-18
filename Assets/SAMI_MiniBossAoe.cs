using UnityEngine;

public class SAMI_MiniBossAoe : MonoBehaviour
{
    [SerializeField]
    private float projectileSpeed = 90;
    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Invoke(nameof(SelfDestruct), 15f);
        rigidBody.AddForce(transform.forward * projectileSpeed, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}