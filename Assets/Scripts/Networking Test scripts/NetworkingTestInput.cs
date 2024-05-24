using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Networking;
using Unity.Netcode;

public class NetworkingTestInput : NetworkBehaviour
{
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            Destroy(this);
    }

    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.right * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.left * Time.deltaTime);
        }
    }
}
