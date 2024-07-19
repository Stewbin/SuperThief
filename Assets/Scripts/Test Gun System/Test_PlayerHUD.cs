using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Collections.Specialized;

public class Test_PlayerHUD : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Gun System")]
    [SerializeField] private Test_GunSystem GunSystem;
    
    [Header("Gun Settings")]
    [SerializeField] public int ReservedAmmoCapacity = 270;

    // [SerializeField]  public TextMeshProUGUI currentAmmoInClipText;
    // [SerializeField]  public TextMeshProUGUI currentAmmoText;

    // Variables that change throughout the core game loop
    private int _currentAmmoInClip;
    private int _ammoInReserve;

    // Boolz
    [SerializeField] private bool _shootDownPress;
    [SerializeField] private bool _shootUpPress; 
    // True if pressed last frame ^^^
    [SerializeField] private bool _isReloadButtonPressed;
    

    void Start()
    {
        _currentAmmoInClip = GunSystem.CurrentAmmo;
        _ammoInReserve = ReservedAmmoCapacity;

    }

    void Update()
    {
        // I'm sorrrry this is so big T^T
        #region Shooting 
        
        switch(GunSystem.SelectedGun.FiringType)
        {
            case FireType.FullyAutomatic:
            {
                if(_shootDownPress)
                    GunSystem.StartFiring();

                if(GunSystem.IsFiring)
                    GunSystem.UpdateFiring();

                if(_shootUpPress)
                    GunSystem.StopFiring();
                break;
            }
            case FireType.SemiAutomatic:
            {
                if(_shootDownPress)
                    GunSystem.StartFiring();

                if(GunSystem.IsFiring)
                {
                    GunSystem.UpdateFiring();
                    GunSystem.StopFiring();
                }
                break;
            }
            case FireType.SingleShot:
            {
                if(_shootDownPress)
                {
                    GunSystem.StartFiring();
                    GunSystem.StopFiring();
                }
                break;
            }
            
        }

        _shootDownPress = false;
        _shootUpPress = false;;
        #endregion

        // Check for reload button press
        if (_isReloadButtonPressed)
        {
            Reload();
        }
        //Display UI Info about ammo in clip and reserve; 
    //     currentAmmoInClipText.text = _currentAmmoInClip.ToString();
    //     currentAmmoText.text = _ammoInReserve.ToString();
    }

    public void Reload()
    {
        print("rElOaDiNg");

        int amountNeeded = GunSystem.SelectedGun.ClipSize - _currentAmmoInClip;
        if (amountNeeded >= _ammoInReserve)
        {
            _currentAmmoInClip += _ammoInReserve;
            _ammoInReserve = 0;
        }
        else
        {
            _currentAmmoInClip = GunSystem.SelectedGun.ClipSize;
            _ammoInReserve -= amountNeeded;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Shoot button
        if(eventData.pointerCurrentRaycast.gameObject.CompareTag("ShootButton"))
        {
            _shootDownPress = true;
            _shootUpPress = false;
        }

        if (eventData.pointerCurrentRaycast.gameObject.CompareTag("ReloadButton"))
        {
            _isReloadButtonPressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Shoot button
        if(eventData.pointerCurrentRaycast.gameObject.CompareTag("ShootButton"))
        {
            _shootDownPress = false;
            _shootUpPress = true;
        }
        

        if (eventData.pointerCurrentRaycast.gameObject.CompareTag("ReloadButton"))
        {
            _isReloadButtonPressed = false;
        }
    }
}