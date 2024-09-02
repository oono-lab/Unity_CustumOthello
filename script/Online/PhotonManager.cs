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

    public GameObject roomListPanel;          // ���[���{�^����z�u����p�l��
    public GameObject roomButtonPrefab;
    public GameObject YESButton;
    public OnlineBotton_process OnlineButtonProcess;// ���[���{�^���̃v���n�u
    public Button connectButton;              // �T�[�o�[�ڑ��{�^��
    public Button createRoomButton;
    public GameObject CreateRoomPanel;        // ���[���쐬��̒ʒm�p�l��
    public OnlineOthelloScript othelloScript;// ���[���쐬�{�^��
    private TextMeshProUGUI roomCreatedMessageText;
    private bool isCreateRoom = false;

    // ��ʏ�Ԃ�ێ����邽�߂̕ϐ��i��Ƃ���string���g�p�j
    private string currentUIState = "InitialUI";  // ��: ������Ԃ� "InitialUI" �Ƃ���

    private void Start()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();

        // �{�^���̃C�x���g���X�i�[��ǉ�
        if (!PhotonNetwork.IsConnected) 
        {
            connectButton.onClick.AddListener(OnConnectButtonClicked);
            createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
        }
        
        OnlineButtonProcess.ExitRoomList.onClick.AddListener(DisconnectFromPhotonServer);
        PhotonNetwork.AutomaticallySyncScene = true;
        roomCreatedMessageText = CreateRoomPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    // �T�[�o�[�ɐڑ����鏈��
    private void OnConnectButtonClicked()
    {
        Debug.Log("�T�[�o�[�ɐڑ����Ă��܂�...");
        roomListPanel.SetActive(true);
        OnlineButtonProcess.RoomListEnter();
        PhotonNetwork.ConnectUsingSettings();  // �T�[�o�[�ɐڑ�
    }

    // ���[���쐬�{�^���������ꂽ�Ƃ��̏���
    private void OnCreateRoomButtonClicked()
    {
        isCreateRoom = true;
        if (!PhotonNetwork.IsConnected)
        {
            OnlineButtonProcess.CreateRoomEnter();
            CreateRoomPanel.SetActive(true);
            roomCreatedMessageText.text = $"���[�����쐬���Ă��܂��B���΂炭���҂����������B";
            PhotonNetwork.ConnectUsingSettings();  // �T�[�o�[�ɐڑ�
        }
        else
        {   
            PhotonNetwork.JoinLobby(); // ���ɐڑ�����Ă���ꍇ�̓��r�[�ɐڑ�
        }
    }

    // �}�X�^�[�T�[�o�[�ւ̐ڑ��������������̃R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    // ���r�[�ɓ�������̃R�[���o�b�N
    public override void OnJoinedLobby()
    {
        Debug.Log("���r�[�ɓ���܂���");

        if (isCreateRoom)
        {
            CreateRoom();
            isCreateRoom = false;
        }
    }

    // ���[���쐬����
    private void CreateRoom()
    {
        Debug.Log("���[�����쐬���Ă��܂�...");
        
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = true;
        string roomName = "Room" + Random.Range(1000, 9999);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    // ���[���̍쐬�������������̃R�[���o�b�N
    public override void OnCreatedRoom()
    {
        Debug.Log("���[���̍쐬�ɐ������܂���: " + PhotonNetwork.CurrentRoom.Name);

        cachedRoomList[PhotonNetwork.CurrentRoom.Name] = PhotonNetwork.CurrentRoom;
        roomCreatedMessageText.text = $"���Ȃ��� {PhotonNetwork.CurrentRoom.Name} ���쐬���܂����B�v���C���[������܂ł��΂炭���҂����������B";
        UpdateRoomListView();
    }

    // �v���C���[�����[���ɓ������ۂɌĂ΂��R�[���o�b�N
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("�V�����v���C���[�����[���ɎQ�����܂���: " + newPlayer.NickName);

        // ���[���̃v���C���[����2�l�ɂȂ�����V�[����J��
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("���[���̃v���C���[��2�l�ɂȂ�܂����B�V�[����J�ڂ��A��Ԃ𓯊����܂�...");
            othelloScript.SendFieldStateToParticipantsCustum();
            currentUIState = GetCurrentUIState();
            PhotonNetwork.LoadLevel(2);

            // ��Ԃ�V�����v���C���[�ɓ���
            photonView.RPC("SyncUIState", RpcTarget.Others, currentUIState);
        }
    }

    // ��ʏ�Ԃ��擾����i��Ƃ���string��Ԃ����\�b�h���g�p�j
    private string GetCurrentUIState()
    {
        // �����Ō��݂�UI��Ԃ��擾���鏈�����L�q
        // ��: ���݂�UI�{�^���̏�ԂȂ�
        return "InitialUI";  // ��: ������ԂƂ��� "InitialUI" ��Ԃ�
    }

    // �}�X�^�[�N���C�A���g����UI��Ԃ𓯊�
    [PunRPC]
    private void SyncUIState(string uiState)
    {
        Debug.Log("�}�X�^�[�N���C�A���g����UI��Ԃ���M���܂���: " + uiState);

        // ��M����UI��ԂɊ�Â��ĉ�ʂ��X�V
        UpdateUI(uiState);
    }

    // UI���X�V���鏈���i��Ƃ���string���g�p�j
    private void UpdateUI(string uiState)
    {
        Debug.Log("UI��Ԃ��X�V: " + uiState);

        // ������UI��Ԃ��X�V���鏈�����L�q
        // ��: ��M����UI��Ԃɉ����ă{�^���̕\����A�N�V������ύX
    }

    // ���[���ɎQ�����鏈��
    private void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // ���[���쐬�����s�������̃R�[���o�b�N
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("���[���̍쐬�Ɏ��s���܂���: " + message);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("�v���C���[�����[����ޏo���܂���: " + otherPlayer.NickName);

        // �������[������1�l�����c���Ă��Ȃ���΁A���̃v���C���[�������ޏo������
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
    // ���[�����X�g�ɍX�V�����������̃R�[���o�b�N
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
            if (info.PlayerCount == 0) roomsToRemove.Add(info.Name); // �폜�Ώۂ̃��[�������X�g�ɒǉ�
        }

        foreach (string roomName in roomsToRemove)
        {
            cachedRoomList.Remove(roomName);

            // ���[�����X�g�p�l���̎q�I�u�W�F�N�g��T���ĊY������{�^�����폜
            foreach (Transform child in roomListPanel.transform)
            {
                TextMeshProUGUI roomNameText = child.GetComponentInChildren<TextMeshProUGUI>();
                if (roomNameText != null && roomNameText.text.Contains(roomName)) Destroy(child.gameObject);
            }
        }
    }
}

