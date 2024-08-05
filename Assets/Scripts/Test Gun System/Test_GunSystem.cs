using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Pool;
//using Palmmedia.ReportGenerator.Core.Reporting.Builders;


public class Test_GunSystem : MonoBehaviourPunCallbacks
{
    [Header("Player Health")]
    public int MaxHealth = 100;
    public int CurrentHealth { get; private set; }
    public Image healthBarDisplay;

    [Header("Gun Switching")]
    public bool IsGunVisible = true;
    [SerializeField] private GameObject _gunHolder;
    private GameObject _selectedGunObject;
    public Test_GunItem[] Guns;
    private int _selectedIndex;
    public Test_GunItem SelectedGun { get; private set; }


    [Header("Gun Shooting")]
    [SerializeField] private Transform _raycastOrigin; // Probably the player's crosshair
    [SerializeField] private Transform _gunBarrel;
    public bool IsFiring { get; private set; }
    private float _accumulatedTime;
    private ObjectPool<GameObject> _projectilePool;
    [Header("Reloading")]
    [HideInInspector] public int CurrentAmmo;
    [HideInInspector] public int AmmoInReserve;

    private void Start()
    {
        CurrentHealth = MaxHealth;
        SelectedGun = Guns[_selectedIndex];
        CurrentAmmo = SelectedGun.ClipSize;
        
        if (IsGunVisible)
        {
            SwitchToGun(_selectedIndex);
        
            // Get location of barrel end 
            _gunBarrel = _selectedGunObject.transform.GetChild(0); // Must ensure first child is Barrel End 
            // not clever way to do this :( 
        }
        else
        {
            Debug.Assert(_gunBarrel != null, "Need to manually assign gun barrel");
        }


        #region Object pooling

        _projectilePool = new ObjectPool<GameObject>
        (
            // Create Projectile
            () =>
            {
                // Instantiate over network
                GameObject bullet = PhotonNetwork.Instantiate(nameof(SelectedGun.BulletPrefab), _gunBarrel.position, Quaternion.identity);
                // Set gravity to 0
                var bulletScript = bullet.GetComponent<ProjectileMotion>();
                Debug.Assert(bulletScript != null);
                bulletScript.Gravity = 0;
                // Set object pool
                bulletScript.ProjectilePool = _projectilePool;

                return bullet;
            },
            // Get from pool
            bullet => bullet.SetActive(true),
            // Return to pool
            bullet =>
            {
                bullet.SetActive(false);
                bullet.transform.position = _gunBarrel.position;
                bullet.transform.rotation = _gunBarrel.rotation;
            },
            // On exceed pool size
            bullet => PhotonNetwork.Destroy(bullet),
            // Check for redundant returns
            true,
            // Staring pool size
            7,
            // Max pool size
            30
        );

        #endregion
    }

    private void Update()
    {
        UpdateHealthBar();

        if(Input.GetKey(KeyCode.Space))
        {
            StartFiring();
            StopFiring();
        }
    }

    #region Gun Shooting

    public void StartFiring() // Can also be used to fire single bullets
    {
        print("Pew");
        IsFiring = true;
        _accumulatedTime = 0.0f;
        if (CurrentAmmo > 0)
            FireBullet();
    }

    public void UpdateFiring()
    {
        IsFiring = true;
        _accumulatedTime += Time.deltaTime; // sec / frame
        float fireRate = 1 / SelectedGun.BulletsPerSecond; // sec / bullet

        while (_accumulatedTime >= 0f && CurrentAmmo > 0)
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

        if (SelectedGun.IsHitscan) // Hitscan weapon
        {
            // Bullet trail
            GameObject bullet = Instantiate(SelectedGun.BulletPrefab, _gunBarrel.position, Quaternion.identity);
            Debug.Assert(bullet.TryGetComponent<TrailRenderer>(out var trail), "Bullet does not have trail renderer");
            trail.AddPosition(_gunBarrel.position); // Start point
            trail.transform.position = _hitInfo.point; // End point

            // Damage enemy
            if (_hitInfo.transform.TryGetComponent(out Test_GunSystem opponentSystem))
            {
                opponentSystem.gameObject.GetPhotonView().RPC(nameof(TakeDamage), RpcTarget.All, this);
            }
        }
        else // Projectile weapon
        {
            _projectilePool.Get();
        }
        // Update ammo
        CurrentAmmo--;
    }

    private RaycastHit _hitInfo;
    public void SetHitInfo(GameObject caller, RaycastHit hit)
    {
        // Security measures, idk if this is actually best practice
        // Only allow scripts on the same object to set
        if (caller == this.gameObject)
        {
            _hitInfo = hit;
        }
    }
    #endregion

    #region Reloading
    public void Reload()
    {
        int amountNeeded = SelectedGun.ClipSize - CurrentAmmo;
        if (amountNeeded >= AmmoInReserve)
        {
            CurrentAmmo += AmmoInReserve;
            AmmoInReserve = 0;
        }
        else
        {
            CurrentAmmo = SelectedGun.ClipSize;
            AmmoInReserve -= amountNeeded;
        }
    }

    #endregion

    #region Gun Switching

    public void SwitchToGun(int gunIndex)
    {
        Destroy(_selectedGunObject);
        _selectedIndex = gunIndex;
        SelectedGun = Guns[_selectedIndex];
        _selectedGunObject = PhotonNetwork.Instantiate
        (
            SelectedGun.GunPrefab.name,
            _gunHolder.transform.position,
            _gunHolder.transform.rotation
        );
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

    [PunRPC]
    public void TakeDamage(Test_GunSystem damager)
    {
        if (photonView.IsMine)
        {
            CurrentHealth -= damager.SelectedGun.Damage;
            print($"{this.name} got hit by {damager.name}!");

            // Death
            if (CurrentHealth <= 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    #endregion
}