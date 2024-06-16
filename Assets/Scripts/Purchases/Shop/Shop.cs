/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extensions;
using TMPro;
using System;

[Serializable]
//Consumable Items: coins, gems; 
public class ConsumableItem
{
    public string Name;
    public string Id;
    public string desc;
    public string price;
}

//Non Consumable Items: Character and gun skins, remove ads; 
[Serializable]
public class NonConsumableItem
{
    public string Name;
    public string Id;
    public string desc;
    public string price;
}

//Subscription: battles pass, VIP members; 
[Serializable]
public class SubscriptionItem
{
    public string Name;
    public string Id;
    public string desc;
    public string price;
    public int timeDuration;
}

public class Shop : MonoBehaviour, IStoreListener
{
    IStoreController storeController;
    public ConsumableItem cItem;
    public NonConsumableItem ncItem;
    public SubscriptionItem sItem;
    [SerializeField] private GameObject restoreButton;
    [SerializeField] private int coins = 0;
    [SerializeField] public TMP_Text coinsText;

    private void Awake()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            restoreButton.SetActive(false);
        }
    }

    private void Start()
    {
        coins = PlayerPrefs.GetInt("totalCoins");
        coinsText.text = coins.ToString();
        SetupBuilder();
    }

    void SetupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(cItem.Id, ProductType.Consumable);
        builder.AddProduct(ncItem.Id, ProductType.NonConsumable);
        builder.AddProduct(sItem.Id, ProductType.Subscription);
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        print("Success");
        storeController = controller;
    }

    public void OnPurchaseComplete(Product product)
    {
        // This method is not used in the code, you can remove it if not needed
    }

    public void OnPressedNonConsumable()
    {
        storeController.InitiatePurchase(ncItem.Id);
    }

    public void OnPressedConsumable()
    {
        storeController.InitiatePurchase(cItem.Id);
    }

    public void OnPressedSubscription()
    {
        storeController.InitiatePurchase(sItem.Id);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        //retrieve the purchased product
        var product = purchaseEvent.purchasedProduct;
        print("Purchase Complete" + product.definition.id);

        //Consumable item is pressed
        if (product.definition.id == cItem.Id)
        {
            AddCoins(50);
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        print($"Failed to purchase product {product.definition.id} because {reason}");
    }

    public void RemoveAds()
    {
        // Implement the logic to remove ads here
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt("totalCoins", coins);
        coinsText.text = coins.ToString();
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
{
    print($"Initialization failed: {error} - {message}");
}
}
*/