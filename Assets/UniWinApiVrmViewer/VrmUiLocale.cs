using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class VrmUiLocale : MonoBehaviour {

    [System.Serializable]
    public class LocaleText
    {
        public Dictionary<string, string> Labels;
        public Dictionary<string, string> Buttons;
    }

    private LocaleText _localeText;

    public void SetLocale(string lang = "en")
    {
        var path = Application.streamingAssetsPath + "/i18n/" + lang + ".json";
        if (!File.Exists(path)) return;
        var json = File.ReadAllText(path);
        _localeText = JsonUtility.FromJson<LocaleText>(json);

        UpdateText(_localeText);
    }

    private void UpdateText(LocaleText localeText)
    {
        if (localeText == null) return;

    }

    public void SaveLocale(string lang = "en")
    {
        
    }
}
