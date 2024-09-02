using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableLayerHandler : MonoBehaviour
{
    // レイヤーマスクを設定
    public GameObject[] buttons; // ターゲットにしたいボタンの配列
    public KeyCode[] keys; // 各ボタンに対応するキーの配列

    private int currentIndex = 0;
    void Start()
    {
        Cursor.visible = false;

    }

    void Update()
    {
        // キーボード操作でターゲットを変更
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                currentIndex = i;
                EventSystem.current.SetSelectedGameObject(buttons[currentIndex]);
                return;
            }
        }

        // マウス操作などでターゲットが外れた場合、ボタンをターゲットに戻す
        if (EventSystem.current.currentSelectedGameObject != buttons[currentIndex])
        {
            EventSystem.current.SetSelectedGameObject(buttons[currentIndex]);
        }
    }

    

    
}
