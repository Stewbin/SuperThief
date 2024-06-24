using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class ButtonController : MonoBehaviourPunCallbacks, IPointerDownHandler, IPointerUpHandler
{
    public bool IsPressed { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(photonView.IsMine){
        IsPressed = true;
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
       if(photonView.IsMine){
        IsPressed = false;
        }

    }
}