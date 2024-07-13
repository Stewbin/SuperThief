using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class ItemManager : MonoBehaviour
{
    public string itemName;
    public int coinsPrice;

    public void BuyItem()
    {
        // First, get the user's current balance
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            if (result.VirtualCurrency.TryGetValue("TC", out int currentBalance))
            {
                if (currentBalance >= coinsPrice)
                {
                    // User has enough coins, proceed with purchase
                    var request = new SubtractUserVirtualCurrencyRequest
                    {
                        VirtualCurrency = "TC",
                        Amount = coinsPrice
                    };
                    PlayFabClientAPI.SubtractUserVirtualCurrency(request, OnSubtractCoinsSuccess, OnError);
                }
                else
                {
                    // Not enough coins
                    Debug.Log("You cannot buy " + itemName + " because of insufficient coins. Current balance: " + currentBalance);
                }
            }
            else
            {
                Debug.Log("Failed to retrieve TC balance");
            }
        }, OnError);
    }

    public void OnSubtractCoinsSuccess(ModifyUserVirtualCurrencyResult result)
    {
        int updatedBalance = result.Balance;

        if (updatedBalance <= 100)
        {
            Debug.Log("You have low coins after buying " + itemName + ". Current balance: " + updatedBalance);
        }
        else
        {
            Debug.Log("Successfully bought " + itemName + ". Remaining balance: " + updatedBalance);
        }
    }

    public void OnError(PlayFabError error)
    {
        Debug.LogError("PlayFab Error: " + error.ErrorMessage);
    }
}