using System;
using System.Collections; 
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class AdvancedGunSystem : MonoBehaviourPunCallbacks, IPointerDownHandler, IPointerUpHandler
{
    [Header("Main Camera for Ray Cast")]
    public new Camera camera;
    [Header("Gun VFX")]
    public GameObject bulletImpact;
    public float muzzleDisplayTime;
    public float muzzleCounter;
    public GameObject playerHitImpact;
    public GameObject BulletTrail;
    public float BulletSpeed = 100f;

    [Header("Heat Gun Settings")]
    public float maxHeat = 10f;
    public float coolRate = 5f;
    public float overheatCoolRate = 5f;
    public float heatCounter;
    public bool overHeated;

    [Header("Player Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public Image healthBarDisplay;

    

    [Header("Gun Variables")]
    public Gun[] allGuns;
    public int selectedGun;
    private bool _isShootButtonPressed;
    private bool _isReloadButtonPressed;

    public float adsSpeed = 5f; 
    
    public Transform adsOutPoint, adsInPoint;

    [Header("SFX")]

    public AudioSource gunShootSource; 

    [Header("Testing")]

    public string damagerText; 

    public bool isPlayerDead = false; 

    [Header("Hitmarker Implementation")]

    public GameObject hitMarker; 

    public AudioSource hitMarkerAudioSource; 
    public bool playHitMarker; 

    public TMP_Text damageIndicator; 

    [SerializeField] public int damageTest; 

 

    [Header("Damage Floating Text Implementation")]

    

    [SerializeField] public string  targetPlayerName; 
    [SerializeField] public PhotonView targetPhotonView;



    public static AdvancedGunSystem instance;

    public void Awake()
    {
        instance = this;
        PhotonNetwork.OfflineMode = true;
    }

    

   private void Start()
    {
        photonView.RPC("SetGun", RpcTarget.All, selectedGun);
        photonView.RPC("SwitchGunForMobile", RpcTarget.All);
        currentHealth = maxHealth;
        UpdateHealthBar();
        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.currentHealthDisplay.text = currentHealth.ToString();
        //Disable hit marker 

        hitMarker.SetActive(false);
        damageIndicator.gameObject.SetActive(false); 
        
        playHitMarker = false; 

        if (photonView.IsMine)
        {
            foreach (Gun gun in allGuns)
            {
                gun.currentAmmoInClip = gun.clipSize;
            }
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            UIController.instance.currentHealthDisplay.text = currentHealth.ToString();
            UIController.instance.magazineSize.text = allGuns[selectedGun].clipSize.ToString();
            UIController.instance.currentAmmo.text = allGuns[selectedGun].currentAmmoInClip.ToString();
            
            //test
          

            if (allGuns[selectedGun].muzzleFlash.activeInHierarchy)
            {
                muzzleCounter -= Time.deltaTime;
                if (muzzleCounter <= 0)
                {
                    allGuns[selectedGun].muzzleFlash.SetActive(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.R) || _isReloadButtonPressed)
            {
                Reload();
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0f || Input.GetKeyDown(KeyCode.Tab))
            {
                int prevSelectedGun = selectedGun;
                if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.Tab))
                {
                    selectedGun = (selectedGun + 1) % allGuns.Length;
                }
                else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
                {
                    selectedGun--;
                    if (selectedGun < 0)
                    {
                        selectedGun = allGuns.Length - 1;
                    }
                }

                if (prevSelectedGun != selectedGun)
                {
                    photonView.RPC("SetGun", RpcTarget.All, selectedGun);
                }
            }

            if (_isShootButtonPressed)
            {
                if (allGuns[selectedGun].isAutomatic)
                {
                    if (Time.time > allGuns[selectedGun].fireRate)
                    {
                        Shoot();
                        allGuns[selectedGun].fireRate = Time.time + allGuns[selectedGun].fireRate;
                    }
                }
                else
                {
                    if (_isShootButtonPressed)
{
    if (Time.time > allGuns[selectedGun].fireRate)
    {
        Shoot();
        allGuns[selectedGun].fireRate = Time.time + allGuns[selectedGun].fireRate;
    }
}
                }
            }

            if (!overHeated)
            {
                heatCounter -= coolRate * Time.deltaTime;
            }
            else
            {
                heatCounter -= overheatCoolRate * Time.deltaTime;
                if (heatCounter <= 0)
                {
                    overHeated = false;
                }
            }

            if (heatCounter < 0)
            {
                heatCounter = 0f;
            }

            for (int i = 0; i < allGuns.Length; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    if (selectedGun != i)
                    {
                        selectedGun = i;
                        photonView.RPC("SetGun", RpcTarget.All, selectedGun);
                    }
                    break;
                }
            }
        }
    }

    public void Shoot()
    {
       if (allGuns[selectedGun].currentAmmoInClip <= 0)
        {
            return;
        }

        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        ray.origin = camera.transform.position;


        photonView.RPC("ShootSFX", RpcTarget.All);

        //shoot sfx 
       // PlayerSoundManager.instance.PlayShootSFX_RPC(shootSFXIndex); 

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Bullet VFX
            Transform currentGun = allGuns[selectedGun].transform;
            photonView.RPC(nameof(ShootVFX), RpcTarget.All, currentGun.position, hit.point);
            
            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetPhotonView().IsMine)
            {
                PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);
                hit.collider.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, photonView.Owner.NickName, allGuns[selectedGun].shotDamage, PhotonNetwork.LocalPlayer.ActorNumber);

                //Show Hit Marker
                HitMarkerActive(); 
                Invoke("HitMarkerInActive", 0.1f);
                playHitMarker = true;
                PlayHitMarkerSoundFX(); 

                UIController.instance.damageTextAmount.text = allGuns[selectedGun].shotDamage.ToString(); 

                damageIndicator.text = allGuns[selectedGun].shotDamage.ToString();

        
                
                

                //get opponent photon view and name dd
                targetPhotonView = hit.collider.gameObject.GetPhotonView(); 
                targetPlayerName = targetPhotonView.Owner.NickName;
                
            }
            else
            {
                GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + hit.normal * 0.002f, Quaternion.LookRotation(hit.normal, Vector3.up));
                Destroy(bulletImpactObject, 5f);
                playHitMarker = false;
                
            }
        }
        

        allGuns[selectedGun].currentAmmoInClip--;
        heatCounter += allGuns[selectedGun].shotDamage;

        allGuns[selectedGun].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;
    }

    [PunRPC]
    private void TakeDamage(string damager, int damageAmount, int actor)
    {
        if (photonView.IsMine)
        {
            currentHealth -= damageAmount;

            //display damage anmount 
           
           
            print(damageAmount + "" + damageTest); 
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                PlayerSpawner.instance.Die(damager);
                MatchManager.instance.UpdateStatsSend(actor, 0, 1);
                //Elimination text     
                //UIController.instance.ShowKillMessage(damager, photonView.Owner.NickName);

            string victimName = photonView.Owner.NickName;
            // Send RPC to show elimination message on all clients
            PhotonView.Find(actor).RPC("ShowEliminationMessageRPC", RpcTarget.All, damager, victimName);

              

                
               
            // Display who the player just killed
            // This ensures only the shooter sees this message
            
                UIController.instance.debugMessage.text = "ELIMINATED: " + isPlayerDead;
            


               damagerText = photonView.Owner.NickName; 
               
                photonView.RPC("ShowEliminationMessage", RpcTarget.All, damager, photonView.Owner.NickName);

               //

                UIController.instance.healthSlider.value = currentHealth;
                photonView.RPC("UpdateHealthBarRPC", RpcTarget.All, currentHealth);

               
            }  
        } 
    }

    [PunRPC]
    private void SetGun(int gunToSwitchTo)
    {
        if (gunToSwitchTo < allGuns.Length)
        {
            selectedGun = gunToSwitchTo;
            SwitchGun();
        }
    }

    public void SwitchGun()
    {
        foreach (Gun gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }
        allGuns[selectedGun].gameObject.SetActive(true);

        if (photonView.IsMine)
        {
            UIController.instance.magazineSize.text = allGuns[selectedGun].clipSize.ToString();
            UIController.instance.currentAmmo.text = allGuns[selectedGun].currentAmmoInClip.ToString();
        }
    }

    public void Reload()
    {
        int amountNeeded = allGuns[selectedGun].clipSize - allGuns[selectedGun].currentAmmoInClip;
        if (amountNeeded >= allGuns[selectedGun].reservedAmmoCapacity)
        {
            allGuns[selectedGun].currentAmmoInClip += allGuns[selectedGun].reservedAmmoCapacity;
            allGuns[selectedGun].reservedAmmoCapacity = 0;
        }
        else
        {
            allGuns[selectedGun].currentAmmoInClip = allGuns[selectedGun].clipSize;
            allGuns[selectedGun].reservedAmmoCapacity -= amountNeeded;
        }
        UIController.instance.currentAmmo.text = allGuns[selectedGun].currentAmmoInClip.ToString();
        UIController.instance.magazineSize.text = allGuns[selectedGun].clipSize.ToString();
    }


    [PunRPC]
    public void SwitchGunForMobile()
    {
        selectedGun++;
        if (selectedGun >= allGuns.Length)
        {
            selectedGun = 0;
        }
        SwitchGun();
        
        //testing something
         

      
    }

    public void AimGun()
    {
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, allGuns[selectedGun].adsZoom, adsSpeed * Time.deltaTime); 
        //PlayerMove.instance.gunHolder.position = Vector3.Lerp(PlayerMove.instance.gunHolder.position, adsInPoint.position, adsSpeed * Time.deltaTime); 
         
    }

    public void CancelAimGun()
    {
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 60f, adsSpeed * Time.deltaTime);
        //PlayerMove.instance.gunHolder.position = Vector3.Lerp(PlayerMove.instance.gunHolder.position, adsOutPoint.position, adsSpeed * Time.deltaTime); 
 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       if (eventData.pointerCurrentRaycast.gameObject.CompareTag("ShootButton"))
    {
        _isShootButtonPressed = true;
    }
        else if (eventData.pointerCurrentRaycast.gameObject.CompareTag("ReloadButton"))
        {
            _isReloadButtonPressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       if (eventData.pointerCurrentRaycast.gameObject.CompareTag("ShootButton"))
        {
        _isShootButtonPressed = false;
        }
        else if (eventData.pointerCurrentRaycast.gameObject.CompareTag("ReloadButton"))
        {
            _isReloadButtonPressed = false;
        }
    }

    public void UpdateHealthBar()
    {
        if (healthBarDisplay != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            healthBarDisplay.fillAmount = fillAmount;
        }
    }

    [PunRPC]
    public void UpdateHealthBarRPC(int health)
    {
        currentHealth = health;
        UpdateHealthBar();
    }

    [PunRPC]
    public void Heal(int healAmount)
    {
        if (photonView.IsMine)
        {
            currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
            UpdateHealthBar();
           
            
        }
    }


     [PunRPC]
    public void AddAmmo(int ammoAmount)
    {
        
        if (photonView.IsMine)
        {
            int currentAmmo = allGuns[selectedGun].currentAmmoInClip;
            int maxAmmo = allGuns[selectedGun].clipSize;
            int ammoToAdd = Mathf.Min(ammoAmount, maxAmmo - currentAmmo);

            
            allGuns[selectedGun].currentAmmoInClip += ammoToAdd;
            UIController.instance.currentAmmo.text = allGuns[selectedGun].currentAmmoInClip.ToString();
            print("Successfully added ammo amount of " + ammoToAdd); 

        }
 
    }


    #region Show Elimination Message (Only For Killer)
    [PunRPC]
    private void ShowEliminationMessage(string killer, string victim)
    {
        if (photonView.IsMine && killer == photonView.Owner.NickName)
        {
            StartCoroutine(DisplayEliminationMessage(victim));
            print("Eliminated " + victim);
        }
    }

    private IEnumerator DisplayEliminationMessage(string victim)
    {
        UIController.instance.eliminationMessage.gameObject.SetActive(true);
        UIController.instance.eliminationMessage.text = "Eliminated " + victim;
        print("Eliminated " + victim);

        yield return new WaitForSeconds(5f);

        UIController.instance.eliminationMessage.gameObject.SetActive(false);
    }

    [PunRPC]
    private void ShowEliminationMessageRPC(string damager, string victim)
    {
        // Display elimination message on all clients
        UIController.instance.ShowEliminationMessage(damager, victim);
        
        // Check if the local player is the damager
        if (photonView.Owner.NickName == damager)
        {
            UIController.instance.ShowLocalEliminationMessage(victim);
        }
    }


    #endregion Show Elimination Message (Only For Killer)


    #region Show Hit Marker

    public void HitMarkerActive()
    {
    hitMarker.SetActive(true); 
    damageIndicator.gameObject.SetActive(true); 
    }
    public void HitMarkerInActive()
    {

    hitMarker.SetActive(false); 
    damageIndicator.gameObject.SetActive(false); 
    

    }

    public void PlayHitMarkerSoundFX(){

    if(playHitMarker == true)
    {
        hitMarkerAudioSource.Play(); 
    } 
    else 
    {
        hitMarkerAudioSource.Stop(); 
    }

    }

    [PunRPC]
    public void ShootSFX()
    {
        gunShootSource.Play(); 
    }


    #endregion Show Hit Marker

    #region Shoot VFX
    
    [PunRPC]
    public void ShootVFX(Vector3 spawnPoint, Vector3 hitDestination)
    {
        StartCoroutine(SpawnTrail(spawnPoint, hitDestination, BulletSpeed));
    }

    #region Bullet Trail
    private IEnumerator SpawnTrail(Vector3 spawnPoint, Vector3 hitDestination, float bulletSpeed)
    {
        GameObject trail = Instantiate(BulletTrail, spawnPoint, Quaternion.identity);
        Debug.Assert(trail != null);
        TrailRenderer renderer = trail.GetComponent<TrailRenderer>();
        float hitDistance = Vector3.Distance(spawnPoint, hitDestination);
        float remainingDistance = hitDistance;

        while(remainingDistance > 0)
        {
            trail.transform.position = Vector3.Lerp(spawnPoint, hitDestination, 1 - (remainingDistance / hitDistance));
            remainingDistance -= Time.deltaTime * bulletSpeed;
            yield return null;
        }
        Destroy(trail, renderer.time);
    }
    #endregion

    #endregion 
}
