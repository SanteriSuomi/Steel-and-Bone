using UnityEngine;

public class BossAreaMusic : MonoBehaviour
{
    public AudioSource m_BossAreaSource;
    public AudioClip m_BossMusic;
    public Collider m_BossEnter;
    public Collider m_ExitBossArea;
    public AudioSource m_DungeonMusic;
    public AudioClip m_NormalMusic;
    public AudioSource m_StartMusic;

    private void Awake()
    {
        m_StartMusic.Play();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        ActivateBossMusic();
    //    }
    //}

    public void ActivateBossMusic()
    {
        m_StartMusic.Stop();
        //m_BossAreaSource.PlayOneShot(m_BossMusic);
        m_BossAreaSource.clip = m_BossMusic;
        m_BossAreaSource.loop = true;
        m_BossAreaSource.Play();
        m_ExitBossArea.enabled = true;
        m_BossEnter.enabled = false;
        m_DungeonMusic.Stop();
    }
}