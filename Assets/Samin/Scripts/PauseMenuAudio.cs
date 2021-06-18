using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseMenuAudio : MonoBehaviour
{
    public Slider AudioSlider;
    private AudioListener AudioListener;


    private void Update()
    {
        AudioSlider.value = AudioListener.volume;
    }
}
