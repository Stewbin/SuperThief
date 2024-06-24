using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerNavigator : MonoBehaviourPunCallbacks
{
    public float speed = 5f; // Movement speed

    private void Awake()
    {
        // Disable the script for other players
        if (!photonView.IsMine)
        {
            enabled = false;
        }
    }

    void Start()
    {
        // Lock the cursor for the local player
        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Calculate movement vector
            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * speed * Time.deltaTime;

            // Move the cube
            transform.Translate(movement);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Cursor.lockState == CursorLockMode.None)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
    }
}