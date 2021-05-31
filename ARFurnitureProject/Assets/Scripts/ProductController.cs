using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.Globalization;

public static class ButtonClick{
    public static void AddEventListener<T>(this Button btn, T prm, Action<T> OnClick){
        btn.onClick.AddListener(
            delegate(){
               OnClick(prm); 
            }
        );
    }
}

public class ProductController : MonoBehaviour
{
    [Serializable]
    public struct Product{
        public string name;
        public Sprite image;
        public string imageUrl;
        public int price;
        public string dimension;
        public string description;
        public ManufacturerData manufacturer;
        
    }

    [Serializable]
    public struct ManufacturerData{
        public string manufacturer_name;
        public string web_url;
    }

    [SerializeField] Product[] products;
    public Button btn;
    public Image loading;

    public Image prodImage;
    public Text nameText;
    public Text priceText;
    public Text dimensionText;
    public GameObject popUpPanel;
    public Text textManName;
    public Text descText;

    public ScrollRect scrollRect;

    // Start is called before the first frame update
    void Start()
    {
        loadStart();
        StartCoroutine(GetJson());
    }

    void CreateInterface(){
        GameObject productTemplate = transform.GetChild(0).gameObject;
        GameObject bfr;

        for(int i = 0; i<products.Length ; i++){
            bfr = Instantiate (productTemplate, transform);
            bfr.transform.GetChild(0).GetComponent<Image>().sprite = products[i].image;
            bfr.transform.GetChild(1).GetComponent<Text>().text = products[i].name;

            bfr.GetComponent<Button>().AddEventListener(i,ProductClicked);
        }
        Destroy(productTemplate);

    }

    void ProductClicked(int index){
        Canvas.ForceUpdateCanvases();
        scrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical() ;
        scrollRect.content.GetComponent<ContentSizeFitter>().SetLayoutVertical() ;
        scrollRect.verticalNormalizedPosition = 0 ;

        if(popUpPanel.activeInHierarchy == false){
            popUpPanel.gameObject.SetActive(true);
        }

        CultureInfo cultureID = new CultureInfo("id-ID");

        nameText.text = products[index].name;
        prodImage.sprite = products[index].image;
        priceText.text = String.Format(cultureID,"{0:C2}", products[index].price); 
        dimensionText.text = products[index].dimension;
        descText.text = "\""+products[index].description+"\"";
        textManName.text = "By "+products[index].manufacturer.manufacturer_name;
    }

    IEnumerator GetJson(){
        string alink = "https://funitureapp-832b5-default-rtdb.firebaseio.com/products.json";
        UnityWebRequest req = UnityWebRequest.Get(alink);
        yield return req.SendWebRequest();

        if(req.isHttpError){
            Debug.LogError(req.error);

        }else if (req.isDone){
           products = JsonHelper.GetArray<Product>(req.downloadHandler.text);
           StartCoroutine(GetImagesUrl());
        }
    }

    IEnumerator GetImagesUrl(){
        for(int i = 0; i<products.Length ; i++){
            using(UnityWebRequest www = UnityWebRequestTexture.GetTexture(products[i].imageUrl)){
              yield return www.SendWebRequest();
                if(www.isHttpError == true){
                    Debug.Log(www.error);
                }else{
                    Texture2D texture = new Texture2D(1, 1);
                    texture = DownloadHandlerTexture.GetContent(www);
                    products[i].image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }    
            }

        }
        btnStart();
        CreateInterface();
    }

    public void loadStart(){
        loading.gameObject.SetActive(true);
        btn.gameObject.SetActive(false);
        popUpPanel.gameObject.SetActive(false);
    }

    public void btnStart(){
        loading.gameObject.SetActive(false);
        btn.gameObject.SetActive(true);
    }

}
