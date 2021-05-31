using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderRotateScript : MonoBehaviour
{

    public GameObject prefabRotate;
    public Slider rotateSlider;
    public float minRValue =0;
    public float maxRValue= 360;
    void Start()
    {
        rotateSlider.minValue = minRValue;
        rotateSlider.maxValue = maxRValue;

        rotateSlider.onValueChanged.AddListener(RotateSlider);
    }

    public void SetPrefab(GameObject pf){
        prefabRotate = pf;
    }

    void RotateSlider(float value){
        prefabRotate.transform.localEulerAngles = new Vector3(prefabRotate.transform.rotation.x, value,prefabRotate.transform.rotation.z);
    }
}
