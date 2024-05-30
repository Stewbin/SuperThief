using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputField : MonoBehaviour
{
    [SerializeField] private string roomNameText;
    //[SerializeField] private string EmailText;

    public void GetRoomNameText() {}
   

    public void GetRoomNameText(string input)
    {
        // Validate username

        // Set username
        roomNameText = input;
    }
    
}
