using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AnalyticsManager : MonoBehaviour
{
    public bool IsInitialized { get; private set; } = false;
    [SerializeField] private GameObject consentUI;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;

    private void Awake()
    {
        if (acceptButton != null)
            acceptButton.onClick.AddListener(ConsentGiven);
        if (declineButton != null)
            declineButton.onClick.AddListener(ConsentDeclined);
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();
        AskForConsent();
    }

    void AskForConsent()
    {
        // Show the player a UI element that asks for consent
        if (consentUI != null)
            consentUI.SetActive(true);
    }

    void ConsentGiven()
    {
        if (consentUI != null)
            consentUI.SetActive(false);

        AnalyticsService.Instance.StartDataCollection();
        IsInitialized = true;
        Debug.Log("User granted consent. Unity Analytics initialized successfully.");
    }

    void ConsentDeclined()
    {
        if (consentUI != null)
            consentUI.SetActive(false);

        Debug.Log("User declined consent. Analytics will not be collected.");
    }

    public void TrackCustomEvent(string eventName, Dictionary<string, object> eventParams)
    {
        if (IsInitialized)
        {
            AnalyticsService.Instance.CustomData(eventName, eventParams);
            Debug.Log($"Tracked event: {eventName}");
        }
        else
        {
            Debug.LogWarning("Analytics not initialized. Event not tracked.");
        }
    }

    public void TrackLevelStart(int levelIndex)
    {
        if (IsInitialized)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "levelIndex", levelIndex }
            };
            AnalyticsService.Instance.CustomData("levelStart", parameters);
            Debug.Log($"Tracked level start: {levelIndex}");
        }
        else
        {
            Debug.LogWarning("Analytics not initialized. Level start not tracked.");
        }
    }
}