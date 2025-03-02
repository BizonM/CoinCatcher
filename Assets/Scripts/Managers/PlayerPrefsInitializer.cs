using UnityEngine;

public class PlayerPrefsInitializer : MonoBehaviour
{
    void Start()
    {
        InitializePlayerPrefs();
    }

    private void InitializePlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("Initialized"))
        {
            PlayerPrefs.SetInt("Initialized", 1);
            PlayerPrefs.SetFloat("MusicVolume", 1f);
            PlayerPrefs.Save();
        }
        
    }
}
