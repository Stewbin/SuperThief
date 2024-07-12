using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro; 

public class ShopManager : MonoBehaviour, IStoreListener
{
    [SerializeField]
    private UIProduct UIProductPrefab;
    [SerializeField]
    private HorizontalLayoutGroup ContentPanel;
    [SerializeField]
    private GameObject LoadingOverlay;
    [SerializeField]
    private bool UseFakeStore = false;

    [SerializeField]
    public TMP_Text HeistDiamondsValuesText;
    private Action OnPurchaseCompleted;
    private IStoreController StoreController;
    private IExtensionProvider ExtensionProvider;

    public const string DIAMONDS_KEY = "Diamonds";
    
    public const string COINS_KEY = "Coins";

    [SerializeField]
    private int diamondsRewardForAd = 50;

    [SerializeField]
    private Button watchAdButton;



    private async void Awake()
    {
        //shows player how many diamonds they already have
        HeistDiamondsValuesText.text = PlayerPrefs.GetInt(DIAMONDS_KEY).ToString();
        InitializationOptions options = new InitializationOptions()
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            .SetEnvironmentName("test");
#else
            .SetEnvironmentName("production");
#endif
        await UnityServices.InitializeAsync(options);
        ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
        operation.completed += HandleIAPCatalogLoaded;
    }

    private void HandleIAPCatalogLoaded(AsyncOperation Operation)
    {
        ResourceRequest request = Operation as ResourceRequest;

        Debug.Log($"Loaded Asset: {request.asset}");
        ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
        Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");

        if (UseFakeStore) // Use bool in editor to control fake store behavior.ee
        {
            StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser; // Comment out this line if you are building the game for publishing.
            StandardPurchasingModule.Instance().useFakeStoreAlways = true; // Comment out this line if you are building the game for publishing.
        }

#if UNITY_ANDROID
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.GooglePlay)
        );
#elif UNITY_IOS
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.AppleAppStore)
        );
#else
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.NotSpecified)
        );
#endif
        foreach (ProductCatalogItem item in catalog.allProducts)
        {
            builder.AddProduct(item.id, item.type);
        }

        Debug.Log($"Initializing Unity IAP with {builder.products.Count} products");
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        StoreController = controller;
        ExtensionProvider = extensions;
        Debug.Log($"Successfully Initialized Unity IAP. Store Controller has {StoreController.products.all.Length} products");
        StoreIconProvider.Initialize(StoreController.products);
        StoreIconProvider.OnLoadComplete += HandleAllIconsLoaded;
    }

    private void HandleAllIconsLoaded()
    {
        StartCoroutine(CreateUI());
    }

    private IEnumerator CreateUI()
    {
        List<Product> sortedProducts = StoreController.products.all
            .TakeWhile(item => !item.definition.id.Contains("sale"))
            .OrderBy(item => item.metadata.localizedPrice)
            .ToList();

        foreach (Product product in sortedProducts)
        {
            UIProduct uiProduct = Instantiate(UIProductPrefab);
            uiProduct.OnPurchase += HandlePurchase;
            uiProduct.Setup(product);
            uiProduct.transform.SetParent(ContentPanel.transform, false);
            yield return null;
        }

        HorizontalLayoutGroup group = ContentPanel.GetComponent<HorizontalLayoutGroup>();
        float spacing = group.spacing;
        float horizontalPadding = group.padding.left + group.padding.right;
        float itemSize = ContentPanel.transform
            .GetChild(0)
            .GetComponent<RectTransform>()
            .sizeDelta.x;

        RectTransform contentPanelRectTransform = ContentPanel.GetComponent<RectTransform>();
        contentPanelRectTransform.sizeDelta = new(
            horizontalPadding + (spacing + itemSize) * sortedProducts.Count,
            contentPanelRectTransform.sizeDelta.y
        );
    }

    private void HandlePurchase(Product Product, Action OnPurchaseCompleted)
    {
        LoadingOverlay.SetActive(true);
        this.OnPurchaseCompleted = OnPurchaseCompleted;
        StoreController.InitiatePurchase(Product);
    }

    public void RestorePurchase() // Use a button to restore purchase only in iOS device.
    {
#if UNITY_IOS
        ExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(OnRestore);
#endif
    }

    private void OnRestore(bool success)
{
    if (success)
    {
        print("Transactions restored successfully");
        // Handle successful restoration (e.g., update UI, refresh available items)
    }
    else
    {
        print("Transaction restoration failed");
        // Handle failed restoration (e.g., show error message to user)
    }
}

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        print($"Error initializing IAP because of {error}." +
            $"\r\nShow a message to the player depending on the error.");
    }

     public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        print($"Error initializing IAP because of {error}. Message: {message}" +
            $"\r\nShow a message to the player depending on the error.");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Failed to purchase {product.definition.id} because {failureReason}");
        OnPurchaseCompleted?.Invoke();
        OnPurchaseCompleted = null;
        LoadingOverlay.SetActive(false);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        print($"Successfully purchased {purchaseEvent.purchasedProduct.definition.id}");
    
    // Determine the amount of diamonds to add based on the product ID
    int diamondsToAdd = 0;
    switch (purchaseEvent.purchasedProduct.definition.id)
    {
        case "com.superthief.diamonds.starter":
            diamondsToAdd = 100; 
            break;
        case "com.superthief.diamonds.basic":
            diamondsToAdd = 500; 
            break;
        case "com.superthief.diamonds.epic":
            diamondsToAdd = 1000; 
            break;
        default:
            print($"Unknown product ID: {purchaseEvent.purchasedProduct.definition.id}");
            break;
    }

    if (diamondsToAdd > 0)
    {
        AddVirtualCurrency(diamondsToAdd);
    }

    OnPurchaseCompleted?.Invoke();
    OnPurchaseCompleted = null;
    LoadingOverlay.SetActive(false);

    return PurchaseProcessingResult.Complete;
    }


    private void AddVirtualCurrency(int amount)
{

    int currentDiamonds = PlayerPrefs.GetInt(DIAMONDS_KEY, 0);
    int newDiamonds = currentDiamonds + amount;
    PlayerPrefs.SetInt(DIAMONDS_KEY, newDiamonds);
    PlayerPrefs.Save();

    // Update UI
    HeistDiamondsValuesText.text = PlayerPrefs.GetInt(DIAMONDS_KEY).ToString(); 

    //
    var request = new AddUserVirtualCurrencyRequest
    {
        VirtualCurrency = "HD", // Assuming "HD" is your diamond currency code
        Amount = amount
    };

    PlayFabClientAPI.AddUserVirtualCurrency(request, OnAddVirtualCurrencySuccess, OnAddVirtualCurrencyError);
}

private void OnAddVirtualCurrencySuccess(ModifyUserVirtualCurrencyResult result)
{
    print($"Successfully added {result.BalanceChange} diamonds. New balance: {result.Balance}");

    PlayerPrefs.SetInt(DIAMONDS_KEY, result.Balance);
    PlayerPrefs.Save();
    // Update the UI to reflect the new balance
    HeistDiamondsValuesText.text = result.Balance.ToString();
}

private void OnAddVirtualCurrencyError(PlayFabError error)
{
    print($"Failed to add virtual currency: {error.ErrorMessage}");
}

public void OnGetUserInventorySuccess(GetUserInventoryResult result)
{
    // ... (existing code for coins)

    // Heist Diamonds
    if (result.VirtualCurrency.ContainsKey("HD"))
    {
        int diamonds = result.VirtualCurrency["HD"];
        HeistDiamondsValuesText.text = diamonds.ToString();
        print("You currently have " + diamonds + " diamonds");
        
        // Sync PlayerPrefs with PlayFab
        PlayerPrefs.SetInt(DIAMONDS_KEY, diamonds);
        PlayerPrefs.Save();
    }
    else
    {
        print("HD currency not found in the inventory");
    }
}

public void OnWatchAdButtonClicked()
    {
        Debug.Log("Watch Ad button clicked");
        AdManager.Instance.ShowRewardedAd(OnRewardedAdCompleted);
    }

    private void OnRewardedAdCompleted()
    {
        Debug.Log("Rewarded ad completed. Awarding diamonds.");
        AddVirtualCurrency(diamondsRewardForAd);
    }
}