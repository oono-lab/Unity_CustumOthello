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

public class OthelloScript : MonoBehaviour
{
    public GameObject OthelloSprite;
    public GameObject Cube;
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
    private GameObject BlackInformation;
    private GameObject WhiteInformation;
    const int FIELD_SIZE_X = 8;
    const int FIELD_SIZE_Y = 8;
    private Image BlackImage;
    private Image WhiteImage;
    private Color BlackColor;
    private Color WhiteColor;
    private int Cube_Position_X = 3;
    private int Cube_Position_Y = 3;
    private float moveDelay = 0.15f;
    private float lastMoveTime = 0.0f;
    private bool Cube_Controll = false;
    private bool BlackCheckFlag = false;
    private bool WhiteCheckFlag = true;
    private bool IsFirstPutAudio = true;

    private Botton_process bottonProcess;
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
    private SpriteState[,] FieldState = new SpriteState[FIELD_SIZE_X, FIELD_SIZE_Y];
    private SpriteState[,] FieldStateCustum = new SpriteState[FIELD_SIZE_X, FIELD_SIZE_Y];
    private SpriteState[,] FieldStateStart = new SpriteState[FIELD_SIZE_X, FIELD_SIZE_Y];
    private SpriteState[,] FieldStateNone = new SpriteState[FIELD_SIZE_X, FIELD_SIZE_Y];
    private SpriteScript[,] FieldSpriteState = new SpriteScript[FIELD_SIZE_X, FIELD_SIZE_Y];

    void Start()
    {
        bottonProcess = BottonSystem.GetComponent<Botton_process>();
        BlackInformation = bottonProcess.BlackStoneInformation;
        WhiteInformation = bottonProcess.WhiteStoneInformation;
        BlackImage = BlackInformation.GetComponent<Image>();
        BlackColor = BlackImage.color;
        BlackColor.a = 0.0f;
        BlackImage.color = BlackColor;
        WhiteImage = WhiteInformation.GetComponent<Image>();
        WhiteColor = WhiteImage.color;
        WhiteColor.a = 0.0f;
        WhiteImage.color = WhiteColor;

        FieldState[4, 3] = SpriteState.White;
        FieldState[3, 4] = SpriteState.White;
        FieldState[3, 3] = SpriteState.Black;
        FieldState[4, 4] = SpriteState.Black;

        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            {
                var sprite = Instantiate(OthelloSprite, new Vector3(1.0f * x, 0, 1.0f * y), Quaternion.Euler(90, 0, 0));
                FieldStateNone[x, y] = SpriteState.None;
                if (!(x==3||x==4)&&!(y==3||y==4)) FieldState[x, y] = SpriteState.None;

                FieldSpriteState[x, y] = sprite.GetComponent<SpriteScript>();
                FieldSpriteState[x, y].SetState(FieldState[x, y]);
                FieldStateStart[x, y] = FieldState[x, y];
                FieldStateCustum[x, y] = FieldState[x, y];
            }
        }    

    }

    void Update()
    {

        #region 座標設定
        var position = Cube.transform.localPosition;
        if (EventSystem.current.currentSelectedGameObject == firstSelectedGameObject && Time.time - lastMoveTime >= moveDelay)
        {
            Cube_Controll = true;

            if (Input.GetKey(KeyCode.RightArrow) && Cube_Position_X < 7)
            {
                Cube_Position_X++;
                Cube.transform.localPosition = new Vector3(position.x + 1.0f, position.y, position.z);
                lastMoveTime = Time.time;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && Cube_Position_X > 0)
            {
                Cube_Position_X--;
                Cube.transform.localPosition = new Vector3(position.x - 1.0f, position.y, position.z);
                lastMoveTime = Time.time;
            }
            else if (Input.GetKey(KeyCode.UpArrow) && Cube_Position_Y < 7)
            {
                Cube_Position_Y++;
                Cube.transform.localPosition = new Vector3(position.x, position.y, position.z + 1.0f);
                lastMoveTime = Time.time;
            }
            else if (Input.GetKey(KeyCode.DownArrow) && Cube_Position_Y > 0)
            {
                Cube_Position_Y--;
                Cube.transform.localPosition = new Vector3(position.x, position.y, position.z - 1.0f);
                lastMoveTime = Time.time;
            }
        }


        if (Input.GetKeyDown(KeyCode.LeftArrow) && (!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) && Cube_Position_X == 0) Cube_Controll = false;
        else if (Input.GetKeyDown(KeyCode.RightArrow) && (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) && Cube_Position_X == 7) Cube_Controll = false;

        if (Cube_Controll) EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);
        #endregion

        if (!bottonProcess.CustumHantei) GameMode();
        else CustumMode();



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
    private void GameMode()
    {
        var turnCheck = false;
        if (EventSystem.current.currentSelectedGameObject == firstSelectedGameObject)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                for (int i = 0; i <= 7; i++)
                {
                    if (TurnCheck(i, Cube_Position_X, Cube_Position_Y)) turnCheck = true;
                }
                for (int x = 0; x < FIELD_SIZE_X; x++)
                {
                    for (int y = 0; y < FIELD_SIZE_Y; y++)
                    {
                        if (FieldState[x, y] == SpriteState.NoneChoice) FieldState[x, y] = SpriteState.None;
                    }


                }
                if (turnCheck && (FieldState[Cube_Position_X, Cube_Position_Y] == SpriteState.None || FieldState[Cube_Position_X, Cube_Position_Y] == SpriteState.NoneChoice))
                {
                    StonePutAudio.Play();
                    foreach (var info in _infoList)
                    {
                        var position_X = info.Item1;
                        var position_Y = info.Item2;
                        FieldState[position_X, position_Y] = PlayerTurn;

                    }
                    FieldState[Cube_Position_X, Cube_Position_Y] = PlayerTurn;
                    PlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.White : SpriteState.Black;

                }
                else
                {
                    if(!IsFirstPutAudio) NotPutAudio.Play();
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
                    if (TurnCheck(i, x, y) && (FieldState[x, y] == SpriteState.None|| FieldState[x, y] == SpriteState.NoneChoice))
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
            PlayerTurn = PlayerTurn == SpriteState.Black ? SpriteState.White : SpriteState.Black;
        }
        _infoList = new List<(int, int)>();
        UpdateBoard();

    }
    private void UpdateBoard()
    {   var WhiteNum = default(int);
        var BlackNum = default(int);
        IsFirstPutAudio = false;
        for (int x = 0; x < FIELD_SIZE_X; x++)
        {
            for (int y = 0; y < FIELD_SIZE_Y; y++)
            {
                    if (FieldState[x, y] == SpriteState.White) WhiteNum++;
                else if (FieldState[x, y] == SpriteState.Black) BlackNum++;
            }


        }
        ShowSpriteBoard();
        if (PlayerTurn == SpriteState.White) ColorChange(0.0f, 1.0f);
        else if (PlayerTurn == SpriteState.Black) ColorChange(1.0f, 0.0f);


        BlackNumText.text = BlackNum.ToString();
        WhiteNumText.text = WhiteNum.ToString();
        if (WhiteNum + BlackNum == 64 || !WhiteCheckFlag && !BlackCheckFlag) GameOver(WhiteNum, BlackNum);
    }
    private void GameOver(int WhiteNum, int BlackNum)
    {
        GameObject Black = WinTextObj.transform.Find("Black").gameObject;
        GameObject White = WinTextObj.transform.Find("White").gameObject;
        Cube.SetActive(false);
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

    private bool TurnCheck(int Direction, int field_size_x, int field_size_y)
    {
        var turnCheck = false;
        var position_x = field_size_x;
        var position_y = field_size_y;
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


            if (FieldState[position_x, position_y] == OpponentPlayerTurn)
            {
                infoList.Add((position_x, position_y));
            }
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
    

    public void ResetBoardGame()
    {
        
        ResetBoardProcess(FieldState, FieldStateCustum);
    }
    public void ResetBoardCustum()
    {
        ResetBoardProcess(FieldState, FieldStateStart);
    }
    public void ResetBoardReturn()
    {
        Cube.SetActive(false);
        ResetBoardProcess(FieldState, FieldStateNone);
        ResetBoardProcess(FieldStateCustum, FieldStateStart);
        FieldState[4, 3] = SpriteState.White;
        FieldState[3, 4] = SpriteState.White;
        FieldState[3, 3] = SpriteState.Black;
        FieldState[4, 4] = SpriteState.Black;
        

    }

    void ColorChange(float Black, float White)
    {
        BlackColor.a = Black;
        BlackImage.color = BlackColor;
        WhiteColor.a = White;
        WhiteImage.color = WhiteColor;
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
    
}
