using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToController : MonoBehaviour
{
    public Canvas canvasHowTo;

    public ScrollRect scrollRect;
    void Start()
    {
        CloseHowTo();
    }

    public void CloseHowTo(){
        canvasHowTo.gameObject.SetActive(false);
    }
    public void ActivateHowTo(){
        Canvas.ForceUpdateCanvases();
        scrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical() ;
        scrollRect.content.GetComponent<ContentSizeFitter>().SetLayoutVertical() ;
        scrollRect.verticalNormalizedPosition = 0 ;
        canvasHowTo.gameObject.SetActive(true);
    }


}
