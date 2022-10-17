using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioSource Music;
    [SerializeField] private List<AudioSource> Sounds;
    [SerializeField] private Slider MusicSlider, SoundsSlider;
    void Start()
    {
        Music.volume = PlayerPrefs.GetFloat("VolumeMusic");
        if(Music.volume == 0) Music.enabled = false;
        else Music.enabled = true;
        MusicSlider.value = PlayerPrefs.GetFloat("VolumeMusic");

        foreach(AudioSource sound in Sounds){
            sound.volume = PlayerPrefs.GetFloat("VolumeSounds");
            if(sound.volume == 0) sound.enabled = false;
            else sound.enabled = true;
        }
        SoundsSlider.value = PlayerPrefs.GetFloat("VolumeSounds");
    }

    public void SetVolumeMusic(){
        PlayerPrefs.SetFloat("VolumeMusic", MusicSlider.value);
        Music.volume = PlayerPrefs.GetFloat("VolumeMusic");
        if(MusicSlider.value==0 && Music.enabled) Music.enabled = false;
        else if(MusicSlider.value!=0 && !Music.enabled) Music.enabled = true;
        PlayerPrefs.Save();
    }

    
    public void SetVolumeSounds(){
        PlayerPrefs.SetFloat("VolumeSounds", SoundsSlider.value);
        foreach(AudioSource sound in Sounds){
            sound.volume = PlayerPrefs.GetFloat("VolumeSounds");
            if(SoundsSlider.value==0 && sound.enabled) sound.enabled = false;
            else if(SoundsSlider.value!=0 && !sound.enabled) sound.enabled = true;
        }
        PlayerPrefs.Save();
    }
}
