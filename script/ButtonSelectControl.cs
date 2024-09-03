using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectControl : MonoBehaviour
{
    private GameObject CurrentSelectObj;

    void Update()
    {
         #region　マウスクリックにより現在選択中のボタンのトリガーが外れた際に元に戻す
        if (Input.GetMouseButtonDown(0)) EventSystem.current.SetSelectedGameObject(CurrentSelectObj);
        else CurrentSelectObj = EventSystem.current.currentSelectedGameObject;
        #endregion
    }
}
