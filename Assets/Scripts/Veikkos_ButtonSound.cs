using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Veikkos_ButtonSound : MonoBehaviour
{
    public AudioClip m_sound;
    private Button m_button { get { return GetComponent<Button>(); } }
    private AudioSource m_source { get { return GetComponent<AudioSource>(); } }

    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        m_source.clip = m_sound;
        m_source.playOnAwake = false;
        m_button.onClick.AddListener(() => PlaySound());
    }

    public void PlaySound()
    {
        if (m_source.isActiveAndEnabled)
        {
            m_source.PlayOneShot(m_sound);
        }
    }
}
