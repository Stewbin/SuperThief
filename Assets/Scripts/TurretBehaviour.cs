using System.Collections;
using System.Collections.Generic;
using PlayFab.GroupsModels;
using Unity.VisualScripting;
using UnityEngine;

public class TurretBehaviour : EnemyBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _turretHead;
    [SerializeField] private Transform _shootOrigin;
    [SerializeField] private AdvancedGunSystem _advancedGunSystem;
    public float RayLength = 5f;
    public Color StartColor = Color.white;
    public Color EndColor = Color.red;
    public float FOV = 60f;
    public float TurnSpeed = 0.01f;
    public int AggroTime;
    private float _aggroTime;

    // Start is called before the first frame update
    void Start()
    {
        CurrentState = State.Searching;
        Debug.Log("Start rotation: " + _turretHead.rotation);

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
        Ray ray = new(_shootOrigin.position, _turretHead.forward);
        Debug.DrawRay(ray.origin, RayLength * ray.direction, Color.red);
        Debug.Log(CurrentState);

        // Shoot raycast
        bool seenPlayer = false;
        if (Physics.Raycast(ray, out RaycastHit hit, RayLength, ~LayerMask.GetMask("Enemy")))
            seenPlayer = hit.collider.CompareTag("Player");

        // Set the positions of the LineRenderer
        _lineRenderer.SetPosition(0, ray.origin);
        _lineRenderer.SetPosition(1, ray.origin + ray.direction * RayLength);

        if (CurrentState == State.Searching)
        {
            // Rotate head
            float targetY = Mathf.Sin(Time.time * TurnSpeed) * (FOV / 2);
            var targetRotation = Quaternion.Euler(90f, targetY, 0); // The 90 is cuz of the model T^T
            _turretHead.rotation = targetRotation;


            // Exit search state
            if (seenPlayer)
            {
                Debug.Log("Player found");
                TargetPlayer = hit.transform;
                CurrentState = State.Hunting;
            }

        }
        else if (CurrentState == State.Hunting)
        {
            // Look in direction of player
            Vector3 directionToPlayer = TargetPlayer.position - _turretHead.position;
            Quaternion targetRotation = Quaternion.FromToRotation(_turretHead.forward, directionToPlayer);
            // Ignore x and z rotations
            targetRotation.x = 0; targetRotation.z = 0;

            _turretHead.rotation *= targetRotation;

            // Shoot
            _advancedGunSystem.Shoot();
            Debug.Log("Pew.");

            // Exit hunting state
            if (!seenPlayer)
            {
                _aggroTime -= Time.deltaTime;
            }
            if (_aggroTime <= 0)
            {
                CurrentState = State.Searching;
                _aggroTime = AggroTime;
            }
        }
    }
}
