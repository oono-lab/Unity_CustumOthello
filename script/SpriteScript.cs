using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SpriteScript : MonoBehaviour
{

    public GameObject CubePoint;
    public GameObject WallObjMaterial;
    public GameObject ChoicePoint;
    private GameObject WallObj;
    private GameObject ChoiceObj;
    private bool WallHantei = false;
    private bool ChoiceHantei = false;

    public void SetState(OthelloScript.SpriteState state)
    {   
        var isActive = state != OthelloScript.SpriteState.None;
        gameObject.SetActive(isActive);
        if (state == OthelloScript.SpriteState.White)
        {
            gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
            #region　何も置けない状態と置けるマスの状態のオブジェクトを削除する。
            if (WallHantei)
            {
                Destroy(WallObj);
                WallHantei = false;
            }
            else if (ChoiceHantei)
            {
                Destroy(ChoiceObj);
                ChoiceHantei = false;
            }
            #endregion
        }
        else if(state == OthelloScript.SpriteState.Black)
        {
            gameObject.transform.rotation = Quaternion.Euler(270, 0, 0);
            #region　何も置けない状態と置けるマスの状態のオブジェクトを削除する。
            if (WallHantei)
            {
                Destroy(WallObj);
                WallHantei = false;
            }
            else if (ChoiceHantei)
            {
                Destroy(ChoiceObj);
                ChoiceHantei = false;
            }
            #endregion
        }
        else if(state == OthelloScript.SpriteState.Wall)
        {   #region　何も置けない状態のオブジェクトを表示する。
            if(!WallHantei) 
            {
                WallObjMaterial.transform.position = new Vector3(
                    CubePoint.transform.localPosition.x,
                    CubePoint.transform.localPosition.y - 0.01f,
                    CubePoint.transform.localPosition.z
                );
                WallObj = Instantiate(WallObjMaterial);
                WallObj.SetActive(true);
                WallHantei = true;
            }
            gameObject.SetActive(false);
            #endregion
        }
        else if (state == OthelloScript.SpriteState.NoneChoice)
        {    #region　置けるマスの状態のオブジェクトを表示する。
            if (!ChoiceHantei)
            {
                ChoicePoint.transform.position = new Vector3(
                    gameObject.transform.localPosition.x,
                    gameObject.transform.localPosition.y,
                    gameObject.transform.localPosition.z
                );
                ChoiceObj = Instantiate(ChoicePoint);
                ChoiceObj.SetActive(true);
                ChoiceHantei = true;
            }
            gameObject.SetActive(false);
            #endregion
        }
        else
        {
            gameObject.SetActive(false);
            
            #region　何も置けない状態と置けるマスの状態のオブジェクトを削除する。
            if (WallHantei)
            {   
                Destroy(WallObj);
                WallHantei = false;
            }else if (ChoiceHantei)
            {
                Destroy(ChoiceObj);
                ChoiceHantei = false;
            }

            #endregion
        }
    }
    


}
