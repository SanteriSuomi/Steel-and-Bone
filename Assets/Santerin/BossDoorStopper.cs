using UnityEngine;

public class BossDoorStopper : MonoBehaviour
{
    [SerializeField]
    private Animator bossDoor1 = default;
    [SerializeField]
    private Animator bossDoor2 = default;
    private Collider bossDoor1Collider;
    private Collider bossDoor2Collider;
    [SerializeField]
    private float doorCloseSpeedMultiplier = 2.75f;
    private float animatorRegularSpeed;

    public bool IsInBossRoom { get => !bossDoor1.GetBool("OpenBossDoor"); }

    private void Awake()
    {
        bossDoor1Collider = bossDoor1.gameObject.GetComponent<Collider>();
        bossDoor2Collider = bossDoor2.gameObject.GetComponent<Collider>();
        animatorRegularSpeed = bossDoor1.speed;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            bossDoor1.speed *= doorCloseSpeedMultiplier;
            bossDoor2.speed *= doorCloseSpeedMultiplier;
            bossDoor1.SetBool("OpenBossDoor", false);
            bossDoor2.SetBool("OpenBossDoor", false);
            bossDoor1Collider.enabled = true;
            bossDoor2Collider.enabled = true;
            Invoke(nameof(SetRegularSpeed), 2.5f);
        }
    }

    private void SetRegularSpeed()
    {
        bossDoor1.speed = animatorRegularSpeed;
        bossDoor2.speed = animatorRegularSpeed;
    }
}