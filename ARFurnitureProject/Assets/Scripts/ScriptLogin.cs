using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Text;

public class ScriptLogin : MonoBehaviour
{
    [Serializable]
    public struct DataLogin{
        public string email;
        public string password;
        public string userId;
    }

    [SerializeField] DataLogin[] allDataLogins;

    public Button buttonLogin;
    public InputField emailInput;
    public InputField passInput;
    
    public Text warningText;
    public Text registerWarningtext;

    public InputField emailInputReg;
    public InputField passInputReg;

    UIControllerLogin scriptUILogin;

    public string urlUsers = "https://funitureapp-832b5-default-rtdb.firebaseio.com/users.json";
    
    void Start()
    {   
        clearRegForm();
        scriptUILogin = GetComponent<UIControllerLogin>();
        buttonLogin.onClick.AddListener(calls);
        passInput.text ="";
        StartCoroutine(GetUserData());
    }

    void calls(){
        StartCoroutine(GetUserData());
        clearText();
        LoginCheck();
    }

    IEnumerator GetUserData(){
        
        UnityWebRequest www = UnityWebRequest.Get(urlUsers);
        yield return www.SendWebRequest();

            if(www.isHttpError){
                Debug.Log("UnityWebRequest Error :" + www.error);
            }else{
                allDataLogins = JsonHelper.GetArray<DataLogin>(www.downloadHandler.text);
            }

    }

    void LoginCheck(){
        for(int i =0 ; i<allDataLogins.Length;i++){
            if(allDataLogins[i].email.Equals(emailInput.text) && allDataLogins[i].password.Equals(passInput.text)){          
                Debug.Log("Success :" + emailInput.text );
                if(warningText.isActiveAndEnabled == true){
                    warningText.gameObject.SetActive(false);
                }
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1 );
            }else{
                if(warningText.isActiveAndEnabled == false){
                    warningText.gameObject.SetActive(true);
                }
                warningText.text = "Email or Pass is Incorrect / User Doesn't Exist";
            }
        }
    }
    
    public void CallRegister(){
        StartCoroutine(GetUserData());
        for(int i = 0;i< allDataLogins.Length;i++){
            if(allDataLogins[i].email.Equals(emailInputReg.text)){
                registerWarningtext.GetComponent<Text>().color = Color.red;
                registerWarningtext.text = "Account Already Registered";
                passInputReg.text = "";
                Debug.Log(allDataLogins[i].email);
                return;
            }
        }
        StartCoroutine(StartRegister());
        Debug.Log(emailInputReg.text);
    }
    
    IEnumerator StartRegister(){
        string lastUserId = allDataLogins[allDataLogins.Length-1].userId;
        string idTemp =lastUserId.Replace("user","");
        int idGet = int.Parse(idTemp)+1;

        int arrPos =allDataLogins.Length;

        string header ="{\"email\" : \"" +emailInputReg.text+"\",\"password\" : \""+passInputReg.text+"\",\"userId\" : \""+"user"+idGet.ToString()+"\"}";
        UnityWebRequest ww = new UnityWebRequest("https://funitureapp-832b5-default-rtdb.firebaseio.com/users/"+arrPos.ToString()+".json","PUT");
        byte[] rawData =  System.Text.Encoding.ASCII.GetBytes(header);
        ww.uploadHandler = (UploadHandler) new UploadHandlerRaw(rawData);
        ww.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        ww.SetRequestHeader("Content-Type", "application/json");
        yield return ww.SendWebRequest();
            if(ww.isHttpError){
                Debug.Log("UnityWebRequest Error :" + ww.error);
            }else{
                registerWarningtext.GetComponent<Text>().color = Color.green;
                registerWarningtext.text = "Register Success";
                passInputReg.text = "";
                clearText();
                StartCoroutine(GetUserData());
            }   
    }

    void clearText(){
        passInputReg.text = "";
        emailInputReg.text = "";
    }

    public void clearRegForm(){
        passInputReg.text = "";
        emailInputReg.text = "";
        registerWarningtext.text ="";
    }

}
