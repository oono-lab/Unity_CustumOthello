using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectControl : MonoBehaviour
{
    private GameObject CurrentSelectObj;
    // Start is called before the first frame update

    void Update()
    {
         
        if (Input.GetMouseButtonDown(0)) EventSystem.current.SetSelectedGameObject(CurrentSelectObj);
        else CurrentSelectObj = EventSystem.current.currentSelectedGameObject;

    }
}
