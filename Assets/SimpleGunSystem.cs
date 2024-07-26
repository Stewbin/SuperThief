using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SimpleGunSystem : MonoBehaviourPunCallbacks
{
    [Header("Gun Logisitics")]
    public Gun TheGun;
    [SerializeField] private Transform _shootOrigin;
    private RaycastHit _hitInfo;
    [Header("SFX")]
    [SerializeField] private AudioSource _audioSource;
    [Header("Gun VFX")]
    public GameObject BulletImpact;
    public GameObject PlayerHitImpact;
    public GameObject BulletTrail;
    public float BulletSpeed = 100f;


    public void Shoot(RaycastHit hitInfo)
    {
        if (TheGun.currentAmmoInClip <= 0)
        {
            return;
        }

        // Shoot SFX
        photonView.RPC(nameof(ShootSFX), RpcTarget.All);

        // Shoot VFX
        StartCoroutine(SpawnTrail(_shootOrigin.position, hitInfo.point, BulletSpeed));


        if (hitInfo.collider.gameObject.CompareTag("Player") && !hitInfo.collider.gameObject.GetPhotonView().IsMine)
        {
            PhotonNetwork.Instantiate(PlayerHitImpact.name, hitInfo.point, Quaternion.identity);
            hitInfo.collider.gameObject.GetPhotonView().RPC(nameof(TakeDamage), RpcTarget.Others, photonView.Owner.NickName, TheGun.shotDamage, PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            GameObject bulletImpactObject = Instantiate(BulletImpact, hitInfo.point + hitInfo.normal * 0.002f, Quaternion.LookRotation(hitInfo.normal, Vector3.up));
            Destroy(bulletImpactObject, 5f);
        }


        TheGun.currentAmmoInClip--;
    }

    public void Reload()
    {
        int amountNeeded = TheGun.clipSize - TheGun.currentAmmoInClip;
        if (amountNeeded >= TheGun.reservedAmmoCapacity)
        {
            TheGun.currentAmmoInClip += TheGun.reservedAmmoCapacity;
            TheGun.reservedAmmoCapacity = 0;
        }
        else
        {
            TheGun.currentAmmoInClip = TheGun.clipSize;
            TheGun.reservedAmmoCapacity -= amountNeeded;
        }
    }

    [PunRPC]
    private void TakeDamage(string damager, int damageAmount, int actor)
    {
        // Empty??
    }


    #region VFX & SFX
    private void ShootSFX()
    {
        _audioSource.Play();
    }

    private IEnumerator SpawnTrail(Vector3 spawnPoint, Vector3 hitDestination, float bulletSpeed)
    {
        GameObject trail = PhotonNetwork.Instantiate(nameof(BulletTrail), spawnPoint, Quaternion.identity);
        Debug.Assert(trail != null);
        TrailRenderer renderer = trail.GetComponent<TrailRenderer>();
        Vector3 startPosition = _shootOrigin.position;
        float hitDistance = Vector3.Distance(startPosition, hitDestination);
        float remainingDistance = hitDistance;

        while (remainingDistance > 0)
        {
            trail.transform.position = Vector3.Lerp(spawnPoint, hitDestination, 1 - (remainingDistance / hitDistance));
            remainingDistance -= Time.deltaTime * bulletSpeed;
            yield return null;
        }
        Destroy(trail, renderer.time);
    }
    #endregion
}
