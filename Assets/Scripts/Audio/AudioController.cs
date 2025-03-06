using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] Slider volumeSlider;
    
    private float volume;

    private void Start()
    {
        volume = PlayerPrefs.GetFloat(PlayerPrefsEnum.MusicVolume.ToString());
        audioSource.volume = volume;
        
        if(volumeSlider != null)
            volumeSlider.value = audioSource.volume;
    }
    
    public void OnSliderVolumeChange(float volume)
    {
        PlayerPrefs.SetFloat(PlayerPrefsEnum.MusicVolume.ToString(), volume);
        PlayerPrefs.Save();
        audioSource.volume = volume;
    }
}
