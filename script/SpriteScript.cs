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
        }
        else if(state == OthelloScript.SpriteState.Black)
        {
            gameObject.transform.rotation = Quaternion.Euler(270, 0, 0);
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
        }
        else if(state == OthelloScript.SpriteState.Wall)
        {   if(!WallHantei) 
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

        }
        else if (state == OthelloScript.SpriteState.NoneChoice)
        {
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

        }
        else
        {
            gameObject.SetActive(false);

            if (WallHantei)
            {   
                Destroy(WallObj);
                WallHantei = false;
            }else if (ChoiceHantei)
            {
                Destroy(ChoiceObj);
                ChoiceHantei = false;
            }


        }
    }
    


}
