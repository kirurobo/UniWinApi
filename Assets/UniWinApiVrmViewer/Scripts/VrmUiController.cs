using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kirurobo;
using UnityEngine.Serialization;

public class VrmUiController : MonoBehaviour
{
    /// <summary>
    /// セーブ情報のバージョン番号
    /// </summary>
    const float prefsVersion = 0.02f;

    public WindowController windowController;

    public RectTransform panel;
    public Text informationText;
    public Text warningText;
    public Button closeButton;
    public Toggle transparentToggle;
    public Toggle topmostToggle;
    public Toggle maximizeToggle;
    public Button openButton;
    public Button quitButton;
    public Text titleText;
    
    [FormerlySerializedAs("zoomModeDropdown")] public Dropdown zoomTypeDropdown;
    [FormerlySerializedAs("transparentMethodDropdown")] public Dropdown transparentTypeDropdown;
    public Dropdown languageDropdown;

    public Toggle motionTogglePreset;
    public Toggle motionToggleRandom;
    public Toggle motionToggleBvh;
    public Toggle faceToggleRandom;

    public Button tabButtonModel;
    public Button tabButtonControl;
    public RectTransform modelPanel;
    public RectTransform controlPanel;
    public CameraController.ZoomType zoomType { get; set; }
    public UniWinApi.TransparentTypes transparentType { get; set; }
    int language { get; set; }

    private float mouseMoveSS = 0f;             // Sum of mouse trajectory squares. [px^2]
    private float mouseMoveSSThreshold = 16f;   // Threshold to be regarded as not moving. [px^2]
    private Vector3 lastMousePosition;

    private Vector2 originalAnchoredPosition;
    private Canvas canvas;

    private VRMLoader.VRMPreviewLocale vrmLoaderLocale;
    private VRMLoader.VRMPreviewUI vrmLoaderUI;
    private VrmUiLocale uiLocale;

    private TabPanelManager tabPanelManager;

    /// <summary>
    /// ランダムモーションが有効かを取得／設定
    /// </summary>
    public bool enableRandomMotion
    {
        get
        {
            if (motionToggleRandom) return motionToggleRandom.isOn;
            return false;
        }
        set
        {
            if (motionToggleRandom) motionToggleRandom.isOn = value;
        }
    }

    /// <summary>
    /// ランダム表情が有効かを取得／設定
    /// </summary>
    public bool enableRandomEmotion
    {
        get
        {
            if (faceToggleRandom) return faceToggleRandom.isOn;
            return false;
        }
        set
        {
            if (faceToggleRandom) faceToggleRandom.isOn = value;
        }
    }

    public VrmCharacterBehaviour.MotionMode motionMode
    {
        get
        {
            if (motionToggleRandom && motionToggleRandom.isOn) return VrmCharacterBehaviour.MotionMode.Random;
            if (motionToggleBvh && motionToggleBvh.isOn) return VrmCharacterBehaviour.MotionMode.Bvh;
            return 0;
        }
        set
        {
            if (value == VrmCharacterBehaviour.MotionMode.Random)
            {
                motionTogglePreset.isOn = false;
                motionToggleRandom.isOn = true;
                motionToggleBvh.isOn = false;
            }
            else if (value == VrmCharacterBehaviour.MotionMode.Bvh)
            {
                motionTogglePreset.isOn = false;
                motionToggleRandom.isOn = false;
                motionToggleBvh.isOn = true;
            }
            else
            {
                motionTogglePreset.isOn = true;
                motionToggleRandom.isOn = false;
                motionToggleBvh.isOn = false;
            }
        }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        if (!canvas)
        {
            canvas = GetComponent<Canvas>();
        }

        zoomType = CameraController.ZoomType.Zoom;
        transparentType = UniWinApi.TransparentTypes.Alpha;

        windowController = FindObjectOfType<WindowController>();
        if (windowController)
        {
            windowController.OnStateChanged += windowController_OnStateChanged;

            transparentType = windowController.transparentType;
        }

        vrmLoaderLocale = this.GetComponentInChildren<VRMLoader.VRMPreviewLocale>();
        vrmLoaderUI = this.GetComponentInChildren<VRMLoader.VRMPreviewUI>();
        uiLocale = this.GetComponentInChildren<VrmUiLocale>();
        tabPanelManager = this.GetComponentInChildren<TabPanelManager>();

        // 中央基準にする
        panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(0.5f, 0.5f);
        originalAnchoredPosition = panel.anchoredPosition;

        // Load settings.
        Load();

        // Initialize toggles.
        UpdateUI();

        // Set event listeners.
        if (closeButton) { closeButton.onClick.AddListener(Close); }
        if (quitButton) { quitButton.onClick.AddListener(Quit); }

        if (windowController)
        {
            // プロパティをバインド
            if (transparentToggle) { transparentToggle.onValueChanged.AddListener(windowController.SetTransparent); }
            if (maximizeToggle) { maximizeToggle.onValueChanged.AddListener(windowController.SetMaximized); }
            if (topmostToggle) { topmostToggle.onValueChanged.AddListener(windowController.SetTopmost); }
        }

        // 直接バインドしない項目の初期値とイベントリスナーを設定
        if (zoomTypeDropdown)
        {
            zoomTypeDropdown.value = (int)zoomType;
            zoomTypeDropdown.onValueChanged.AddListener(val => SetZoomType(val));
        }
        if (transparentTypeDropdown)
        {
            transparentTypeDropdown.value = (int)transparentType;
            transparentTypeDropdown.onValueChanged.AddListener(val => SetTransparentType(val));
        }
        if (languageDropdown)
        {
            languageDropdown.value = language;
            languageDropdown.onValueChanged.AddListener(val => SetLanguage(val));
        }

        // Show menu on startup.
        Show(null);
    }

    public void Save()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("Version", prefsVersion);

        if (windowController)
        {
            PlayerPrefs.SetInt("Transparent", windowController.isTransparent ? 1 : 0);
            PlayerPrefs.SetInt("Maximized", windowController.isMaximized ? 1 : 0);
            PlayerPrefs.SetInt("Topmost", windowController.isTopmost ? 1 : 0);
        }

        PlayerPrefs.SetInt("ZoomType", (int)zoomType);
        PlayerPrefs.SetInt("TransparentType", (int)transparentType);
        PlayerPrefs.SetInt("Language", language);

        PlayerPrefs.SetInt("VrmCharacterBehaviour.MotionMode", (int)motionMode);
        PlayerPrefs.SetInt("EmotionMode", enableRandomEmotion ? 1 : 0);
    }

    public void Load()
    {
        //// セーブされた情報のバージョンが異なれば読み出さない
        //if (PlayerPrefs.GetFloat("Version") != prefsVersion) return;

        int defaultTransparentTypeNo = 1;
        int defaultZoomTypeNo = 0;
        int defaultLanguageNo = 0;

        // UIで設定されている値を最初にデフォルトとする
        if (zoomTypeDropdown) defaultZoomTypeNo = zoomTypeDropdown.value;
        if (transparentTypeDropdown) defaultTransparentTypeNo = transparentTypeDropdown.value;
        if (languageDropdown) defaultLanguageNo = languageDropdown.value;

        if (windowController)
        {
            windowController.isTransparent = LoadPrefsBool("Transparent", windowController.isTransparent);
            windowController.isMaximized = LoadPrefsBool("Maximized", windowController.isMaximized);
            windowController.isTopmost = LoadPrefsBool("Topmost", windowController.isTopmost);

            // WindowControllerの値をデフォルトとする
            defaultTransparentTypeNo = (int)windowController.transparentType;
        }
        SetZoomType(PlayerPrefs.GetInt("ZoomType", defaultZoomTypeNo));
        SetTransparentType(PlayerPrefs.GetInt("TransparentType", defaultTransparentTypeNo));
        SetLanguage(PlayerPrefs.GetInt("Language", defaultLanguageNo));

        motionMode = (VrmCharacterBehaviour.MotionMode)PlayerPrefs.GetInt("VrmCharacterBehaviour.MotionMode", (int)motionMode);
        enableRandomEmotion = LoadPrefsBool("EmotionMode", enableRandomEmotion);
    }

    private bool LoadPrefsBool(string name, bool currentVal)
    {
        int pref = PlayerPrefs.GetInt(name, -1);
        if (pref < 0) return currentVal;    // データがないか-1なら元の値のまま
        return (pref > 0);    // そうでなければ 0:false , 1以上:true を返す
    }

    /// <summary>
    /// マウスホイールでのズーム方法を選択
    /// </summary>
    /// <param name="no">選択肢の番号（Dropdownを編集したら下記も要編集）</param>
    private void SetZoomType(int no)
    {
        if (no == 1)
        {
            zoomType = CameraController.ZoomType.Dolly;
        }
        else
        {
            zoomType = CameraController.ZoomType.Zoom;
        }
    }

    /// <summary>
    /// ウィンドウ透過方式を選択
    /// </summary>
    /// <param name="no">選択肢の番号（Dropdownを編集したら下記も要編集）</param>
    private void SetTransparentType(int no)
    {
        if (no == 1)
        {
            transparentType = UniWinApi.TransparentTypes.Alpha;
        }
        else if (no == 2)
        {
            transparentType = UniWinApi.TransparentTypes.ColorKey;
        }
        else
        {
            transparentType = UniWinApi.TransparentTypes.None;
        }
    }

    /// <summary>
    /// UI言語選択
    /// </summary>
    /// <param name="no">選択肢の番号（Dropdownを編集したら下記も要編集）</param>
    private void SetLanguage(int no)
    {
        string lang;
        switch (no)
        {
            case 0:
                lang = "en";
                language = 0;
                break;
            case 1:
                lang = "ja";
                language = 1;
                break;
            default:
                lang = "en";
                language = 0;
                break;
        }

        if (vrmLoaderLocale) vrmLoaderLocale.SetLocale(lang);
        if (uiLocale) uiLocale.SetLocale(lang);
    }

    private void windowController_OnStateChanged()
    {
        UpdateUI();
    }

    /// <summary>
    /// UIの状況を現在のウィンドウ状態に合わせて更新
    /// </summary>
    public void UpdateUI()
    {
        if (!windowController) return;

        if (transparentToggle) { transparentToggle.isOn = windowController.isTransparent; }
        if (maximizeToggle) { maximizeToggle.isOn = windowController.isMaximized; }
        if (topmostToggle) { topmostToggle.isOn = windowController.isTopmost; }
    }


    /// <summary>
    /// メニューを閉じる
    /// </summary>
    private void Close()
    {
        panel.gameObject.SetActive(false);
        //Debug.Log("Close. Zoom:" + zoomMode + ", Trans.:" + transparentMethod + ", Lang.:" + language);
    }

    /// <summary>
    /// 終了ボタンが押された時の処理。エディタ上であれば再生停止とする。
    /// </summary>
    private void Quit()
    {
#if UNITY_EDITOR
        // Stop playing for the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit application for the standalone player
        Application.Quit();
#endif
    }

    void OnApplicationQuit()
    {
        // 終了時には設定を保存する
        Save();
        //Debug.Log("Saved. Zoom:" + zoomMode + ", Trans.:" + transparentMethod + ", Lang.:" + language);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // マウス右ボタンクリックでメニューを表示させる。閾値以下の移動ならクリックとみなす。
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            mouseMoveSS += (Input.mousePosition - lastMousePosition).sqrMagnitude;
        }
        if (Input.GetMouseButtonUp(1))
        {
            if (mouseMoveSS < mouseMoveSSThreshold)
            {
                Show(lastMousePosition);
            }
            mouseMoveSS = 0f;
        }

        // [ESC] でメニューを閉じる
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    /// <summary>
    /// フォーカスが外れたときの処理
    /// </summary>
    /// <param name="focus"></param>
    private void OnApplicationFocus(bool focus)
    {
        // フォーカスが外れたらメニューを閉じる
        if (!focus)
        {
            Close();
        }
    }

    /// <summary>
    /// 座標を指定してメニューを表示する
    /// </summary>
    /// <param name="mousePosition"></param>
    public void Show(Vector2 mousePosition)
    {
        if (panel)
        {
            Vector2 pos = mousePosition;
            float w = panel.rect.width;
            float h = panel.rect.height;

            pos.x += Mathf.Max(w * 0.5f - pos.x, 0f);   // 左にはみ出していれば右に寄せる
            pos.y += Mathf.Max(h * 0.5f - pos.y, 0f);   // 下にはみ出していれば上に寄せる
            pos.x -= Mathf.Max(pos.x - Screen.width + w * 0.5f, 0f);    // 右にはみ出していれば左に寄せる
            pos.y -= Mathf.Max(pos.y - Screen.height + h * 0.5f, 0f);   // 上にはみ出していれば下に寄せる

            panel.anchorMin = Vector2.zero;
            panel.anchorMax = Vector2.zero;
            panel.anchoredPosition = pos;
            panel.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// メニューを表示する
    /// </summary>
    public void Show()
    {
        if (panel)
        {
            // 中央基準にして表示
            panel.anchorMin = panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.anchoredPosition = originalAnchoredPosition;
            panel.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Show the meta information
    /// </summary>
    /// <param name="meta"></param>
    public void Show(VRM.VRMMetaObject meta)
    {
        if (meta)
        {
            if (vrmLoaderUI) vrmLoaderUI.setMeta(meta);
            if (tabPanelManager) tabPanelManager.Select(0); // 0番がモデル情報のパネルという前提
        }

        Show();
    }

    /// <summary>
    /// Set the warning text
    /// </summary>
    /// <param name="message"></param>
    public void SetWarning(string message)
    {
        if (warningText)
        {
            warningText.text = message;
        }
    }
}
