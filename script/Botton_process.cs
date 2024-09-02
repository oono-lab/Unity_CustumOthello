using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Threading;
using UnityEngine.SceneManagement;

public class Botton_process : MonoBehaviour
{
    public Button[] BottonLsist_first;
    public Button[] BottonLsist_second;
    public Button[] BottonLsist_third;
    public Button BottonGameStart;
    public GameObject OthelloSystemScripts;
    public GameObject firstSelectedGameObject;
    public GameObject BlackStoneInformation;
    public GameObject WhiteStoneInformation;
    public GameObject ChoiceUI;
    public int CustumSelectBotton = 0;
    public bool CustumHantei = false;
    private OthelloScript othelloScript;
    private GameObject No;
    private TextMeshProUGUI ChoiceUIText;
    private bool ResetHantei = false;
    private bool GameOverHantei = false;
    //public GameObject button_list;
    void Start()
    {   
        othelloScript = OthelloSystemScripts.GetComponent<OthelloScript>();
        No = ChoiceUI.transform.Find("No").gameObject;
        ChoiceUIText = ChoiceUI.transform.Find("UI_text").GetComponent<TextMeshProUGUI>();
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Custum()
    {
        button_active(false, BottonLsist_first);
        button_active(true, BottonLsist_second);   
        EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);
        script_active(true, OthelloSystemScripts);
        othelloScript.Cube.SetActive(true);
        CustumHantei = true;
    }
    public void Online_Select()
    {
        SceneManager.LoadScene(1);
    }
    public void return_push()
    {
        return;
    }
    
    public void GameStart()
    {
        button_active(false, BottonLsist_first);
        button_active(false, BottonLsist_second);
        button_active(true, BottonLsist_third);
        BlackStoneInformation.SetActive(true);
        WhiteStoneInformation.SetActive(true);
        BottonGameStart.gameObject.SetActive(false);
        othelloScript.Cube.SetActive(true);
        if (CustumHantei == false) script_active(true, OthelloSystemScripts);
        else CustumHantei = false;
        EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);
    }
    #region 盤面をカスタマイズするためのボタン選択処理
    public void CustumBlackBotton()
    {
        CustumSelectBotton = 0;
    }
    public void CustumWhiteBotton()
    {
        CustumSelectBotton = 1;
    }
    public void CustumNoneBotton()
    {
        CustumSelectBotton = 2;
    }
    public void CustumWallBotton()
    {
        CustumSelectBotton = 3;
    }
    #endregion
    public void return_ShowUI()
    {
        ResetHantei = false;
        if (CustumHantei)
        {
            Reset_UI(OthelloSystemScripts, BottonLsist_second, No, false);
            ChoiceUIText.text = "カスタムを中断して、タイトルに戻りますか？";

        }
        else
        {
            Reset_UI(OthelloSystemScripts, BottonLsist_third, No, false);
            ChoiceUIText.text = "現在の試合を中断して、タイトルに戻りますか？";
        }    

        
    }
    public void Reset_ShowUI()
    {
        ResetHantei = true;
        if (CustumHantei)Reset_UI(OthelloSystemScripts, BottonLsist_second, No, false);
        else Reset_UI(OthelloSystemScripts, BottonLsist_third, No, false);
        ChoiceUIText.text = "盤面のコマを初めの位置に戻しますか？";
    }
    public void GameOver_TitleShowUI()
    {
        GameOverHantei = true;
        ResetHantei = false;
        Reset_UI(OthelloSystemScripts, BottonLsist_third, No, false);
        othelloScript.TitleButton.SetActive(false);
        othelloScript.OneMoreButton.SetActive(false);
        ChoiceUIText.text = "タイトルに戻りますか？";
    }
    public void GameOver_OneMoreShowUI()
    {
        GameOverHantei = true;
        ResetHantei = true;
        Reset_UI(OthelloSystemScripts, BottonLsist_third, No, false);
        othelloScript.TitleButton.SetActive(false);
        othelloScript.OneMoreButton.SetActive(false);
        ChoiceUIText.text = "もう一度同じ盤面で対戦しますか？";
    }
    public void YesButton()
    {   if(!ResetHantei)
        {
            SceneManager.LoadScene(0);
        }
        else if(CustumHantei)
        {
            othelloScript.ResetBoardCustum();
            Reset_UI(OthelloSystemScripts, BottonLsist_second, firstSelectedGameObject, true);
            
        }
        else
        {
            GameOverHantei = false;
            othelloScript.WinTextObj.SetActive(false);
            othelloScript.HikiwakeTextObj.SetActive(false);
            othelloScript.ResetBoardGame();
            Reset_UI(OthelloSystemScripts, BottonLsist_third, firstSelectedGameObject, true);
            othelloScript.Cube.SetActive(true);
        }
    }
    public void NoButton()
    {
        if (GameOverHantei)
        {
            ChoiceUI.SetActive(false);
            othelloScript.TitleButton.SetActive(true);
            othelloScript.OneMoreButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(othelloScript.TitleButton);

        }
        else
        {
            if (CustumHantei) Reset_UI(OthelloSystemScripts, BottonLsist_second, firstSelectedGameObject, true);
            else Reset_UI(OthelloSystemScripts, BottonLsist_third, firstSelectedGameObject, true);
        }
        
    }
    public void GameOverReset()
    {
        Reset_UI(OthelloSystemScripts, BottonLsist_third, othelloScript.TitleButton, false);
        ChoiceUI.SetActive(false);
    }
    void button_active(bool hantei, Button[] Botton_Lsist)
    {
        foreach (Button button in Botton_Lsist) button.gameObject.SetActive(hantei);
    }

    void script_active(bool hantei, GameObject ScriptObject)
    {

        MonoBehaviour[] scripts = ScriptObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) script.enabled = hantei;

    }
    void Reset_UI(GameObject Object, Button[] Object1,GameObject Object2,bool hantei)
    {  
        button_active(hantei, Object1);
        ChoiceUI.SetActive(!hantei);
        EventSystem.current.SetSelectedGameObject(Object2);
        if(CustumHantei) BottonGameStart.gameObject.SetActive(hantei);
        script_active(hantei, Object);
    }


}
