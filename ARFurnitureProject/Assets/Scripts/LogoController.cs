using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;


public class LogoController : MonoBehaviour
{
    [Serializable]
    public struct Manufacturer{
        public Sprite logo;
        public string logo_image;
        public string address;
        public string manufacturer_name;
        public string web_url;
        public string manufacture_description;

    }
    [SerializeField] Manufacturer[] logoGroup;

    public Button btn;
    public Image loading;

    public Text manNameText;
    public Text manAddress;
    public Text manweburl;
    public Text mandesc;

    public Image logoImage;

    public Image popUpManufacturer;

    public ScrollRect scrollRect;
    
    void Start()
    {
        loadStart();
        StartCoroutine(GetJson());
    }
    
    void CreateInterface(){
        GameObject logoTemplate = transform.GetChild(0).gameObject;
        GameObject bfr;


        for(int i = 0; i<logoGroup.Length;i++){
            bfr = Instantiate(logoTemplate, transform);
            bfr.transform.GetChild(0).GetComponent<Image>().sprite = logoGroup[i].logo;

            bfr.GetComponent<Button>().AddEventListener(i,OnClickManufacture);
        }
        
        Destroy(logoTemplate);
    }

    void OnClickManufacture(int index){
        Canvas.ForceUpdateCanvases();
        scrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        scrollRect.content.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        scrollRect.verticalNormalizedPosition = 0 ;
        popUpManufacturer.gameObject.SetActive(true);

        manNameText.text = logoGroup[index].manufacturer_name;
        manweburl.text = logoGroup[index].web_url;
        manAddress.text =  logoGroup[index].address;
        mandesc.text = logoGroup[index].manufacture_description;
        logoImage.sprite =  logoGroup[index].logo;
    }
    
    IEnumerator GetJson(){
        string alink = "https://funitureapp-832b5-default-rtdb.firebaseio.com/manufacturer.json";
        UnityWebRequest req = UnityWebRequest.Get(alink);
        yield return req.SendWebRequest();

        if(req.isHttpError){
            Debug.LogError(req.error);

        }else if (req.isDone){
           logoGroup = JsonHelper.GetArray<Manufacturer>(req.downloadHandler.text);
           StartCoroutine(GetImagesUrl());
        }
    }

    IEnumerator GetImagesUrl(){
        for(int i = 0; i<logoGroup.Length ; i++){
    
            using(UnityWebRequest www = UnityWebRequestTexture.GetTexture(logoGroup[i].logo_image)){
              yield return www.SendWebRequest();
                if(www.isHttpError == true){
                    Debug.Log(www.error);
                }else{
                    Texture2D texture = new Texture2D(1, 1);
                    texture = DownloadHandlerTexture.GetContent(www);
                    logoGroup[i].logo = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }    
            }

        }
        btnStart();
        CreateInterface();
    }

    public void loadStart(){
        loading.gameObject.SetActive(true);
        btn.gameObject.SetActive(false);
    }

    public void btnStart(){
        loading.gameObject.SetActive(false);
        btn.gameObject.SetActive(true);
    }

}
