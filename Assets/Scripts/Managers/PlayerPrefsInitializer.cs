using UnityEngine;

public class PlayerPrefsInitializer : MonoBehaviour
{
    void Start()
    {
        InitializePlayerPrefs();
    }

    private void InitializePlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsEnum.Initialized.ToString()))
        {
            PlayerPrefs.SetInt(PlayerPrefsEnum.Initialized.ToString(), 1);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.MusicVolume.ToString(), 1f);
            PlayerPrefs.SetString(PlayerPrefsEnum.Font.ToString(),
                PlayerPrefsEnum.FontDefault.ToString());
            PlayerPrefs.Save();
        }
        
    }
}
