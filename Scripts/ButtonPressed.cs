using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isBeingPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isBeingPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isBeingPressed = false;
    }
}
