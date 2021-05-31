using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Globalization;


[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObjectTap : MonoBehaviour
{

    private ARRaycastManager rayManager;
    private Vector2 touchPos = default;

    private List<GameObject> placedPFList = new List<GameObject>();
    
    static List<ARRaycastHit> rayHit = new List<ARRaycastHit>();

    [SerializeField]
    private GameObject placedPF;
    [SerializeField]
    private Camera cameraThis;

    private GameObject objectSpawned;

    public Canvas canvasSetPrefab;
    
    public Canvas uiCanvas;
    public Canvas tableCanvas;

    SliderRotateScript sliderScript;

    public Canvas popUpCanvas;

    SceneController sceneControllerScript;
    public string nameTemp;
    public int priceTemp; 

    public GameObject rowTable;
    public Transform rowParent;

    public List<GameObject> tableTextList = new List<GameObject>(); 

    public Text texttotalPrice;
    int totalPrice;
    
    CultureInfo cultureID = new CultureInfo("id-ID");
    public Button btnCancel;

    bool isUndoNew = false;

    private void Awake()
    {
        rayManager = GetComponent<ARRaycastManager>();
    }

    private void Start()
    {
        sceneControllerScript = GameObject.FindGameObjectWithTag("sliderPrefab").GetComponent<SceneController>();
        sliderScript = GameObject.FindGameObjectWithTag("sliderPrefab").GetComponent<SliderRotateScript>();
        DeactivateSetCanvas();
    }
    
    bool GetPos(out Vector2 touchPos){
        if(Input.touchCount > 0){
            touchPos = Input.GetTouch(0).position;
            if(!IsPointOverUIObject(touchPos) && Input.GetTouch(0).phase == TouchPhase.Began){
                Ray ray = cameraThis.ScreenPointToRay(touchPos);
                RaycastHit objHit;
                if(Physics.Raycast(ray, out objHit)){
                    objectSpawned = objHit.collider.transform.gameObject;
                    isUndoNew = false;
                    ObjectSelected();
                }
            }
            return true;
        }
        touchPos = default;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        //jika tidak mendeteksi sentuhan
        if(!GetPos(out Vector2 touchPos)){
            return;
        }

        if( !IsPointOverUIObject(touchPos) && rayManager.Raycast(touchPos,rayHit, TrackableType.PlaneWithinPolygon)){
            var hitPose = rayHit[0].pose;

            SpawnPF(hitPose);
        }
    }

    public void SetPrefabType(GameObject pfType){
        placedPF = pfType;
    }

    public void SetPrefabInfo(String name, int price ){
        nameTemp = name;
        priceTemp = price;
    }

    public void PriceClick(int Count){
        GameObject temp = rowTable.gameObject;
        GameObject bfr;

        bfr = Instantiate (temp, rowParent);
        bfr.transform.GetChild(0).GetComponent<Text>().text = Count.ToString();
        bfr.transform.GetChild(1).GetComponent<Text>().text = nameTemp;
        bfr.transform.GetChild(2).GetComponent<Text>().text = String.Format(cultureID,"{0:C2}", priceTemp);
        tableTextList.Add(bfr);
        totalPrice += priceTemp;
        SetPrice(totalPrice);
    }


    public void SpawnPF(Pose hitPose){
        if(placedPF != null){
            objectSpawned = Instantiate(placedPF,hitPose.position, hitPose.rotation);
            placedPFList.Add(objectSpawned);
            placedPF = null;
            isUndoNew = true;
            ObjectSelected();
            PriceClick(placedPFList.Count);
        }else{
            objectSpawned.transform.position = hitPose.position;
        }
    }

    void SetPrice(int price){
        texttotalPrice.text = String.Format(cultureID,"{0:C2}", price);
    }

    void ObjectSelected(){
        if(!isUndoNew){
            btnCancel.transform.GetChild(0).GetComponent<Text>().text = "Delete";
            btnCancel.transform.GetComponent<Image>().color =Color.red;
        }else{
            btnCancel.transform.GetChild(0).GetComponent<Text>().text = "Cancel";
            btnCancel.transform.GetComponent<Image>().color =Color.white;
        }
        sliderScript.SetPrefab(objectSpawned);
        canvasSetPrefab.gameObject.SetActive(true);
        uiCanvas.gameObject.SetActive(false);
        tableCanvas.gameObject.SetActive(false);
    }

    public void OnClickDestroy(){
        DeletePrefab(placedPFList.Count-1);
    }

    public void OnClickSetPrefab(){
        objectSpawned = null;
        DeactivateSetCanvas();
    }

    public void OnClickCancel(){
        if(!isUndoNew){
            for(int i = 0;i <placedPFList.Count;i++){
                if(objectSpawned == placedPFList[i].gameObject){
                DeletePrefab(i);
                }
            }
            OnClickSetPrefab();
        }else{
            OnClickDestroy();
            DeactivateSetCanvas();
        }
    }

    void DeletePrefab(int index){
        if (0 < placedPFList.Count){
            Destroy(placedPFList[index].gameObject); 
            placedPFList.RemoveAt(index);
        }

        if (0 < tableTextList.Count){
            String oldValue = tableTextList[index].gameObject.transform.GetChild(2).GetComponentInChildren<Text>().text;
            String newValue = oldValue.Replace(".", string.Empty).Replace("Rp",string.Empty);
            int a =  int.Parse(newValue.Replace(",00",string.Empty));
            totalPrice -= a;

            //numbering table resetting
            for(int i = 0 ;i<tableTextList.Count;i++){
                int tempNum = int.Parse(tableTextList[i].gameObject.transform.GetChild(0).GetComponentInChildren<Text>().text);
                if(index+1< tempNum){
                    tempNum -=1;
                    tableTextList[i].gameObject.transform.GetChild(0).GetComponentInChildren<Text>().text = tempNum.ToString();
                }
            }
                    
            SetPrice(totalPrice);
            Destroy(tableTextList[index].gameObject);
            tableTextList.RemoveAt(index);
        }
    }

    void DeactivateSetCanvas(){
        canvasSetPrefab.gameObject.SetActive(false);
        uiCanvas.gameObject.SetActive(true);
        popUpCanvas.gameObject.SetActive(false);
        tableCanvas.gameObject.SetActive(false);
    }


    bool IsPointOverUIObject(Vector2 pos){
        if (EventSystem.current.IsPointerOverGameObject()){
            return false;
        }

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(pos.x, pos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;

    }
    public void OnClickBackPopUp(){
        if(0 < placedPFList.Count){
            popUpCanvas.gameObject.SetActive(true);
            canvasSetPrefab.gameObject.SetActive(false);
            uiCanvas.gameObject.SetActive(false);
            tableCanvas.gameObject.SetActive(false);
        }else{
            sceneControllerScript.OnClickBack();
        }
    }

    public void OnClickTableCanvas(){
        if(tableCanvas.isActiveAndEnabled){
            DeactivateSetCanvas();
        }else{
            popUpCanvas.gameObject.SetActive(false);
            canvasSetPrefab.gameObject.SetActive(false);
            tableCanvas.gameObject.SetActive(true);
        }
    }

    public void OnClickCancelPopUp(){
        DeactivateSetCanvas();
    }
}
