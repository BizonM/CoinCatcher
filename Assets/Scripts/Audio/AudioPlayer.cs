using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] clips;
    [SerializeField] Slider volumeSlider;
    
    private float volume;

    private void Start()
    {
        volume = PlayerPrefs.GetFloat("AudioVolume");
        audioSource.volume = volume;
        if(volumeSlider != null)
            volumeSlider.value = audioSource.volume;
        
        //StartCoroutine(PlayAudioClipsAndLastInLoop());
    }
    
    public void OnSliderVolumeChange(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        audioSource.volume = volume;
        PlayerPrefs.Save();
    }

    private IEnumerator PlayAudioClipsAndLastInLoop()
    {
        if (clips != null)
        {
            for (int i = 0; i < clips.Length; i++)
            {
                audioSource.clip = clips[i];
                audioSource.Play();
                yield return new WaitForSeconds(clips[i].length);
                if (i == clips.Length - 1)
                    audioSource.loop = true;
            }
        }
    }
}
