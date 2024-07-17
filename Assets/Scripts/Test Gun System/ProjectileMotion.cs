using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{
    [SerializeField] private Test_GunItem gunItem;
    [SerializeField] private AudioClip[] sounds;
    public float Gravity = 9.8f;
    public Vector3 Velocity;

    private void Update()
    {
        
    } 
}