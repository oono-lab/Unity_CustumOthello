using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineSpriteScript : MonoBehaviourPun
{
    public GameObject WallObjMaterial;
    public GameObject ChoicePoint;
    private GameObject WallObj;
    private GameObject ChoiceObj;
    private bool WallHantei = false;
    private bool ChoiceHantei = false;

    // Start is called before the first frame update
    public void SetState(OnlineOthelloScript.SpriteState state)
    {

        var isActive = state != OnlineOthelloScript.SpriteState.None;
        gameObject.SetActive(isActive);
        if (state == OnlineOthelloScript.SpriteState.White)
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
        else if (state == OnlineOthelloScript.SpriteState.Black)
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
        else if (state == OnlineOthelloScript.SpriteState.Wall)
        {
            //Debug.Log(OnlineOthelloObject);
            if (!WallHantei)
            {
                WallObjMaterial.transform.position = new Vector3(
                    gameObject.transform.localPosition.x,
                    gameObject.transform.localPosition.y-0.01f,
                    gameObject.transform.localPosition.z
                );
                WallObj = Instantiate(WallObjMaterial);
                WallObj.SetActive(true);
                WallHantei = true;
            }
            gameObject.SetActive(false);

        }
        else if (state == OnlineOthelloScript.SpriteState.NoneChoice)
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
            }
            else if (ChoiceHantei)
            {
                Destroy(ChoiceObj);
                ChoiceHantei = false;
            }


        }
    }
}
