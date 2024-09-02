using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour
{
    public AudioSource selectSound;  // �{�^�����I�����ꂽ�Ƃ��̉�
    public AudioSource clickSound;   // �{�^�����N���b�N���ꂽ�Ƃ��̉�
    public bool isFirstSelection = true;


    // �ŏ��ɑI�����ꂽ�Ƃ��͉���炳�Ȃ��悤�ɂ��A����ȍ~�̑I���ł͉����Đ�����֐�
    public void OnSelect(AudioSource audiosorce)
    {
        if (!isFirstSelection) audiosorce.Play();
        else isFirstSelection = false;
      
    }   
   
}
