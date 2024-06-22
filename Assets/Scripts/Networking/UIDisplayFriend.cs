using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class UIDisplayFriend : MonoBehaviour
{
    [SerializeField] private Transform friendContainer;
    [SerializeField] private UIFriend uiFriendPrefab;

    private void Awake()
    {
        PhotonFriendManager.OnDisplayFriends += HandleDisplayFriends;
    }

    private void OnDestroy()
    {
        PhotonFriendManager.OnDisplayFriends -= HandleDisplayFriends;
    }

    private void HandleDisplayFriends(List<FriendInfo> friends)
    {
        foreach (Transform child in friendContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (FriendInfo friend in friends)
        {
            UIFriend uiFriend = Instantiate(uiFriendPrefab, friendContainer);
            uiFriend.Initialize(friend);
        }
    }
}