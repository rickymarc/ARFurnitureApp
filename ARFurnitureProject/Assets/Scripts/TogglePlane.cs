using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

[RequireComponent(typeof(ARPlaneManager))]
public class TogglePlane : MonoBehaviour
{
    private ARPlaneManager arPlaneManager;
    [SerializeField]
    private Text textButton;
    private void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
    }

    public void SetPlaneToActive(bool status){
        foreach(var planeState in arPlaneManager.trackables){
            planeState.gameObject.SetActive(status);
        }
    }

    public void toggleDetection(){
        arPlaneManager.enabled = !arPlaneManager.enabled;
        
        if(arPlaneManager.enabled){
            textButton.text = "Plane Off";
            SetPlaneToActive(true);
        }else{
            textButton.text = "Plane On";
            SetPlaneToActive(false);
        }
    }
}
