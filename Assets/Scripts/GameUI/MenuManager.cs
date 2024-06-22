using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class MenuManager : MonoBehaviour
{
    public static MenuManager MC; 

    public GameObject shopPanel; 
    public GameObject [] buttonLocks; 
    public Button [] unlockedButtons; 

    private void OnEnable()
    {
        MC = this;
    }

     private void Start()
    {
       SetUpStore();
    }

    public void SetUpStore()
    {
        for(int i = 0; i < PersistentData.PD.allSkins.Length; i++){

            buttonLocks[i].SetActive(!PersistentData.PD.allSkins[i]);
            unlockedButtons[i].interactable = PersistentData.PD.allSkins[i];

        }
    }

    public void UnlockSkin(int index)
    {
        PersistentData.PD.allSkins[index] = true; 
        //save data to the cloud 
        PlayFabManager.PFC.SetUserData(PersistentData.PD.SkinsDataToString());
        SetUpStore();
    }
 
       public void OpenShop()
    {
        shopPanel.SetActive(true);
    }

    public void SetMySkin( int skinSelected)
    {
        PersistentData.PD.mySkin = skinSelected; 
    }





}
