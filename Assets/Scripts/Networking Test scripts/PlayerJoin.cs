using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using Unity.Netcode;
using Unity.Networking.Transport;
using System;

public class PlayerJoin : NetworkBehaviour
{
   // [Rpc(SendTo.Everyone)]
    private void JoinRpc()
    {
        Debug.Log("joined");
    }

    private void Start()
    {
        if (IsClient && !IsServer)
            JoinRpc();
    }
}
