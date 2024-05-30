using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public Transform viewPoint;
    public float mouseSensitivity = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;

    public float moveSpeed = 5f, runSpeed = 8f;
    private float activeMoveSpeed;
    private Vector3 moveDir, movement;

    public CharacterController charCon;

    private Camera cam;

    public float jumpForce = 12f, gravityMod = 2.5f;

    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundLayers;

    public GameObject bulletImpact;
    //public float timeBetweenShots = .1f;
    private float shotCounter;
    public float muzzleDisplayTime;
    private float muzzleCounter;

    public float maxHeat = 10f, /* heatPerShot = 1f, */ coolRate = 4f, overheatCoolRate = 5f;
    private float heatCounter;
    private bool overHeated;

    public Gun[] allGuns;
    private int selectedGun;

    public GameObject playerHitImpact;

    public int maxHealth = 100;
    private int currentHealth;



    public float adsSpeed = 5f;


  

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;

        UIController.instance.weaponTempSlider.maxValue = maxHeat;

        //SwitchGun();
       // photonView.RPC("SetGun", RpcTarget.All, selectedGun);

       /// currentHealth = maxHealth;

        //Transform newTrans = SpawnManager.instance.GetSpawnPoint();
        //transform.position = newTrans.position;
        //transform.rotation = newTrans.rotation;

     
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {

            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

            verticalRotStore += mouseInput.y;
            verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

            if (invertLook)
            {
                viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
            }
            else
            {
                viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
            }

            moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

          
            float yVel = movement.y;
            movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;
            movement.y = yVel;

            if (charCon.isGrounded)
            {
                movement.y = 0f;
            }

            isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, groundLayers);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                movement.y = jumpForce;
            }

            movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;

            charCon.Move(movement * Time.deltaTime);

            if (allGuns[selectedGun].muzzleFlash.activeInHierarchy)
            {
                muzzleCounter -= Time.deltaTime;

                if (muzzleCounter <= 0)
                {
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

        Ray ray = GetComponent<Camera>().ViewportPointToRay(new Vector3(.5f, .5f, 0f)); 

        ray.origin = GetComponent<Camera>().transform.position;

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
