using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyAdvancedGunSystem : AdvancedGunSystem
{
    public GameObject GunObject; 
    private Gun _gun;

    void Awake()
    {
        _gun = GunObject.GetComponent<Gun>();
    }

    public new void Shoot()
    {
        Ray ray = new(); 
        ray.direction = GunObject.transform.forward;
        ray.origin = GunObject.transform.position;

        if(Physics.Raycast(ray, out RaycastHit hit )) {

            print("We just hit : " + hit.collider.gameObject.name);

            if(hit.collider.gameObject.CompareTag("Player"))
            {
                print("We just hit : " + hit.collider.gameObject.GetPhotonView().Owner.NickName);
                PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);

                hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, _gun.shotDamage, PhotonNetwork.LocalPlayer.ActorNumber); 
            } 
            else 
            {
                GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
                Destroy(bulletImpactObject, 5f); 
            }
        }

        // shotCounter = _gun.timeBetweenShots; 

        heatCounter += _gun.heatPerShot;

        if (heatCounter >= maxHeat)
        {
            heatCounter = maxHeat; 
            overHeated = true; 
           // UIController.instance.overheatedMessage.gameObject.SetActive(true);
        }
        
        _gun.muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;
    }
}
