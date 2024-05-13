using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinEvents : NetworkBehaviour
{
    // Start is called before the first frame update
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.SceneManager.LoadScene("Networking", LoadSceneMode.Single);
    }

    public void JoinClient()
    {
        NetworkManager.Singleton.StartClient();
        NetworkManager.SceneManager.LoadScene("Networking", LoadSceneMode.Single);
    }
}
