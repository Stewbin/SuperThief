using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels; 
using System; 
using System.Linq; 
using PlayFab;

public class PlayFabFriendManager : MonoBehaviour
{
    public static Action<List<FriendInfo>> OnFriendListUpdated = delegate { }; 
    private List<FriendInfo> friends; 
    private void Awake()
    {
        //PhotonConnector.GetPhotonFriends += HandleGetFriends;
        friends = new  List<FriendInfo>(); 
        FriendManager.OnAddFriend += HandleAddPlayfabFriend;
        UIFriend.OnRemoveFriend += HandleRemoveFriend; 
    }

    private void OnDestroy()
    {
       // PhotonConnector.GetPhotonFriends -= HandleGetFriends;
        FriendManager.OnAddFriend -= HandleAddPlayfabFriend;
        UIFriend.OnRemoveFriend -= HandleRemoveFriend;
 
    }

    private void HandleAddPlayfabFriend(string name)
    {
        var request = new AddFriendRequest { FriendTitleDisplayName = name };
        PlayFabClientAPI.AddFriend(request, OnFriendAddedSuccess, OnFailure);
    }

    private void HandleRemoveFriend(string name)
    {
        string id = friends.FirstOrDefault(f => f.TitleDisplayName == name).FriendPlayFabId;
        var request = new RemoveFriendRequest { FriendPlayFabId = id};
        PlayFabClientAPI.RemoveFriend(request, OnFriendRemoveSuccess, OnFailure);
    }

   private void HandleGetFriends()
   {
    GetPlayfabFriends(); 
   }

    private void GetPlayfabFriends()
    {
        var request = new GetFriendsListRequest { ProfileConstraints = new PlayerProfileViewConstraints { ShowDisplayName = true } };
        PlayFabClientAPI.GetFriendsList(request, OnFriendsListSuccess, OnFailure);
    }

    private void OnFriendAddedSuccess(AddFriendResult result)
    {
        Debug.Log("Successfully added this friend");
        GetPlayfabFriends(); // Refresh the friend list after adding a friend
    }

    private void OnFriendsListSuccess(GetFriendsListResult result)
    {
        friends = result.Friends; 
        OnFriendListUpdated?.Invoke(result.Friends);
    }

      private void OnFriendRemoveSuccess(RemoveFriendResult result)
    {
        GetPlayfabFriends();
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.LogError($"Error occurred: {error.GenerateErrorReport()}");
    }
}