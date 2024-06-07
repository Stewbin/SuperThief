using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro; 

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
public GameObject playerHitImpact;  

[Header("Player Health ")]

public int maxHealth = 100; 
private int currentHealth; 






//Networking UI Text testing



    void Start()
    {
        //UIController.instance.weaponTempSlider.maxValue = maxHeat;

      //SwitchGun(); 

      photonView.RPC("SetGun", RpcTarget.All, selectedGun);
       
      currentHealth = maxHealth;

      if(photonView.IsMine){
      UIController.instance.healthSlider.maxValue = maxHealth;
      UIController.instance.healthSlider.value = currentHealth;
      UIController.instance.currentHealthDisplay.text = currentHealth.ToString(); 
      }

    
  
    }


    void Update()
    {

if(photonView.IsMine){

        //display hp in the bottom 
        UIController.instance.currentHealthDisplay.text = currentHealth.ToString(); 
       
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
           // UIController.instance.overheatedMessage.gameObject.SetActive(false);


        }
    }
    if (heatCounter < 0){

        heatCounter = 0f;
    }
      // UIController.instance.weaponTempSlider.value = heatCounter; 
       

       if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f){
        selectedGun++; 

        if(selectedGun >= allGuns.Length){
            selectedGun = 0;
        }

        //SwitchGun(); 
        photonView.RPC("SetGun", RpcTarget.All, selectedGun);

       } else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f){

        selectedGun--; 
         if(selectedGun <0){
            selectedGun = allGuns.Length -1;
        }

        //SwitchGun(); 
          photonView.RPC("SetGun", RpcTarget.All, selectedGun);
       }

       for (int i = 0 ; i <allGuns.Length ; i++){

        if (Input.GetKeyDown((i + 1).ToString())){
            selectedGun = i; 
            //SwitchdGun(); 
            photonView.RPC("SetGun", RpcTarget.All, selectedGun);
        }
       }


        

}
       
    }

    public void Shoot(){

        Ray ray = camera.ViewportPointToRay(new Vector3(.5f, .5f, 0f)); 

        ray.origin = camera.transform.position;

        if(Physics.Raycast(ray, out RaycastHit hit )) {

            print("We just hit : " + hit.collider.gameObject.name);

if(hit.collider.gameObject.tag =="Player"){
    print("We just hit : " + hit.collider.gameObject.GetPhotonView().Owner.NickName);
PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);

hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, allGuns[selectedGun].shotDamage, PhotonNetwork.LocalPlayer.ActorNumber); 

} else {


            GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));

            Destroy(bulletImpactObject, 5f); 
}
        }

        shotCounter = allGuns[selectedGun].timeBetweenShots; 

        heatCounter += allGuns[selectedGun].heatPerShot;

        if (heatCounter >= maxHeat){
            heatCounter = maxHeat; 
            overHeated = true; 

           // UIController.instance.overheatedMessage.gameObject.SetActive(true);
        }
        allGuns[selectedGun].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;

    }

    [PunRPC] 
    public void DealDamage(string damager, int damageAmount, int actor ){
         TakeDamage(damager, damageAmount, actor); 
    }

    public void TakeDamage(string damager,int damageAmount, int actor) {

if(photonView.IsMine){

        print(photonView.Owner.NickName + "has been hit by" + damager); 

        //gameObject.SetActive(false);

        currentHealth -= damageAmount;
        
        if(currentHealth <= 0){
            currentHealth = 0; 
            PlayerSpawner.instance.Die(damager);
            
            MatchManager.instance.UpdateStatsSend(actor, 0, 1);
        }
        UIController.instance.healthSlider.value = currentHealth;

      
}
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

    public void notShooting(){
        print("Not shooting "); 
    }

    [PunRPC]
    public void SetGun(int gunToSwitchTo){

        if (gunToSwitchTo < allGuns.Length){

            selectedGun = gunToSwitchTo;
            SwitchGunForMobile(); 
        }
    }
   
}
