using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableLayerHandler : MonoBehaviour
{
    // ���C���[�}�X�N��ݒ�
    public GameObject[] buttons; // �^�[�Q�b�g�ɂ������{�^���̔z��
    public KeyCode[] keys; // �e�{�^���ɑΉ�����L�[�̔z��

    private int currentIndex = 0;
    void Start()
    {
        Cursor.visible = false;

    }

    void Update()
    {
        // �L�[�{�[�h����Ń^�[�Q�b�g��ύX
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                currentIndex = i;
                EventSystem.current.SetSelectedGameObject(buttons[currentIndex]);
                return;
            }
        }

        // �}�E�X����ȂǂŃ^�[�Q�b�g���O�ꂽ�ꍇ�A�{�^�����^�[�Q�b�g�ɖ߂�
        if (EventSystem.current.currentSelectedGameObject != buttons[currentIndex])
        {
            EventSystem.current.SetSelectedGameObject(buttons[currentIndex]);
        }
    }

    

    
}
