using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.Globalization;
using System.IO;

public static class ModelClick{
    public static void AddEvent<T>(this Button button, T param, Action<T> OnClick){
        button.onClick.AddListener(
            delegate(){
               OnClick(param); 
            }
        );
    }
}
public class ModelController : MonoBehaviour
{
    [Serializable]
    public struct Model{
        public string name;
        public Sprite image;
        public string imageUrl;
        public string assetName;
        public int price;

        public ManufacturerData manufacturer;
        
    }

    [Serializable]
    public struct ManufacturerData{
        public string manufacturer_name;
    }

    PlaceObjectTap placeObject;

    [SerializeField] Model[] allModel;

    AssetBundle myLoadedAssetBundle;

    public Button btn;
    public Image loading;

    void Start()
    {
        loadStart();
        placeObject = GameObject.FindGameObjectWithTag("placedObj").GetComponent<PlaceObjectTap>();
        StartCoroutine(GetJson());
    }

    IEnumerator GetJson(){
        string alink = "https://funitureapp-832b5-default-rtdb.firebaseio.com/products.json";
        UnityWebRequest req = UnityWebRequest.Get(alink);
        yield return req.SendWebRequest();

        if(req.isHttpError){
            Debug.LogError(req.error);

        }else if (req.isDone){
           allModel = JsonHelper.GetArray<Model>(req.downloadHandler.text);
           StartCoroutine(GetImagesUrl());
        }
    }

    void FindAssetBundle(){
        myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "futnitureasset"));
        if (myLoadedAssetBundle == null){
             Debug.Log("error");
        }
    }

    void CreateInterface(){
        GameObject productTemplate = transform.GetChild(0).gameObject;
        GameObject bfr;

        for(int i = 0; i<allModel.Length ; i++){
            bfr = Instantiate (productTemplate, transform);
            bfr.transform.GetChild(0).GetComponent<Image>().sprite = allModel[i].image;
            bfr.transform.GetChild(1).GetComponent<Text>().text = allModel[i].name;
       
            bfr.GetComponent<Button>().AddEvent(i,ModelClicked);
        }
        Destroy(productTemplate);

    }

    // void ClickedOne(int index){
    //     StartCoroutine(ModelClicked(index));
    // }

    void ModelClicked(int index){
        placeObject.SetPrefabInfo(allModel[index].manufacturer.manufacturer_name+"\'s " + allModel[index].name,allModel[index].price);
        // placeObject.PriceClick();
        string assetss = allModel[index].assetName;
        Debug.Log(assetss);
        FindAssetBundle();
        GameObject prefabChoice = myLoadedAssetBundle.LoadAsset<GameObject>(assetss);
        placeObject.SetPrefabType(prefabChoice);
        myLoadedAssetBundle.Unload(false);
    }


    IEnumerator GetImagesUrl(){
        for(int i = 0; i<allModel.Length ; i++){
            using(UnityWebRequest www = UnityWebRequestTexture.GetTexture(allModel[i].imageUrl)){
                yield return www.SendWebRequest();
                    if(www.isHttpError == true){
                        Debug.Log(www.error);
                    }else{
                        Texture2D texture = new Texture2D(1, 1);
                        texture = DownloadHandlerTexture.GetContent(www);
                        allModel[i].image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
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
