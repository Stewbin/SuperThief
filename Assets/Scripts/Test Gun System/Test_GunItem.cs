using UnityEngine;
using UnityEngine.VFX;


[CreateAssetMenu(fileName = "New Gun Item", menuName = "GunItem")]
public class Test_GunItem : ScriptableObject {
    [Header("Gun Stats")]
    public int ClipSize; 
    public int BulletsPerSecond;
    public float BulletSpeed = 100f;
    public int Damage;
    public bool IsHitscan = true;
    public FireType FiringType;

    [Header("Gun VFX")]
    public readonly GameObject GunPrefab;
    public readonly GameObject BulletImpact;
    public readonly TrailRenderer BulletTrail;
    public readonly GameObject BulletObject;
    public readonly VisualEffect MuzzleFlash;
    
    [Header("Gun SFX")]
    public readonly AudioClip FireSound;
    public readonly AudioClip TriggerPullSound;
}

public enum FireType
{
    FullyAutomatic,
    SemiAutomatic,
    SingleShot
}
