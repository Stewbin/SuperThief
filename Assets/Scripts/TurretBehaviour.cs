using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class TurretBehaviour : EnemyBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _turretHead;
    [SerializeField] private Transform _shootOrigin;
    [SerializeField] private Test_GunSystem _gunSystem;
    public float RayLength = 5f;
    public float FOV = 60f;
    public float TurnSpeed = 0.01f;


    private Quaternion _initRotation;
    Quaternion targetRotation; // Not a member, I just needed to lift a local variable T^T
    void Start()
    {
        // Setup state
        CurrentState = State.Searching;

        // Setup rotating behavior
        _initRotation = _turretHead.rotation;
        targetRotation = _initRotation; // Initialization

        // Setup line renderer
        _lineRenderer.positionCount = 2;
    }

    private bool _isAttacking;
    public int AggroTime;
    private float _aggroTime;
    void Update()
    {
        #region Raycast 
        Ray ray = new(_shootOrigin.position, _shootOrigin.forward);
        Debug.DrawRay(ray.origin, 5 * ray.direction, Color.black);

        // Shoot raycast
        bool seenPlayer = false;
        if (Physics.Raycast(ray, out RaycastHit hit, RayLength, ~LayerMask.GetMask("Enemy")))
        {
            seenPlayer = hit.collider.CompareTag("Player");
        }

        // Set the positions of the LineRenderer
        _lineRenderer.SetPosition(0, ray.origin);
        _lineRenderer.SetPosition(1, ray.origin + ray.direction * RayLength);

        #endregion

        #region State machine


        if (CurrentState == State.Searching)
        {
            // Stop attacknig
            if (_isAttacking)
            {
                StopCoroutine(Attack(hit));
                _isAttacking = false;
            }

            // Rotate between two angles on Y
            targetRotation = Quaternion.Euler(0, FOV / 2, 0);

            if (Quaternion.Angle(_turretHead.rotation, targetRotation * _initRotation) < 0.1f)
            {
                targetRotation = Quaternion.Inverse(targetRotation);
            }

            // Destination ray
            // Debug.DrawRay(_turretHead.position, targetRotation * Vector3.forward * 10, Color.yellow);

            // Exit search state
            if (seenPlayer)
            {
                TargetPlayer = hit.transform;
                CurrentState = State.Hunting;
            }

        }
        else if (CurrentState == State.Hunting)
        {
            // Calculate direction to the player
            Vector3 directionToPlayer = TargetPlayer.position - _turretHead.position;
            // Ignore vertical rotation
            directionToPlayer.y = 0;
            directionToPlayer.Normalize(); // Normalize the direction to avoid scaling issues

            // Compute the new rotation
            targetRotation = Quaternion.FromToRotation(_shootOrigin.forward, directionToPlayer);



            // Start atttacking
            if (!_isAttacking)
            {
                StartCoroutine(Attack(hit));
                _isAttacking = true;
                print("Shooting at " + hit.transform.name);
            }

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
        #endregion

        #region Update rotation

        float step = TurnSpeed * Mathf.Rad2Deg * Time.deltaTime;
        _turretHead.rotation = Quaternion.RotateTowards(_turretHead.rotation, targetRotation * _initRotation, step);

        #endregion
    }



    public float ReloadTime = 5;
    public float SecPerBullet = 5;

    private IEnumerator Attack(RaycastHit hit)
    {
        _gunSystem.AmmoInReserve = int.MaxValue;

        while (true)
        {
            // If gun empty
            if (0 == _gunSystem.CurrentAmmo)
            {
                yield return new WaitForSeconds(ReloadTime);
                _gunSystem.Reload();
            }

            // Otherwise shoot
            _gunSystem.SetHitInfo(gameObject, hit);
            _gunSystem.StartFiring();
            _gunSystem.StopFiring();
            yield return new WaitForSeconds(SecPerBullet);
        }
    }
}
