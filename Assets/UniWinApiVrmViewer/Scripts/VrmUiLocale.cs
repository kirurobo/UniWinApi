/**
 * UI internationalization
 *
 * Author: Kirurobo
 *
 * Original author: m2wasabi, https://github.com/m2wasabi/VRMLoaderUI 
 * Original license: MIT License https://github.com/m2wasabi/VRMLoaderUI/blob/master/LICENSE 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI要素の多言語対応
/// </summary>
public class VrmUiLocale : MonoBehaviour
{

    private LocaleText _localeText;

    private Transform targetTransform;   // 対象のUI要素の親


    /// <summary>
    /// 言語ファイルを読み込んで適用
    /// </summary>
    /// <param name="lang"></param>
    public void SetLocale(string lang = "en")
    {
        var path = Application.streamingAssetsPath + "/i18n/" + lang + ".json";
        if (!File.Exists(path)) return;
        var json = File.ReadAllText(path);
        _localeText = JsonUtility.FromJson<LocaleText>(json);

        UpdateText(_localeText);
    }

    /// <summary>
    /// 開始時の処理
    /// </summary>
    void Awake()
    {
        // 親要素指定がなければ自分自身の子にUI要素があるものとする
        if (!targetTransform)
        {
            targetTransform = this.transform;
        }
    }

    /// <summary>
    /// 各UI要素に指定言語のテキストを適用
    /// </summary>
    /// <param name="localeText"></param>
    private void UpdateText(LocaleText localeText)
    {
        if (localeText == null || !targetTransform) return;

        var transforms = targetTransform.GetComponentsInChildren<Transform>(true);

        SetText(ref transforms, "TransparentToggle", localeText.labels.Transparent);
        SetText(ref transforms, "TopmostToggle", localeText.labels.Topmost);
        SetText(ref transforms, "MaximizeToggle", localeText.labels.Maximize);
        SetText(ref transforms, "ZoomModeDropdown", localeText.labels.ZoomMode);
        SetText(ref transforms, "TransparentTypeDropdown", localeText.labels.TransparentType);
        SetText(ref transforms, "HitTestTypeDropdown", localeText.labels.HitTestType);
        SetText(ref transforms, "LanguageDropdown", localeText.labels.Language);
        SetText(ref transforms, "MotionDropdown", localeText.labels.Motion);
        SetText(ref transforms, "MotionModeText", localeText.labels.Motion);
        SetText(ref transforms, "MotionTogglePreset", localeText.labels.Preset);
        SetText(ref transforms, "MotionToggleRandom", localeText.labels.Random);
        SetText(ref transforms, "MotionToggleBvh", localeText.labels.Bvh);
        SetText(ref transforms, "FaceDropdown", localeText.labels.Face);
        SetText(ref transforms, "FaceModeText", localeText.labels.Face);
        SetText(ref transforms, "FaceToggleRandom", localeText.labels.Random);

        SetText(ref transforms, "OpenButton", localeText.buttons.Open);
        SetText(ref transforms, "QuitButton", localeText.buttons.Quit);
        SetText(ref transforms, "NextButton", localeText.buttons.Next);
        SetText(ref transforms, "BackButton", localeText.buttons.Back);

        SetText(ref transforms, "TabButtonModel", localeText.buttons.TabButtonModel);
        SetText(ref transforms, "TabButtonMotion", localeText.buttons.TabButtonMotion);
        SetText(ref transforms, "TabButtonConfig", localeText.buttons.TabButtonConfig);
        SetText(ref transforms, "TabButtonAbout", localeText.buttons.TabButtonAbout);
    }

    /// <summary>
    /// 指定の名前のUIについてテキストをセット
    /// </summary>
    /// <param name="transforms"></param>
    /// <param name="name"></param>
    /// <param name="text"></param>
    private void SetText(ref Transform[] transforms, string name, string text)
    {
        foreach (Transform trans in transforms)
        {
            if (trans && (trans.name == name))
            {
                Text textComponent;
                Dropdown dropdown = trans.gameObject.GetComponent<Dropdown>();
                if (dropdown)
                {
                    // Dropdown ならば Text という名前の子要素を対象とする
                    textComponent = trans.Find("Text").gameObject.GetComponent<Text>();
                }
                else
                {
                    // Dropdown 以外ならばそのまま Text コンポーネントを探す
                    textComponent = trans.GetComponentInChildren<Text>(true);
                }

                if (textComponent)
                {
                    textComponent.text = text;
                }
                break;
            }
        }
    }

    [System.Serializable]
    public struct Labels
    {
        public string Motion, Face;
        public string Transparent, Topmost, Maximize;
        public string Language, ZoomMode, TransparentType, HitTestType, None, Preset, Random, Bvh, Repeat;
    }

    [System.Serializable]
    public struct Buttons
    {
        public string Quit, Open, Back, Next;
        public string TabButtonModel, TabButtonMotion, TabButtonConfig, TabButtonAbout;
    }

    [System.Serializable]
    public class LocaleText
    {
        public Labels labels;
        public Buttons buttons;
    }
}