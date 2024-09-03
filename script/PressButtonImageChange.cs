using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressButtonImageChange : MonoBehaviour
{
    public Button NoneButton;
    public Button BlackButton;
    public Button WhiteButton;
    public Button WallButton;
    public Sprite PressImage;
    public Sprite NotPressImage;
    

    // Start is called before the first frame update
    void Start()
    {

        #region 各ボタンのクリックイベントに対応するメソッドを登録
        NoneButton.onClick.AddListener(() => OnButtonPressed(NoneButton));
        BlackButton.onClick.AddListener(() => OnButtonPressed(BlackButton));
        WhiteButton.onClick.AddListener(() => OnButtonPressed(WhiteButton));
        WallButton.onClick.AddListener(() => OnButtonPressed(WallButton));
        BlackButton.image.sprite =  PressImage;
        #endregion
    }

    private void OnButtonPressed(Button pressedButton)
    {
        #region 押されたボタンにPressImageを設定し、それ以外はNotPressImageに変更
        NoneButton.image.sprite = (pressedButton == NoneButton) ? PressImage : NotPressImage;
        BlackButton.image.sprite = (pressedButton == BlackButton) ? PressImage : NotPressImage;
        WhiteButton.image.sprite = (pressedButton == WhiteButton) ? PressImage : NotPressImage;
        WallButton.image.sprite = (pressedButton == WallButton) ? PressImage : NotPressImage;
        #endregion
    }
    
}
