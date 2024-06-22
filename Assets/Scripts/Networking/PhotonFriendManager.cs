using System.Collections;
using System.Collections.Generic;
using System;
using Photon.Pun;
using PhotonFriendInfo = Photon.Realtime.FriendInfo;
using PlayFabFriendInfo = PlayFab.ClientModels.FriendInfo;
using UnityEngine;
using System.Linq;
using PlayFab;

public class PhotonFriendManager : MonoBehaviourPunCallbacks
{
    public static Action<List<PhotonFriendInfo>> OnDisplayFriends = delegate { };

    private List<PlayFabFriendInfo> friends = new List<PlayFabFriendInfo>();

    private void Awake()
    {
        PlayFabFriendManager.OnFriendListUpdated += HandleFriendsUpdated;
    }

    private void OnDestroy()
    {
        PlayFabFriendManager.OnFriendListUpdated -= HandleFriendsUpdated;
    }

    private void HandleFriendsUpdated(List<PlayFabFriendInfo> friendList)
    {
        friends = friendList;

        if (friends.Count != 0)
        {
            string[] friendsDisplayNames = friends.Select(f => f.TitleDisplayName).ToArray();
            PhotonNetwork.FindFriends(friendsDisplayNames);
        } else {

            List <PhotonFriendInfo> friendsList = new List<PhotonFriendInfo>();
            OnDisplayFriends?.Invoke(friendsList);

        }
    }

    public override void OnFriendListUpdate(List<PhotonFriendInfo> friendList)
    {
        OnDisplayFriends?.Invoke(friendList);
    }
}