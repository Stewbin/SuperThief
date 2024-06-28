using System.Collections;
using System.Collections.Generic;
using PlayFab.GroupsModels;
using UnityEngine;

public class TurretBehaviour : EnemyBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform turretHead;
    [SerializeField] private AdvancedGunSystem advancedGunSystem;
    [SerializeField] private State state;
    public float RayLength = 5f;
    public Color StartColor = Color.white;
    public Color EndColor = Color.red;
    public float FOV = 60f;
    public float TurnSpeed = 0.01f;
    [HideInInspector] public Transform TargetPlayer;
    // Start is called before the first frame update
    void Start()
    {
        // Set the number of positions to 2 (start and end points of the line)
        lineRenderer.positionCount = 2;

        // Set the start and end colors
        lineRenderer.startColor = StartColor;
        lineRenderer.endColor = EndColor;

        // Set the start and end widths
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new(turretHead.position, turretHead.forward);
        Debug.DrawRay(ray.origin, RayLength * ray.direction, Color.red);
        // Set the positions of the LineRenderer
        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, ray.origin + ray.direction * RayLength);  

        if(state == State.Searching)
        {
            // Rotate head
            float targetRotation = Mathf.Sin(Time.time * TurnSpeed) * (FOV / 2);
            turretHead.transform.rotation = Quaternion.Euler(0, targetRotation, 0);

            // Fire Physics Raycasts
            if(Physics.Raycast(ray, out RaycastHit hit, RayLength, ~LayerMask.GetMask("Enemy")))
            {
                Debug.Log($"{hit.collider.name} hit.");
                if(hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Player found");
                    TargetPlayer = hit.transform;
                    state = State.Hunting;
                }
            }
        } 
        else if (state == State.Hunting)
        {
            // Look in direction of player
            Vector3 directionToPlayer = TargetPlayer.position - turretHead.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer.normalized, transform.up);
            turretHead.eulerAngles = new Vector3(0, lookRotation.eulerAngles.y, 0);
            
            // Shoot
            advancedGunSystem.Shoot();
            Debug.Log("Pew.");
        } 
    }

    public override void SwitchToHuntingState(Transform NewPlayer)
    {
        TargetPlayer = NewPlayer;
        state = State.Hunting;
    }
}

// Just gonna use this make-shift State enum for now
enum State 
{
    Searching,
    Hunting,
}
