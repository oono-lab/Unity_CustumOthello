using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PhotonFindRoom : MonoBehaviourPunCallbacks
{
    private Dictionary<string, RoomInfo> cachedRoomList;

    public GameObject roomListPanel;          // ルームボタンを配置するパネル
    public GameObject roomButtonPrefab;
    public GameObject YESButton;
    public OnlineBotton_process OnlineButtonProcess;// ルームボタンのプレハブ
    public Button connectButton;              // サーバー接続ボタン
    public Button createRoomButton;
    public GameObject CreateRoomPanel;        // ルーム作成後の通知パネル
    public OnlineOthelloScript othelloScript;// ルーム作成ボタン
    private TextMeshProUGUI roomCreatedMessageText;
    private bool isCreateRoom = false;

    // 画面状態を保持するための変数（例としてstringを使用）
    private string currentUIState = "InitialUI";  // 例: 初期状態を "InitialUI" とする

    private void Start()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();

        // ボタンのイベントリスナーを追加
        if (!PhotonNetwork.IsConnected) 
        {
            connectButton.onClick.AddListener(OnConnectButtonClicked);
            createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
        }
        
        OnlineButtonProcess.ExitRoomList.onClick.AddListener(DisconnectFromPhotonServer);
        PhotonNetwork.AutomaticallySyncScene = true;
        roomCreatedMessageText = CreateRoomPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    // サーバーに接続する処理
    private void OnConnectButtonClicked()
    {
        Debug.Log("サーバーに接続しています...");
        roomListPanel.SetActive(true);
        OnlineButtonProcess.RoomListEnter();
        PhotonNetwork.ConnectUsingSettings();  // サーバーに接続
    }

    // ルーム作成ボタンが押されたときの処理
    private void OnCreateRoomButtonClicked()
    {
        isCreateRoom = true;
        if (!PhotonNetwork.IsConnected)
        {
            OnlineButtonProcess.CreateRoomEnter();
            CreateRoomPanel.SetActive(true);
            roomCreatedMessageText.text = $"ルームを作成しています。しばらくお待ちください。";
            PhotonNetwork.ConnectUsingSettings();  // サーバーに接続
        }
        else
        {   
            PhotonNetwork.JoinLobby(); // 既に接続されている場合はロビーに接続
        }
    }

    // マスターサーバーへの接続が成功した時のコールバック
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    // ロビーに入った後のコールバック
    public override void OnJoinedLobby()
    {
        Debug.Log("ロビーに入りました");

        if (isCreateRoom)
        {
            CreateRoom();
            isCreateRoom = false;
        }
    }

    // ルーム作成処理
    private void CreateRoom()
    {
        Debug.Log("ルームを作成しています...");
        
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = true;
        string roomName = "Room" + Random.Range(1000, 9999);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    // ルームの作成が成功した時のコールバック
    public override void OnCreatedRoom()
    {
        Debug.Log("ルームの作成に成功しました: " + PhotonNetwork.CurrentRoom.Name);

        cachedRoomList[PhotonNetwork.CurrentRoom.Name] = PhotonNetwork.CurrentRoom;
        roomCreatedMessageText.text = $"あなたは {PhotonNetwork.CurrentRoom.Name} を作成しました。プレイヤーが来るまでしばらくお待ちください。";
        UpdateRoomListView();
    }

    // プレイヤーがルームに入った際に呼ばれるコールバック
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("新しいプレイヤーがルームに参加しました: " + newPlayer.NickName);

        // ルームのプレイヤー数が2人になったらシーンを遷移
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ルームのプレイヤーが2人になりました。シーンを遷移し、状態を同期します...");
            othelloScript.SendFieldStateToParticipantsCustum();
            currentUIState = GetCurrentUIState();
            PhotonNetwork.LoadLevel(2);

            // 状態を新しいプレイヤーに同期
            photonView.RPC("SyncUIState", RpcTarget.Others, currentUIState);
        }
    }

    // 画面状態を取得する（例としてstringを返すメソッドを使用）
    private string GetCurrentUIState()
    {
        // ここで現在のUI状態を取得する処理を記述
        // 例: 現在のUIボタンの状態など
        return "InitialUI";  // 例: 初期状態として "InitialUI" を返す
    }

    // マスタークライアントからUI状態を同期
    [PunRPC]
    private void SyncUIState(string uiState)
    {
        Debug.Log("マスタークライアントからUI状態を受信しました: " + uiState);

        // 受信したUI状態に基づいて画面を更新
        UpdateUI(uiState);
    }

    // UIを更新する処理（例としてstringを使用）
    private void UpdateUI(string uiState)
    {
        Debug.Log("UI状態を更新: " + uiState);

        // ここでUI状態を更新する処理を記述
        // 例: 受信したUI状態に応じてボタンの表示やアクションを変更
    }

    // ルームに参加する処理
    private void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // ルーム作成が失敗した時のコールバック
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("ルームの作成に失敗しました: " + message);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("プレイヤーがルームを退出しました: " + otherPlayer.NickName);

        // もしルーム内に1人しか残っていなければ、そのプレイヤーも強制退出させる
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            OnlineButtonProcess.GameDisconectedPanel.SetActive(true);
            OnlineButtonProcess.OthelloSystemScripts.SetActive(false);
            OnlineButtonProcess.firstSelectedGameObject.SetActive(false);
            OnlineButtonProcess.BottonLsist_second[1].gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(YESButton);
            //PhotonNetwork.Disconnect();

        }
    }
    // ルームリストに更新があった時のコールバック
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }
    public void DisconnectFromPhotonServer()
    {
        PhotonNetwork.Disconnect();
        OnlineButtonProcess.ExitRoomList.gameObject.SetActive(false);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (OnlineButtonProcess.CustumHantei)
        {
            OnlineButtonProcess.CreateRoomExit();
            CreateRoomPanel.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
                continue;
            }

            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    private void UpdateRoomListView()
    {
        HashSet<string> existingRoomNames = new HashSet<string>();
        foreach (Transform child in roomListPanel.transform)
        {
            TextMeshProUGUI roomNameText = child.GetComponentInChildren<TextMeshProUGUI>();
            if (roomNameText != null)
            {
                string roomName = roomNameText.text.Split(' ')[0];
                existingRoomNames.Add(roomName);
            }
        }

        foreach (RoomInfo info in cachedRoomList.Values)
        {
            if (!existingRoomNames.Contains(info.Name))
            {
                GameObject newButton = Instantiate(roomButtonPrefab, roomListPanel.transform);
                TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = info.Name;
                }

                newButton.GetComponent<Button>().onClick.AddListener(() => JoinRoom(info.Name));
            }
        }
        RemoveEmptyRoomButtons();
    }
    private void RemoveEmptyRoomButtons()
    {
        List<string> roomsToRemove = new List<string>();

        foreach (RoomInfo info in cachedRoomList.Values)
        {
            if (info.PlayerCount == 0) roomsToRemove.Add(info.Name); // 削除対象のルームをリストに追加
        }

        foreach (string roomName in roomsToRemove)
        {
            cachedRoomList.Remove(roomName);

            // ルームリストパネルの子オブジェクトを探して該当するボタンを削除
            foreach (Transform child in roomListPanel.transform)
            {
                TextMeshProUGUI roomNameText = child.GetComponentInChildren<TextMeshProUGUI>();
                if (roomNameText != null && roomNameText.text.Contains(roomName)) Destroy(child.gameObject);
            }
        }
    }
}

