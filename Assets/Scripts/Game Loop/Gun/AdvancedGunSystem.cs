using System.ComponentModel;
using System.Timers;
using System;
using System.Threading;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class AdvancedGunSystem : MonoBehaviourPunCallbacks
{
    

[Header("Main Camera for Ray Cast")]
 public Camera camera; 
 public GameObject bulletImpact; 
 //public float timeBetweenShots = 0.1f; 
 private float shotCounter; 


[Header("Heat Gun Settings")]
public float maxHeat = 10f;
//public float heatPerShot = 1f; 
public float coolRate = 5f;
public float overheatCoolRate = 5f;

public float heatCounter; 
public bool overHeated; 

public Gun[] allGuns; 
private int selectedGun;

public Image imageButton;

public float muzzleDisplayTime; 
public float muzzleCounter; 


    void Start()
    {
        UIController.instance.weaponTempSlider.maxValue = maxHeat;

        SwitchGun(); 
       
    }


    void Update()
    {

if(photonView.IsMine){


       
        if (allGuns[selectedGun].muzzleFlash.activeInHierarchy){

        muzzleCounter -= Time.deltaTime;

        if(muzzleCounter <= 0){
        allGuns[selectedGun].muzzleFlash.SetActive(false);

        }
        
        }


        if(!overHeated){
        if(Input.GetMouseButtonDown(1)){
            Shoot(); 
        }

         if(Input.GetMouseButton(1) && allGuns[selectedGun].isAutomatic){
            shotCounter -= Time.deltaTime;

            if(shotCounter <= 0){
                Shoot(); 
            }
        }

        heatCounter -= coolRate * Time.deltaTime;

        }  else {

        heatCounter -= overheatCoolRate * Time.deltaTime;

        if(heatCounter <= 0) {
           
            overHeated = false; 
            UIController.instance.overheatedMessage.gameObject.SetActive(false);


        }
    }
    if (heatCounter < 0){

        heatCounter = 0f;
    }
       UIController.instance.weaponTempSlider.value = heatCounter; 
       

       if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f){
        selectedGun++; 

        if(selectedGun >= allGuns.Length){
            selectedGun = 0;
        }

        SwitchGun(); 

       } else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f){

        selectedGun--; 
         if(selectedGun <0){
            selectedGun = allGuns.Length;
        }

        SwitchGun(); 
       }

       for (int i = 0 ; i <allGuns.Length ; i++){

        if (Input.GetKeyDown((i + 1).ToString())){
            selectedGun = i; 
            SwitchGun(); 
        }
       }


        

}
       
    }

    public void Shoot(){

        Ray ray = camera.ViewportPointToRay(new Vector3(.5f, .5f, 0f)); 

        ray.origin = camera.transform.position;

        if(Physics.Raycast(ray, out RaycastHit hit )) {

            print("We just hit : " + hit.collider.gameObject.name);

            GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));

            Destroy(bulletImpactObject, 5f); 
        }

        shotCounter = allGuns[selectedGun].timeBetweenShots; 

        heatCounter += allGuns[selectedGun].heatPerShot;

        if (heatCounter >= maxHeat){
            heatCounter = maxHeat; 
            overHeated = true; 

            UIController.instance.overheatedMessage.gameObject.SetActive(true);
        }
        allGuns[selectedGun].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;

    }

    public void SwitchGun(){
        

        foreach(Gun gun in allGuns){
            gun.gameObject.SetActive(false); 
        }

        allGuns[selectedGun].gameObject.SetActive(true);
        //allGuns[selectedGun].gameObject.SetActive(false);
    }

  
    public void SwitchGunForMobile()
    {
        selectedGun++;
        if (selectedGun >= allGuns.Length)
        {
         selectedGun = 0;
         /// <summary>
         /// 
         /// </summary>
         /// <returns></returns>
        } 
        SwitchGun();

        print("Switching Gun"); 
    }

   
}
