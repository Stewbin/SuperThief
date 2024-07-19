using UnityEngine;
using System.Collections;

public class ProjectileMotion : MonoBehaviour
{
    [SerializeField] private Test_GunItem gunItem;
    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private Collider collider;
    
    public float Gravity = 9.8f;
    private Vector3 y_Velocity; 
    private Vector3 xz_Velocity;
    
    

    private void Start()
    {
        // Proper directions
        y_Velocity = Vector3.up * transform.forward.y;
        xz_Velocity = transform.forward - y_Velocity;
        // Angle
        float theta = Vector3.Angle(xz_Velocity, transform.forward);

        // Proper magniudes
        y_Velocity *= gunItem.BulletSpeed * Mathf.Sin(theta);
        xz_Velocity *= gunItem.BulletSpeed * Mathf.Cos(theta);
    }

    private void Update()
    {
        Vector3 targetPosition = transform.position;
        
        // Forward motion
        targetPosition += xz_Velocity * Time.deltaTime;
        
        // Up & down motion
        // Velocity
        targetPosition += Time.deltaTime * y_Velocity; 
        // Gravity
        targetPosition -= Gravity * (Time.deltaTime * Time.deltaTime) * Vector3.up;

        transform.position = targetPosition; 
    }
    
    private void OnCollisionEnter()
    {
        Destroy(gameObject);
        // TODO: Add back bullet to pool instead of destroying
    }

    // private IEnumerator PlaySounds()
    // {
    //     // TODO: Implement playing sounds while the projectile flies
    // }
}