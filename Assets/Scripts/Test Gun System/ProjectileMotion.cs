using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class ProjectileMotion : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private List<AudioClip> sounds;
    [SerializeField] private AudioSource audioSource;
    public float SoundInterval = 2f;

    [SerializeField] private Test_GunItem gunItem;
    public float Gravity = 9.8f;
    private float _bulletSpeed => gunItem.BulletSpeed;
    private Vector3 y_Vel;
    private Vector3 xz_Vel;


    private void Start()
    {
        // Bullet sounds
        StartCoroutine(PlaySounds());      
        
        Vector3 velocity = _bulletSpeed * transform.forward;  
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

    private void OnCollisionEnter()
    {
        print("Touched something, gunna die now!");
        Destroy(gameObject);
        // TODO: Add back bullet to object pool instead of destroying
    }

    private IEnumerator PlaySounds()
    {
        while (true)
        {
            sounds.ForEach(sound => audioSource.PlayOneShot(sound));
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