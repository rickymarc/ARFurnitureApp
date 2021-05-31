using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerAR : MonoBehaviour
{
    public Canvas canvasIntro;


    private void Start()
    {
        canvasIntro.gameObject.SetActive(true);
    }

    public void OnClickDeactivateIntro(){
        canvasIntro.gameObject.SetActive(false);
    }
}
