/**
 * TabPanelManager
 * 
 * ボタンによってパネルを表示／非表示にします
 * 
 * Author: Kirurobo http://twitter.com/kirurobo
 * License: CC0 https://creativecommons.org/publicdomain/zero/1.0/
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabPanelManager : MonoBehaviour {

    [System.SerializableAttribute]
    public class TabPanel
    {
        public Button button;
        public RectTransform panel;
    }

    [SerializeField]
    public TabPanel[] tabPanels;

    public Color FocusedTabColor = new Color(1f, 1f, 1f, 0.75f);
    public Color UnfocusedTabColor = new Color(0.75f, 0.75f, 0.75f, 0.75f);

    private Vector2 panelPosition = Vector2.zero;


    // Use this for initialization
    void Start () {
        if (tabPanels.Length > 0)
        {
            // 最初のパネルの位置を記憶しておく
            if (tabPanels[0].panel)
            {
                panelPosition = tabPanels[0].panel.position;
            }
        }

        // イベントリスナーを準備
        int index = 0;
        foreach(var tab in tabPanels)
        {
            int tabPanelIndex = index + 0;  // indexをそのまま使うと最後の値になるようなので、ループ内のローカル変数を作成
            if (tab.button)
            {
                tab.button.onClick.AddListener(() => Select(tabPanelIndex));
            }

            if (tab.panel)
            {
                // 位置をすべて合わせる（エディタ上では異なる位置に並べて設計できるように、実行時に位置調整）
                tab.panel.position = panelPosition;
            }

            index++;
        }

        //  最初のパネルを選択状態にする
        Select(0);
    }
    
    // Update is called once per frame
    void Update () {
        
    }

    /// <summary>
    /// 指定番号のタブを表示し、他を隠す
    /// </summary>
    /// <param name="selectedIndex"></param>
    public void Select(int selectedIndex)
    {
        int index = 0;
        foreach (var tab in tabPanels)
        {
            if (index == selectedIndex)
            {
                if (tab.panel)
                {
                    tab.panel.gameObject.SetActive(true);
                    tab.panel.GetComponent<Image>().color = FocusedTabColor;
                }
                if (tab.button)
                {
                    tab.button.image.color = FocusedTabColor;
                    tab.button.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                }
            }
            else
            {
                if (tab.panel)
                {
                    tab.panel.gameObject.SetActive(false);
                    tab.panel.GetComponent<Image>().color = UnfocusedTabColor;
                }
                if (tab.button)
                {
                    tab.button.image.color = UnfocusedTabColor;
                    tab.button.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
                }
            }
            index++;
        }
    }

}
