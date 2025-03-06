using System;
using TMPro;
using UnityEngine;

public class FontChanger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private TMP_FontAsset comicSansFont;
    private TMP_FontAsset currentFont;
    private void OnEnable()
    {
        string currentFontString = PlayerPrefs.GetString(PlayerPrefsEnum.Font.ToString());
        if (currentFontString == PlayerPrefsEnum.FontDefault.ToString())
            currentFont = defaultFont;
        else if(currentFontString == PlayerPrefsEnum.FontComicSans.ToString())
            currentFont = comicSansFont;
        
        tmpText.font = currentFont;
    }
}
