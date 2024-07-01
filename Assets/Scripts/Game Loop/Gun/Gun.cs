using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public static Gun instance;

    private void Awake()
    {
        instance = this;
    }

    public bool isAutomatic;
    public float fireRate = 0.1f;
    [SerializeField] public int clipSize;
    [SerializeField] public int reservedAmmoCapacity;
    public GameObject muzzleFlash;
    public int shotDamage;
    public int currentAmmoInClip;
    public float adsZoom ;
    public void Start()
    {
        currentAmmoInClip = clipSize;
        //test
    }
}