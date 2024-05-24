/*
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener
{

    [Header("Platform Ids")]
    [SerializeField] private string androidGameId;
    [SerializeField] private string iOSGameId;

    [Header("Test Mode (Deactivate For Production")]
    [SerializeField] private testMode = true;  

    [Header("Test Mode (Deactivate For Production")]
    [SerializeField] private string androidAddUnitId;
    [SerializeField] private string iOSAddUnitId;

    public static AdManager Instance; 

    private string gameId;
    private string adUnitId;

    private void Awake()
    {
      if(Instance != null && Instance!=this)
        {
            Destroy(gameObject)
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 

        }
    }

    private void InitializeAds()
    {
#if UNITY_IOS
        gameId = iOSGameId; 
        addUnitid = iOSAddUnitId; 

#elif UNITY_ANDROID
        gameId = androidGameId; 
        addUnitid = androidAddUnitId; 

#elif UNITY_EDITOR
        gameId = androidGameId; 
        addUnitid = androidAddUnitId; 
#endif 

        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameId, testMode, this);
        }

       

        public void OnInitialazationComplete()
        {
            print("Unity ads Initialization succesfull"); 
        }

        public void OnInitialazationFailed(UnityAdsInitializationError error, string message)
        {
            print($"Unity ads failed : {error.ToString()} - {message}");
        }

    }
}
*/

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


public class AdManager : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {

    }
}