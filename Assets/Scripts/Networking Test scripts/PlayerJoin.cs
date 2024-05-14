using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using Unity.Netcode;
using Unity.Networking.Transport;

public class PlayerJoin : NetworkBehaviour
{
    [ClientRpc]
    private void PlayerJoinedClientRPC()
    {
        Debug.Log("joined");
    }

    private void Awake() { }

    public override void OnNetworkSpawn() { }
}
