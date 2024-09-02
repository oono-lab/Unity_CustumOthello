using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour
{
    public AudioSource selectSound;  // ボタンが選択されたときの音
    public AudioSource clickSound;   // ボタンがクリックされたときの音
    public bool isFirstSelection = true;


    // 最初に選択されたときは音を鳴らさないようにし、それ以降の選択では音を再生する関数
    public void OnSelect(AudioSource audiosorce)
    {
        if (!isFirstSelection) audiosorce.Play();
        else isFirstSelection = false;
      
    }   
   
}
