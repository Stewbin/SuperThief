using UnityEngine;
using UnityEngine.VFX;


[CreateAssetMenu(fileName = "New Gun Item", menuName = "GunItem")]
public class Test_GunItem : ScriptableObject 
{
    [Header("Gun Stats")]
    public int ClipSize; 
    public int BulletsPerSecond;
    public float BulletSpeed = 100f;
    public int Damage;
    public bool IsHitscan = true;
    public FireType FiringType;

    [Header("Gun Model")]
    public GameObject GunPrefab;
    public Transform GunBarrel;
    
    [Header("Gun VFX")]
    public GameObject BulletImpact;
    public TrailRenderer BulletTrail;
    public GameObject BulletPrefab;
    public VisualEffect MuzzleFlash;
    
    [Header("Gun SFX")]
    public AudioClip FireSound;
    public AudioClip TriggerPullSound;
}

public enum FireType
{
    FullyAutomatic,
    SemiAutomatic,
    SingleShot
}
