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

        // �e�{�^���̃N���b�N�C�x���g�ɑΉ����郁�\�b�h��o�^
        NoneButton.onClick.AddListener(() => OnButtonPressed(NoneButton));
        BlackButton.onClick.AddListener(() => OnButtonPressed(BlackButton));
        WhiteButton.onClick.AddListener(() => OnButtonPressed(WhiteButton));
        WallButton.onClick.AddListener(() => OnButtonPressed(WallButton));
        BlackButton.image.sprite =  PressImage;
    }

    private void OnButtonPressed(Button pressedButton)
    {
        // �����ꂽ�{�^����PressImage��ݒ肵�A����ȊO��NotPressImage�ɕύX
        NoneButton.image.sprite = (pressedButton == NoneButton) ? PressImage : NotPressImage;
        BlackButton.image.sprite = (pressedButton == BlackButton) ? PressImage : NotPressImage;
        WhiteButton.image.sprite = (pressedButton == WhiteButton) ? PressImage : NotPressImage;
        WallButton.image.sprite = (pressedButton == WallButton) ? PressImage : NotPressImage;
    }
}