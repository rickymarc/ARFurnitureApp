using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerLogin : MonoBehaviour
{
    public Canvas canvasReg;

    ScriptLogin ScriptLogin;

    void Start()
    {
        ScriptLogin = GetComponent<ScriptLogin>();
        OnClickCloseRegister();
    }

    public void OnClickCloseRegister(){
        ScriptLogin.clearRegForm();
        canvasReg.gameObject.SetActive(false);
    }

    public void OnClickOpenRegister(){
        canvasReg.gameObject.SetActive(true);
    }
}
