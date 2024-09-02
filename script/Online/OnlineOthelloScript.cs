using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Xml.Linq;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnlineOthelloScript : MonoBehaviourPun
{
    public GameObject OthelloSprite;
    public GameObject Cube;
    public GameObject CubeCliant;
    public GameObject firstSelectedGameObject;
    public GameObject BottonSystem;
    public TextMeshProUGUI BlackNumText;
    public TextMeshProUGUI WhiteNumText;
    public GameObject WinTextObj;
    public GameObject HikiwakeTextObj;
    public GameObject TitleButton;
    public GameObject OneMoreButton;
    public AudioSource StonePutAudio;
    public AudioSource NotPutAudio;
    public AudioSource CustumPutAudio;
    public AudioSource GameOverAudio;
    private GameObject CubeCopy;
    private GameObject CubeCopyCliant;
    const int FIELD_SIZE_X = 8;
    const int FIELD_SIZE_Y = 8;
    private Image BlackImage;
    private Image WhiteImage;
    private Color BlackColor;
    private Color WhiteColor;
    private int Cube_Position_X = 3;
    private int Cube_Position_Y = 3;
    private int Cube_Position_X_Cliant = 3;
    private int Cube_Position_Y_Cliant = 3;
    private float moveDelay = 0.15f;
    private float lastMoveTime = 0.0f;
    private Vector3 originalPosition;
    private bool Cube_Controll = false;
    private bool BlackCheckFlag = false;
    private bool WhiteCheckFlag = true;
    private bool isCallColor = true;
    private bool isChoicPoint = false;
    private OnlineBotton_process bottonProcess;
    private List<(int, int)> _infoList = new List<(int, int)>();

    public enum SpriteState
    {
        None,
        Black,
        White,
        Wall,
        NoneChoice
    }
    public SpriteState[] spriteStates = new SpriteState[4]
    {
    SpriteState.Black,
    SpriteState.White,
    SpriteState.None,
    SpriteState.Wall,
    };
    private SpriteState PlayerTurn = SpriteState.Black;
    public static SpriteState[,] FieldState = new SpriteState[FIELD_SIZE_X, FIELD_SIZE_Y];
    public static SpriteState[,] FieldStateCustum = new SpriteState[FIELD_SIZE_X, FIELD_SIZE_Y];
    private SpriteState[,] FieldStateStart = new SpriteState[FIELD_SIZE_X, FIELD_SIZE_Y];
    private SpriteState[,] FieldStateNone = new SpriteState[FIELD_SIZE_X, FIELD_SIZE_Y];
    private OnlineSpriteScript[,] FieldSpriteState = new OnlineSpriteScript[FIELD_SIZE_X, FIELD_SIZE_Y];
    private OnlineSpriteScript Cube1;
    SpriteState[] SerializeFieldState(SpriteState[,] fieldState)
    {
        SpriteState[] serializedData = new SpriteState[FIELD_SIZE_X * FIELD_SIZE_Y];
        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            {
                serializedData[x * FIELD_SIZE_Y + y] = fieldState[x, y];
            }
        }
        return serializedData;
    }

    // 1次元配列を2次元配列にデシリアライズ
    SpriteState[,] DeserializeFieldState(SpriteState[] serializedData)
    {
        SpriteState[,] fieldState = new SpriteState[FIELD_SIZE_X, FIELD_SIZE_Y];
        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            {
                fieldState[x, y] = serializedData[x * FIELD_SIZE_Y + y];
            }
        }
        return fieldState;
    }
    void Start()
    {
 
        originalPosition = Cube.transform.position;

        
        bottonProcess = BottonSystem.GetComponent<OnlineBotton_process>();
        
        
        if (!PhotonNetwork.IsConnected)
        {   
            CubeCopy = Instantiate(Cube);
            CubeCopy.SetActive(true);
        }
        else
        {   GameObject blackInstance = GameObject.Find("BlackStoneInformation");
            GameObject whiteInstance = GameObject.Find("WhiteStoneInformation");
            BlackImage = blackInstance.GetComponent<Image>();
            WhiteImage = whiteInstance.GetComponent<Image>();
            BlackColor = BlackImage.color;
            BlackColor.a = 0.0f;
            BlackImage.color = BlackColor;
            WhiteColor = WhiteImage.color;
            WhiteColor.a = 0.0f;
            WhiteImage.color = WhiteColor;
            if (PhotonNetwork.IsMasterClient) CubeCopy = PhotonNetwork.Instantiate(Cube.name, originalPosition, Quaternion.identity);
            else CubeCopyCliant = PhotonNetwork.Instantiate(CubeCliant.name, originalPosition, Quaternion.identity);



        }
        FieldStateStart[4, 3] = SpriteState.White;
        FieldStateStart[3, 4] = SpriteState.White;
        FieldStateStart[3, 3] = SpriteState.Black;
        FieldStateStart[4, 4] = SpriteState.Black;

        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            {
                var sprite = Instantiate(OthelloSprite, new Vector3(1.0f * x, 0, 1.0f * y), Quaternion.Euler(90, 0, 0));
                FieldStateNone[x, y] = SpriteState.None;
                if (!(FieldStateStart[x, y] == SpriteState.Black) && !(FieldStateStart[x, y] == SpriteState.White )) FieldStateStart[x, y] = SpriteState.None;

                FieldSpriteState[x, y] = sprite.GetComponent<OnlineSpriteScript>();
                if (bottonProcess.CustumHantei) 
                {
                    FieldState[x, y] = FieldStateStart[x, y];
                    FieldStateCustum[x, y] = FieldState[x, y];

                }
                else FieldState[x, y] = FieldStateCustum[x, y];
                FieldSpriteState[x, y].SetState(FieldState[x, y]);
            }
        }

    }

    void Update()
    {

        #region 座標設定
        var position = Cube.transform.localPosition;
        var positionCliant = CubeCliant.transform.localPosition;
        
        if (EventSystem.current.currentSelectedGameObject == firstSelectedGameObject && Time.time - lastMoveTime >= moveDelay)
        {
            Cube_Controll = true;
            if (bottonProcess.CustumHantei)
            {
                CubeControll(Cube_Position_X, Cube_Position_Y, Cube, position, true);
                CubeCopy.transform.position = Cube.transform.position;
            }
            else 
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    CubeControll(Cube_Position_X, Cube_Position_Y, Cube, position, true);
                    CubeCopy.transform.position = Cube.transform.position;
                }
                else if (!PhotonNetwork.IsMasterClient)
                {
                    CubeControll(Cube_Position_X_Cliant, Cube_Position_Y_Cliant, CubeCliant, positionCliant, false);
                    CubeCopyCliant.transform.position = CubeCliant.transform.position;
                }
            }
            

        }

        if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
        {
            CubeControllHantei(Cube_Position_X, firstSelectedGameObject);
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            CubeControllHantei(Cube_Position_X_Cliant, firstSelectedGameObject);
        }
        
        #endregion

        if (!bottonProcess.CustumHantei)
        {
            
            if (PlayerTurn == SpriteState.White && PhotonNetwork.IsMasterClient) photonView.RPC("GameMode", RpcTarget.All, Cube_Position_X, Cube_Position_Y);
            else if (PlayerTurn == SpriteState.Black && !PhotonNetwork.IsMasterClient) photonView.RPC("GameMode", RpcTarget.All, Cube_Position_X_Cliant, Cube_Position_Y_Cliant);
        }
        else
        { 

            CustumMode();
        }



    }
    private void CustumMode()
    {
        if (EventSystem.current.currentSelectedGameObject == firstSelectedGameObject)
        {
            if (Input.GetKeyDown(KeyCode.Return)) 
            {
                FieldState[Cube_Position_X, Cube_Position_Y] = spriteStates[bottonProcess.CustumSelectBotton];
                FieldStateCustum[Cube_Position_X, Cube_Position_Y] = FieldState[Cube_Position_X, Cube_Position_Y];
                CustumPutAudio.Play();
            } 
            _infoList = new List<(int, int)>();
        }

        ShowSpriteBoard();
    }
    [PunRPC]
    private void GameMode(int Cube_Position_X_This,int Cube_Position_Y_This)
    {
        var turnCheck = false;
        if (EventSystem.current.currentSelectedGameObject == firstSelectedGameObject)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                for (int i = 0; i <= 7; i++)
                {
                    if (TurnCheck(i, Cube_Position_X_This, Cube_Position_Y_This)) turnCheck = true;
                }
                if (turnCheck && (FieldState[Cube_Position_X_This, Cube_Position_Y_This] == SpriteState.None || FieldState[Cube_Position_X_This, Cube_Position_Y_This] == SpriteState.NoneChoice))
                {
                    foreach (var info in _infoList)
                    {
                        var position_X = info.Item1;
                        var position_Y = info.Item2;
                        FieldState[position_X, position_Y] = PlayerTurn;

                    }
                    FieldState[Cube_Position_X_This, Cube_Position_Y_This] = PlayerTurn;
                    ChangePlayerTurn();
                    isChoicPoint = true;
                    isCallColor = true;
                    StonePutAudio.Play();

                }
                else 
                {
                    NotPutAudio.Play();
                }



            }
            _infoList = new List<(int, int)>();
        }

        turnCheck = false;
        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            {
                for (int i = 0; i <= 7; i++)
                {
                    if (TurnCheck(i, x, y) && (FieldState[x, y] == SpriteState.None || FieldState[x, y] == SpriteState.NoneChoice))
                    {
                        FieldState[x, y] = SpriteState.NoneChoice;
                        if (PlayerTurn == SpriteState.Black)
                        {
                            turnCheck = true;
                            BlackCheckFlag = true;
                            if (!WhiteCheckFlag) WhiteCheckFlag = true;
                            break;
                        }
                        else if (PlayerTurn == SpriteState.White)
                        {
                            turnCheck = true;
                            WhiteCheckFlag = true;
                            if (!BlackCheckFlag) BlackCheckFlag = true;
                            break;
                        }

                    }

                }
            }
        }
        if (!turnCheck)
        {
            if (PlayerTurn == SpriteState.Black) BlackCheckFlag = false;
            else if (PlayerTurn == SpriteState.White) WhiteCheckFlag = false;
            ChangePlayerTurn();
        }
        _infoList = new List<(int, int)>();
        UpdateBoard();

    }
    private void UpdateBoard()
    {
        var WhiteNum = default(int);
        var BlackNum = default(int);
        
        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            {
                if (FieldState[x, y] == SpriteState.White) WhiteNum++;
                else if (FieldState[x, y] == SpriteState.Black) BlackNum++;
            }


        }

        if (Input.GetKeyDown(KeyCode.Return) && isChoicPoint) SendFieldStateToParticipantsFieldState();


        if(PlayerTurn == SpriteState.White && PhotonNetwork.IsMasterClient) ShowSpriteBoard();
        else if(PlayerTurn == SpriteState.Black && !PhotonNetwork.IsMasterClient) ShowSpriteBoard();



        if (isCallColor)
        {
            if (PlayerTurn == SpriteState.White) ColorChange(0.0f, 1.0f);
            else if (PlayerTurn == SpriteState.Black) ColorChange(1.0f, 0.0f);
        }

        BlackNumText.text = BlackNum.ToString();
        WhiteNumText.text = WhiteNum.ToString();
        if (WhiteNum + BlackNum == 64 || !WhiteCheckFlag && !BlackCheckFlag) GameOver(WhiteNum, BlackNum);
    }
    private void GameOver(int WhiteNum, int BlackNum)
    {
        GameObject Black = WinTextObj.transform.Find("Black").gameObject;
        GameObject White = WinTextObj.transform.Find("White").gameObject;
        ColorChange(0.0f, 0.0f);
        TitleButton.SetActive(true);
        OneMoreButton.SetActive(true);
        bottonProcess.GameOverReset();
        firstSelectedGameObject.SetActive(false);
        Cube_Controll = false;
        GameOverAudio.Play();
        if (WhiteNum == BlackNum) HikiwakeTextObj.SetActive(true);
        else
        {
            WinTextObj.SetActive(true);
            if (WhiteNum > BlackNum) Black.SetActive(false);
            if (WhiteNum < BlackNum) White.SetActive(false);
        }



    }


    private bool TurnCheck(int Direction ,int Cube_Position_X_Target,int Cube_Position_Y_Target)
    {
        var turnCheck = false;
        var position_x = Cube_Position_X_Target;
        var position_y = Cube_Position_Y_Target;
        var OpponentPlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.White : SpriteState.Black;
        var infoList = new List<(int, int)>();
        while (0 <= position_x && 7 >= position_x && 0 <= position_y && 7 >= position_y)
        {
            switch (Direction)
            {
                case 0://左方向
                    if (position_x == 0) return turnCheck;
                    position_x--;
                    break;
                case 1://右方向
                    if (position_x == 7) return turnCheck;
                    position_x++;
                    break;
                case 2://下方向
                    if (position_y == 0) return turnCheck;
                    position_y--;
                    break;
                case 3://上方方向
                    if (position_y == 7) return turnCheck;
                    position_y++;
                    break;
                case 4://右上方向
                    if (position_x == 7) return turnCheck;
                    if (position_y == 7) return turnCheck;
                    position_x++;
                    position_y++;
                    break;
                case 5://左下方向
                    if (position_x == 0) return turnCheck;
                    if (position_y == 0) return turnCheck;
                    position_x--;
                    position_y--;
                    break;
                case 6://左上方向
                    if (position_x == 0) return turnCheck;
                    if (position_y == 7) return turnCheck;
                    position_x--;
                    position_y++;
                    break;
                case 7://右下方向
                    if (position_x == 7) return turnCheck;
                    if (position_y == 0) return turnCheck;
                    position_x++;
                    position_y--;
                    break;
            }


            if (FieldState[position_x, position_y] == OpponentPlayerTurn) infoList.Add((position_x, position_y));
            if (infoList.Count == 0 && FieldState[position_x, position_y] == PlayerTurn || FieldState[position_x, position_y] == SpriteState.None || FieldState[position_x, position_y] == SpriteState.Wall || FieldState[position_x, position_y] == SpriteState.NoneChoice)
            {
                break;
            }
            if (infoList.Count > 0 && FieldState[position_x, position_y] == PlayerTurn)
            {
                turnCheck = true;
                foreach (var info in infoList) _infoList.Add(info);
                break;
            }
        }


        return turnCheck;
    }
    void CubeControll(int Cube_Position_X_This, int Cube_Position_Y_This,GameObject TargetCube,Vector3 position,bool hantei)
    {
        if (Input.GetKey(KeyCode.RightArrow) && Cube_Position_X_This < 7)
        {
            Cube_Position_X_This++;
            TargetCube.transform.localPosition = new Vector3(position.x + 1.0f, position.y, position.z);
            lastMoveTime = Time.time;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && Cube_Position_X_This > 0)
        {
            Cube_Position_X_This--;
            TargetCube.transform.localPosition = new Vector3(position.x - 1.0f, position.y, position.z);
            lastMoveTime = Time.time;
        }
        else if (Input.GetKey(KeyCode.UpArrow) && Cube_Position_Y_This < 7)
        {
            Cube_Position_Y_This++;
            TargetCube.transform.localPosition = new Vector3(position.x, position.y, position.z + 1.0f);
            lastMoveTime = Time.time;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Cube_Position_Y_This > 0)
        {
            Cube_Position_Y_This--;
            TargetCube.transform.localPosition = new Vector3(position.x, position.y, position.z - 1.0f);
            lastMoveTime = Time.time;
        }

        if (bottonProcess.CustumHantei) CubeControllUpdate(Cube_Position_X_This, Cube_Position_Y_This, hantei);
        else photonView.RPC("CubeControllUpdate", RpcTarget.All, Cube_Position_X_This, Cube_Position_Y_This, hantei);




    }
    [PunRPC]
    void CubeControllUpdate(int Cube_Position_X_This, int Cube_Position_Y_This, bool hantei)
    {
        if (hantei)
        {
            Cube_Position_X = Cube_Position_X_This;
            Cube_Position_Y = Cube_Position_Y_This;
        }
        else
        {
            Cube_Position_X_Cliant = Cube_Position_X_This;
            Cube_Position_Y_Cliant = Cube_Position_Y_This;
        }
    }
    void CubeControllHantei(int Cube_Position_X_This,GameObject TargetObject)
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && (!Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow)) && Cube_Position_X_This == 0) Cube_Controll = false;
        else if (Input.GetKeyDown(KeyCode.RightArrow) && (!Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow)) && Cube_Position_X_This == 7) Cube_Controll = false;
        
        if (Cube_Controll) EventSystem.current.SetSelectedGameObject(TargetObject);
    }
    [PunRPC]
    public void ResetBoardGame()
    {
        ResetBoardProcess(FieldState, FieldStateCustum);
    }
    public void ResetBoardCustum()
    {
        ResetBoardProcess(FieldState, FieldStateStart);
    }
    

    [PunRPC]
    void SyncColorChange(float blackAlpha, float whiteAlpha)
    {
        BlackColor.a = blackAlpha;
        BlackImage.color = BlackColor;
        WhiteColor.a = whiteAlpha;
        WhiteImage.color = WhiteColor;


    }

    void ColorChange(float blackAlpha, float whiteAlpha)
    {
        // ローカルの値を更新
        BlackColor.a = blackAlpha;
        BlackImage.color = BlackColor;
        WhiteColor.a = whiteAlpha;
        WhiteImage.color = WhiteColor;

        // 他のクライアントにもRPCで色の変更を伝える
        
        photonView.RPC("SyncColorChange", RpcTarget.Others, blackAlpha, whiteAlpha);
        isCallColor = false;
        
    }
    void ResetBoardProcess(SpriteState[,] MainSpriteState, SpriteState[,] TargetSpriteState)
    {
        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            {
                MainSpriteState[x, y] = TargetSpriteState[x, y];
            }
        }
        PlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.Black : SpriteState.Black;
        Cube.transform.position = new Vector3(3.02400017f, -0.00100000005f, 3.02099991f);
        Cube_Position_X = 3;
        Cube_Position_Y = 3;
        
    }
    [PunRPC]
    void ShowSpriteBoard()
    {
        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            { 
                FieldSpriteState[x, y].SetState(FieldState[x, y]);
            }
        }
    }
    void OnDisable()
    {
        // ゲーム終了時に元のTransform情報にリセット
        CubeCliant.transform.position = originalPosition;
        Cube.transform.position = originalPosition;
    }
    private void Awake()
    {
        RegisterCustomTypes();
    }

    private void RegisterCustomTypes()
    {
        PhotonPeer.RegisterType(typeof(SpriteState), (byte)'S', SerializeSpriteState, DeserializeSpriteState);
    }

    private static byte[] SerializeSpriteState(object customObject)
    {
        SpriteState spriteState = (SpriteState)customObject;
        return new byte[] { (byte)spriteState };
    }

    private static object DeserializeSpriteState(byte[] data)
    {
        return (SpriteState)data[0];
    }
    public void SendFieldStateToParticipantsCustum()
    {
        SpriteState[] serializedFieldState = SerializeFieldState(FieldStateCustum);
        photonView.RPC("SyncFieldStateCustum", RpcTarget.Others, serializedFieldState);
    }

    public void SendFieldStateToParticipantsFieldState()
    {   
        SpriteState[] serializedFieldState = SerializeFieldState(FieldState);
        photonView.RPC("SyncFieldStateFieldState", RpcTarget.All, serializedFieldState);

    }
    // 受信側でフィールド状態を同期するためのRPCメソッド
    [PunRPC]
    void SyncFieldStateCustum(SpriteState[] serializedFieldState)
    {

        // デシリアライズしてフィールド状態を適用
        FieldStateCustum = DeserializeFieldState(serializedFieldState);

    }
    
    [PunRPC]
    void SyncFieldStateFieldState(SpriteState[] serializedFieldState)
    {
        // デシリアライズしてフィールド状態を適用
        FieldState = DeserializeFieldState(serializedFieldState);
        UpdateField();
    }
    // フィールドを更新するメソッド
    void UpdateField()
    {
        isChoicPoint = false;
        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            {
                if (FieldState[x, y] == SpriteState.NoneChoice) FieldState[x, y] = SpriteState.None;
                FieldSpriteState[x, y].SetState(FieldState[x, y]);
            }
        }// フィールド状態に基づいてゲームボードを更新する処理をここに追加
    }
    [PunRPC]
    void SyncPlayerTurn(SpriteState newTurn)
    {
        PlayerTurn = newTurn;
        
    }
    void ChangePlayerTurn()
    {
        // プレイヤーのターンを切り替える
        PlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.White : SpriteState.Black;

        // RPCで全クライアントに同期
        photonView.RPC("SyncPlayerTurn", RpcTarget.All, PlayerTurn);
    }

}
