using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Collections.Specialized;

public class GunController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Gun Settings")]
    public float fireRate = 0.1f;
    [SerializeField] public int clipSize = 30;
    [SerializeField] public int reservedAmmoCapacity = 270;

    [SerializeField]  public TextMeshProUGUI currentAmmoInClipText;
    [SerializeField]  public TextMeshProUGUI currentAmmoText;

    // Variables that change throughout the core game loop
    private bool _canShoot;
    private int _currentAmmoInClip;
    private int _ammoInReserve;

    private bool _isShootButtonPressed;
    private bool _isReloadButtonPressed;
    private bool _isAimButtonPressed; 

    //Creating a muzzleFlash For our Gun 
    public Image muzzleFlashImage;
    public Sprite [] flashes;

    //Arms position for aiming
    public Vector3 normalLocalPosition;
    public Vector3 aimingLocalPosition;

    public float aimSmoothing = 10f; 


    void Start()
    {
        _currentAmmoInClip = clipSize;
        _ammoInReserve = reservedAmmoCapacity;
        _canShoot = true;


      
    }

    void Update()
    {

        DetermineAim(); 
        // Check for shoot button press
        if (_isShootButtonPressed && _canShoot && _currentAmmoInClip > 0)
        {
            Shoot();
        }

        // Check for reload button press
        if (_isReloadButtonPressed)
        {
            Reload();
        }
        //Display UI Info about ammo in clip and reserve; 
        currentAmmoInClipText.text = _currentAmmoInClip.ToString();
        currentAmmoText.text = _ammoInReserve.ToString();

    }

    public void Shoot()
    {
        _canShoot = false;

        if (_currentAmmoInClip == 0)
        {
            _canShoot = false;
            return; 
        } else
        {
            _currentAmmoInClip--;
        }
       
        

        StartCoroutine(ShootCooldown());
       
    }

    private IEnumerator ShootCooldown()
    {
        StartCoroutine(MuzzleFlash()); 
        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
    }

    IEnumerator MuzzleFlash()
    {
        muzzleFlashImage.sprite = flashes[Random.Range(0, flashes.Length)];
        muzzleFlashImage.color = Color.white;

        yield return new WaitForSeconds(0.05f);
        muzzleFlashImage.sprite = null;
        muzzleFlashImage.color = new Color(0,0,0,0);


    }

    public void Reload()
    {
        int amountNeeded = clipSize - _currentAmmoInClip;
        if (amountNeeded >= _ammoInReserve)
        {
            _currentAmmoInClip += _ammoInReserve;
            _ammoInReserve = 0;
        }
        else
        {
            _currentAmmoInClip = clipSize;
            _ammoInReserve -= amountNeeded;
        }
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

    public void DetermineAim()
    {
        Vector3 target = normalLocalPosition; 
    }
}