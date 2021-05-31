using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIControllerMenu : MonoBehaviour
{
    public Image logoutCanvas;

    public GameObject contentPopUp;
    public Image manufacturerPopUp;

    private void Start()
    {
        OnClickCancelPopUp();
        OnClickCloseManufacturer();
         
    }
     public void OnClickCloseManufacturer(){
         manufacturerPopUp.gameObject.SetActive(false);
       
    }

    public void OnClickActivatePopUp(){
        logoutCanvas.gameObject.SetActive(true);
    }

    public void OnClickCancelPopUp(){
        logoutCanvas.gameObject.SetActive(false);
    }
}
