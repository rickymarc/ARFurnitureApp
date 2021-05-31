using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptLoadRotate : MonoBehaviour
{
    public RectTransform iconM;
    public float timeStep; 
    public float angleStep;
    float timeStart;
    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - timeStart >= timeStep){
            Vector3 iconAngle = iconM.localEulerAngles;
            iconAngle.z += angleStep;

            iconM.localEulerAngles = iconAngle;
            timeStart = Time.time;
        }
    }
}
