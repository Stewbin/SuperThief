using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputFieldGrabber : MonoBehaviour
{
    [SerializeField] private string UsernameText;
    [SerializeField] private string EmailText;

    public void HandoverUsername() {}
    public void HandoverEmail() {}

    public void GrabUsername(string input)
    {
        // Validate username

        // Set username
        UsernameText = input;
    }

    public void GrabEmail(string input)
    {
        // Validate email

        // Set email
        EmailText = input;
    }
    
}
