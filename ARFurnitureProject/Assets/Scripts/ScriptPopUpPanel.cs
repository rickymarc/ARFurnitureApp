using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptPopUpPanel : MonoBehaviour
{
    public GameObject popUpPanel;
    public void OnClickClose(){
        if(popUpPanel.activeInHierarchy == true){
            popUpPanel.SetActive(false);
        }
    }
}
