using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Pool;
//using Palmmedia.ReportGenerator.Core.Reporting.Builders;

namespace Prototype_Shooting
{
    public class Test_GunSystem : MonoBehaviourPunCallbacks
    {
        [Header("Player Health")]
        public int MaxHealth = 100;
        public int CurrentHealth { get; private set; }

        [Header("Gun Switching")]
        public bool IsGunVisible = true;
        [SerializeField] private GameObject _gunHolder;
        private GameObject _selectedGunObject;
        public Test_GunItem[] Guns;
        private int _selectedIndex;
        public Test_GunItem SelectedGun { get; private set; }
        [Header("Gun SFX")]
        [SerializeField] private AudioSource _audioSource;

        [Header("Gun Shooting")]
        [SerializeField] private Transform _raycastOrigin; // Probably the player's crosshair
        [SerializeField] private Transform _gunBarrel;
        public bool IsFiring { get; private set; }
        private float _accumulatedTime;
        [Header("Object Pooling")]
        public int DefaultPoolSize = 7;
        public int MaxPoolSize { get; private set; } = 7;
        private ObjectPool<GameObject> _bulletPool;
        [Header("Reloading")]
        public int CurrentAmmo;
        public int AmmoInReserve;

        private void Start()
        {
            PhotonNetwork.OfflineMode = true;
            CurrentHealth = MaxHealth;
            SelectedGun = Guns[_selectedIndex];
            Debug.Assert(SelectedGun != null, "Something wrong with gun");
            Debug.Log("Hitscan? " + SelectedGun.IsHitscan);
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

            if (photonView.IsMine)
            {
                #region Object pooling

                if (SelectedGun.IsHitscan) // Hitscan pool
                {
                    _bulletPool = new ObjectPool<GameObject>
                    (
                        // Create Tracer
                        () =>
                        {
                            // Instantiate over network
                            GameObject bullet = PhotonNetwork.Instantiate(SelectedGun.BulletPrefab.name, _gunBarrel.position, Quaternion.identity);
                            Debug.Assert(bullet != null, "Bullet was not instantiated over network");

                            // Get TrailRenderer out
                            Debug.Assert
                            (
                                bullet.TryGetComponent<TrailRenderer>(out var trail),
                                "No TrailRenderer found on bullets"
                            );
                            trail.autodestruct = false;

                            return bullet;
                        },
                        // Get from pool
                        bullet => bullet.SetActive(true),
                        // Return to pool
                        bullet =>
                        {
                            bullet.SetActive(false);
                            bullet.transform.SetPositionAndRotation(_gunBarrel.position, _gunBarrel.rotation);
                        },
                        // On exceed pool size
                        bullet => PhotonNetwork.Destroy(bullet),
                        // Check for redundant returns
                        true,
                        // Staring pool size
                        DefaultPoolSize,
                        // Max pool size
                        MaxPoolSize
                    );
                }
                else // Projectile pool
                {
                    _bulletPool = new ObjectPool<GameObject>
                    (
                        // Create Projectile
                        () =>
                        {
                            // Instantiate over network
                            GameObject bullet = PhotonNetwork.Instantiate(SelectedGun.BulletPrefab.name, _gunBarrel.position, Quaternion.identity);
                            Debug.Assert(bullet != null, "Bullet was not instantiated over network");

                            // Get script out
                            Debug.Assert
                            (
                                bullet.TryGetComponent<ProjectileMotion>(out var bulletScript),
                                "No ProjectileMotion script found on bullets"
                            );
                            // Return to object pool on collision
                            bulletScript.FinalCollisionEvent += (handler, otherCollider) => _bulletPool.Release(handler.gameObject);
                            // Set owner of projectile to deal damaage
                            bulletScript.FinalCollisionEvent += (handler, otherCollider) =>
                            {
                                if (otherCollider.TryGetComponent<Test_GunSystem>(out Test_GunSystem gunSystem))
                                {
                                    otherCollider.gameObject.GetPhotonView().RPC(nameof(gunSystem.TakeDamage), RpcTarget.Others, this);
                                }
                            };

                            return bullet;
                        },
                        // Get from pool
                        bullet => bullet.SetActive(true),
                        // Return to pool
                        bullet =>
                        {
                            bullet.SetActive(false);
                            bullet.transform.SetPositionAndRotation(_gunBarrel.position, _gunBarrel.rotation);
                        },
                        // On exceed pool size
                        bullet => PhotonNetwork.Destroy(bullet),
                        // Check for redundant returns
                        true,
                        // Staring pool size
                        DefaultPoolSize,
                        // Max pool size
                        MaxPoolSize
                    );
                }
                #endregion
            }
        }


        private void Update()
        {


            if (Input.GetKey(KeyCode.Space))
            {
                print("Pew");
                StartFiring();
                StopFiring();
            }
        }

        #region Gun Shooting

        public void StartFiring() // Can also be used to fire single bullets
        {
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
        }

        public void FireBullet()
        {
            // Muzzle flash
            // SelectedGun.MuzzleFlash.Play();

            // Sound
            _audioSource.Play();

            if (SelectedGun.IsHitscan) // Hitscan weapon
            {
                // Bullet trail
                GameObject bullet = _bulletPool.Get();
                TrailRenderer trail = bullet.GetComponent<TrailRenderer>();

                trail.AddPosition(_gunBarrel.position); // Start point
                trail.transform.position = _hitInfo.point; // End point


                // Damage damage-able entity
                if (_hitInfo.collider.TryGetComponent(out Test_GunSystem opponentSystem))
                {
                    opponentSystem.gameObject.GetPhotonView().RPC(nameof(TakeDamage), RpcTarget.All, this);
                }
                // AdvancedGunSystem compatability
                else if (_hitInfo.collider.CompareTag("Player"))
                {
                    _hitInfo.collider.gameObject.GetPhotonView().RPC(nameof(TakeDamage), RpcTarget.All, "Turret", SelectedGun.Damage, PhotonNetwork.LocalPlayer.ActorNumber);
                }

                // Clear, then return bullet to pool 
                trail.Clear();
                _bulletPool.Release(bullet);
            }
            else // Projectile weapon
            {
                _bulletPool.Get();
            }
            // Update ammo
            CurrentAmmo--;
        }

        private RaycastHit _hitInfo;
        public void SetHitInfo(GameObject caller, RaycastHit hit)
        {
            // Security measures, idk if this is actually best practice
            // Only allow scripts on the same object to set
            if (caller != null && caller == this.gameObject)
            {
                _hitInfo = hit;
            }
        }
        #endregion

        #region Reloading
        public void Reload()
        {
            Debug.Assert(SelectedGun != null, "Selected Gun is null");
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
}