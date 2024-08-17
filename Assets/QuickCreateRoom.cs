using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuickCreateRoom : MonoBehaviourPunCallbacks
{
    public string SceneName;
    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;  // Enable automatic scene synchronization
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnJoinedRoom()
    {
        // A Scene that is not added to the Scenes in Build window returns a buildIndex equal to the size of the list
        if (SceneManager.GetSceneByName(SceneName).buildIndex == SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("The scene you're trying to load is not in the build settings!");
        }
        else
        {
            PhotonNetwork.LoadLevel(SceneName);
        }

        // // Find all objects in the scene with PhotonView components
        // PhotonView[] photonViews = FindObjectsOfType<PhotonView>();

        // // Set the PhotonView's ownership to the local player if it's unassigned
        // foreach (var photonView in photonViews)
        // {
        //     if (photonView.Owner == null)
        //     {
        //         photonView.RequestOwnership();
        //     }
        // }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to create room: " + message);
    }
}
