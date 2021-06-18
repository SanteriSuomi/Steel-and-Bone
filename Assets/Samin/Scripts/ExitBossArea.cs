using UnityEngine;

public class ExitBossArea : MonoBehaviour
{
    public AudioSource m_BossMusic;
    public AudioClip m_NormalMusic;
    public Collider m_BossEnter;
    public Collider m_ExitBossArea;
    public AudioSource m_DungeonMusic;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        StartNormalMusic();
    //    }
    //}

    public void StartNormalMusic()
    {
        //m_DungeonMusic.PlayOneShot(m_NormalMusic);
        m_DungeonMusic.clip = m_NormalMusic;
        m_DungeonMusic.loop = true;
        m_DungeonMusic.Play();
        m_ExitBossArea.enabled = false;
        m_BossEnter.enabled = false;
        m_BossMusic.Stop();
    }
}