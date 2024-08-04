using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using Photon.Pun;

public class ProjectileMotion : MonoBehaviourPunCallbacks
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip onDestroySound;
    [SerializeField] private List<AudioClip> regularSounds;
    public float SoundInterval = 2f;

    [Header("Motion")]
    public float Gravity = 9.8f;
    [SerializeField] private Test_GunItem _gunItem;
    private float _damage => _gunItem.Damage;
    private float _speed => _gunItem.BulletSpeed;
    private Vector3 y_Vel;
    private Vector3 xz_Vel;
    [HideInInspector] public ObjectPool<GameObject> ProjectilePool;

    public override void OnEnable()
    {
        base.OnEnable();
        // Bullet sounds
        StartCoroutine(PlaySounds());

        Vector3 velocity = _speed * transform.forward;
        // Velocity components
        y_Vel = velocity.y * Vector3.up;
        xz_Vel = velocity - y_Vel;
    }

    private Vector3 targetPosition;
    private void FixedUpdate()
    {
        targetPosition = transform.position;

        // Check bullet is facing forward
        Assert.IsTrue(Vector3.Dot(xz_Vel, transform.forward) >= 0);
        // Apply forward motion
        targetPosition += Time.fixedDeltaTime * xz_Vel;

        // Incorporate gravity
        y_Vel.y += -0.5f * Time.fixedDeltaTime * Gravity;

        // Apply vertical motion
        targetPosition.y += Time.fixedDeltaTime * y_Vel.y;

        transform.position = targetPosition;
    }

    private void OnTriggerEnter(Collider victim)
    {
        print("Touched something, gunna die now!");

        if (victim.TryGetComponent<Test_GunSystem>(out Test_GunSystem gunSystem))
        {
            victim.gameObject.GetPhotonView().RPC(nameof(gunSystem.TakeDamage), RpcTarget.Others, this);
        }

        //  Add back bullet to object pool instead of destroying
        ProjectilePool.Release(gameObject);
    }

    private IEnumerator PlaySounds()
    {
        while (true)
        {
            regularSounds.ForEach(sound => audioSource.PlayOneShot(sound));
            yield return new WaitForSeconds(SoundInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, y_Vel);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, xz_Vel);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, targetPosition);
    }
}