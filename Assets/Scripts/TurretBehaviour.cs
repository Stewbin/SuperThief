using System.Collections;
using System.Collections.Generic;
using PlayFab.GroupsModels;
using UnityEngine;

public class TurretBehaviour : EnemyBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _turretHead;
    [SerializeField] private AdvancedGunSystem _advancedGunSystem;
    [SerializeField] private State _state;
    public float RayLength = 5f;
    public Color StartColor = Color.white;
    public Color EndColor = Color.red;
    public float FOV = 60f;
    public float TurnSpeed = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        // Set the number of positions to 2 (start and end points of the line)
        _lineRenderer.positionCount = 2;

        // Set the start and end colors
        _lineRenderer.startColor = StartColor;
        _lineRenderer.endColor = EndColor;

        // Set the start and end widths
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new(_turretHead.position, _turretHead.forward);
        Debug.DrawRay(ray.origin, RayLength * ray.direction, Color.red);
        // Set the positions of the LineRenderer
        _lineRenderer.SetPosition(0, ray.origin);
        _lineRenderer.SetPosition(1, ray.origin + ray.direction * RayLength);  

        if(_state == State.Searching)
        {
            // Rotate head
            float targetRotation = Mathf.Sin(Time.time * TurnSpeed) * (FOV / 2);
            _turretHead.transform.rotation = Quaternion.Euler(0, targetRotation, 0);

            // Fire Physics Raycasts
            if(Physics.Raycast(ray, out RaycastHit hit, RayLength, ~LayerMask.GetMask("Enemy")))
            {
                Debug.Log($"{hit.collider.name} hit.");
                if(hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Player found");
                    TargetPlayer = hit.transform;
                    _state = State.Hunting;
                }
            }
        } 
        else if (_state == State.Hunting)
        {
            // Look in direction of player
            Vector3 directionToPlayer = TargetPlayer.position - _turretHead.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer.normalized, transform.up);
            _turretHead.eulerAngles = new Vector3(0, lookRotation.eulerAngles.y, 0);
            
            // Shoot
            _advancedGunSystem.Shoot();
            Debug.Log("Pew.");
        } 
    }

    public override void SwitchToHuntingState(Transform NewPlayer)
    {
        TargetPlayer = NewPlayer;
        _state = State.Hunting;
    }
}
