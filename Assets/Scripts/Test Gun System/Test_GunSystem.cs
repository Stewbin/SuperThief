using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Unity.VisualScripting;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;


public class Test_GunSystem : MonoBehaviourPunCallbacks
{
    [Header("Player Health")]
    public int MaxHealth = 100;
    public int CurrentHealth {get; private set;}
    public Image healthBarDisplay;

    [Header("Gun Switching")]
    [SerializeField] private GameObject _gunHolder;
    private GameObject _selectedGunObject;
    public Test_GunItem[] Guns;
    private int _selectedIndex;
    public Test_GunItem SelectedGun {get; private set;}
    
    
    [Header("Gun Shooting")]
    [SerializeField] private Transform _raycastOrigin; // Probably the player's crosshair
    private Transform _gunBarrel;
    private RaycastHit _hitInfo;
    public bool IsFiring;
    private float _accumulatedTime;
    public int CurrentAmmo;
    
    private void Start()
    {
        CurrentHealth = MaxHealth;
        SelectedGun = Guns[_selectedIndex];
        CurrentAmmo = SelectedGun.ClipSize;
        _gunBarrel = SelectedGun.GunBarrel;
        SwitchToGun(_selectedIndex);
    }

    private void Update()
    {
        UpdateHealthBar();

        // Always be raycasting from the player's crosshair
        Physics.Raycast(_raycastOrigin.position, transform.forward, out _hitInfo);

        // Get location of barrel end 
        _gunBarrel = _selectedGunObject.transform.GetChild(0); // Must ensure first child is Barrel End 
        // not clever way to do this :( 
    }
    
    #region Gun Shooting

    public void StartFiring() // Can also be used to fire single bullets
    {
        print("Pew");
        IsFiring = true;
        _accumulatedTime = 0.0f;
        if(CurrentAmmo > 0)
            FireBullet();
    }

    public void UpdateFiring()
    {
        IsFiring = true;
        _accumulatedTime += Time.deltaTime; // sec / frame
        float fireRate = 1 / SelectedGun.BulletsPerSecond; // sec / bullet

        while(_accumulatedTime >= 0f && CurrentAmmo > 0)
        {
            _accumulatedTime -= fireRate;
            FireBullet();
        }
    }

    public void StopFiring()
    {
        IsFiring = false;
        print("Stop pewing");
    }

    private void FireBullet()
    {
        // Muzzle flash
        // SelectedGun.MuzzleFlash.Play();
        
        if(SelectedGun.IsHitscan) // Hitscan weapon
        {
            // Bullet trail
            TrailRenderer trail = Instantiate(SelectedGun.BulletTrail, _gunBarrel.position, Quaternion.identity);
            trail.AddPosition(_gunBarrel.position); // Start point
            trail.transform.position = _hitInfo.point; // End point
        }
        else // Projectile weapon
        {
            Instantiate(SelectedGun.BulletPrefab, _gunBarrel.transform);
        }
        // Update ammo
        CurrentAmmo--;

        // Damage enemy
        if(_hitInfo.transform.TryGetComponent(out Test_GunSystem opponentSystem))
        {
            opponentSystem.TakeDamage(this);
        }
    }
    
    #endregion

    #region Gun Switching

    public void SwitchToGun(int gunIndex)
    {
        Destroy(_selectedGunObject);
        _selectedIndex = gunIndex;
        SelectedGun = Guns[_selectedIndex];
        _selectedGunObject = Instantiate(SelectedGun.GunPrefab, _gunHolder.transform);
        // Gun Prefabs will probably have to be constructed such that their transform origins are
        // at their handles
    }

    #endregion

    #region Health System

    private void UpdateHealthBar() 
    {
        if (healthBarDisplay != null)
        {
            float fillAmount = (float)CurrentHealth / MaxHealth;
            healthBarDisplay.fillAmount = fillAmount;
        }
    }

    public void TakeDamage(Test_GunSystem damager) 
    {
        CurrentHealth -= damager.SelectedGun.Damage; 
        print($"{this.name} got hit by {damager.name}!");
    }

    #endregion
}