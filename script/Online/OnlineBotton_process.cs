using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class OnlineBotton_process : MonoBehaviourPunCallbacks
{
    public Button[] BottonLsist_first;
    public Button[] BottonLsist_second;
    public Button[] BottonLsist_third;
    public Button BottonGameStart;
    public Button ExitRoomList;
    public GameObject GameDisconectedPanel;
    public GameObject OthelloSystemScripts;
    public GameObject firstSelectedGameObject;
    public GameObject ChoiceUI;
    public GameObject BattleMatchScene;
    public ButtonSound buttonSound;
    public int CustumSelectBotton = 0;
    public bool CustumHantei = false;
    private OnlineOthelloScript othelloScript;
    private GameObject No;
    private TextMeshProUGUI ChoiceUIText;
    private bool ResetHantei = false;
    private bool GameOverHantei = false;
    private bool localPlayerAgreed = false;  // 自分の同意ステータス
    private bool remotePlayerAgreed = false; // 相手プレイヤーの同意ステータス
    void Start()
    {
        othelloScript = OthelloSystemScripts.GetComponent<OnlineOthelloScript>();
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
        script_active(true);
        CustumHantei = true;
    }
    public void RoomListEnter()
    {
        button_active(false, BottonLsist_first);
        ExitRoomList.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(ExitRoomList.gameObject);
    }
    public void CreateRoomEnter()
    {
        button_active(false, BottonLsist_second);
        ExitRoomList.gameObject.SetActive(true);
        buttonSound.isFirstSelection = true;
        EventSystem.current.SetSelectedGameObject(ExitRoomList.gameObject);
    }
    public void CreateRoomExit()
    {
        button_active(true, BottonLsist_second);
        ExitRoomList.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);
    }
    public void Online_Select()
    {
        button_active(false, BottonLsist_first);
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
        BottonGameStart.gameObject.SetActive(false);
        othelloScript.Cube.SetActive(true);
        if (CustumHantei == false) script_active(true);
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
        button_active(false, BottonLsist_third);
        ChoiceUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(No);
        ChoiceUIText.text = "現在の試合を中断して、タイトルに戻りますか？";
        
    }
    public void Reset_ShowUI()
    {
        ResetHantei = true;
        button_active(false, BottonLsist_second);
        ChoiceUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(No);

        ChoiceUIText.text = "盤面のコマを初めの位置に戻しますか？";
    }
    public void GameOver_TitleShowUI()
    {
        GameOverHantei = true;
        ResetHantei = false;
        Reset_UI(BottonLsist_third, No, false);
        othelloScript.TitleButton.SetActive(false);
        othelloScript.OneMoreButton.SetActive(false);
        ChoiceUIText.text = "タイトルに戻りますか？";
    }
    public void GameOver_OneMoreShowUI()
    {
        GameOverHantei = true;
        ResetHantei = true;
        Reset_UI(BottonLsist_third, No, false);
        othelloScript.TitleButton.SetActive(false);
        othelloScript.OneMoreButton.SetActive(false);
        ChoiceUIText.text = "もう一度同じプレイヤーと対戦しますか？";
    }
    public void YesButton()
    {
        if (CustumHantei)
        {

            
            othelloScript.ResetBoardCustum();
            buttonSound.isFirstSelection = true;
            Reset_UI(BottonLsist_second, firstSelectedGameObject, true);

        }
        else
        {

            if (!ResetHantei)
            {   
                ChoiceUI.SetActive(false);
                PhotonNetwork.Disconnect();
            }
            else
            {
                GameOverHantei = false;
                localPlayerAgreed = true;

                // 自分のhanteiステータスを相手プレイヤーに通知
                photonView.RPC("RPCHantei", RpcTarget.Others);
                // 両方のhanteiがtrueになったか確認
                ChoiceUI.SetActive(false);
                BattleMatchScene.SetActive(true);
                if (localPlayerAgreed && remotePlayerAgreed) { Debug.Log(true);  PhotonNetwork.LoadLevel(2); }

            }

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
            if (CustumHantei) 
            {
                button_active(true, BottonLsist_second);
                ChoiceUI.SetActive(false);
                EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);

            }
            else
            {
                button_active(true, BottonLsist_third);
                ChoiceUI.SetActive(false);
                EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);
            }
        }

    }
    public void GameOverReset()
    {
        Reset_UI(BottonLsist_third, othelloScript.TitleButton, false);
        ChoiceUI.SetActive(false);
    }
    
    public void SceneLoad(int ScenePoint)
    {   
        SceneManager.LoadScene(ScenePoint);
    }
    void button_active(bool hantei, Button[] Botton_Lsist)
    {
        foreach (Button button in Botton_Lsist) button.gameObject.SetActive(hantei);
    }
    [PunRPC]
    void script_active(bool hantei)
    {

        MonoBehaviour[] scripts = OthelloSystemScripts.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) script.enabled = hantei;

    }
    void Reset_UI( Button[] Object1, GameObject Object2, bool hantei)
    {
        button_active(hantei, Object1);
        ChoiceUI.SetActive(!hantei);
        EventSystem.current.SetSelectedGameObject(Object2);
        if (CustumHantei) BottonGameStart.gameObject.SetActive(hantei);

        script_active(hantei);
    }
    
    [PunRPC]
    void RPCHantei()
    {
        remotePlayerAgreed = true;
        if (localPlayerAgreed && remotePlayerAgreed) PhotonNetwork.LoadLevel(2);
    }

}
