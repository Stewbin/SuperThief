using System;
using UnityEngine;

public class FriendManager : MonoBehaviour
{
   [SerializeField] private string displayName; 

   public static Action <string> OnAddFriend = delegate {}; 

   public void SetAddFriendName(string name)
   {
    displayName = name; 
   }

    public void AddFriend()
   {
    if(string.IsNullOrEmpty(displayName)) return; 
    OnAddFriend?.Invoke(displayName); 
   }

}
