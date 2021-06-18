using UnityEngine;

public class ShootFireBall : MonoBehaviour
{
    public GameObject fireBall;
   
    public void FireBall()
    {
        Instantiate(fireBall, transform.position, Quaternion.identity);
    }
}