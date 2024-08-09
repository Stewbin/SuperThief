using UnityEngine;
using System.Collections;


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
    private Quaternion _leftRotation;
    private Quaternion _rightRotation;
    Quaternion _targetRotation;
    void Start()
    {
        // Setup state
        CurrentState = State.Searching;

        // Setup rotating behavior
        _initRotation = _turretHead.rotation;
        _rightRotation = Quaternion.Euler(0, FOV / 2, 0) * _initRotation;
        _leftRotation = Quaternion.Euler(0, -FOV / 2, 0) * _initRotation;

        // Setup line renderer
        _lineRenderer.positionCount = 2;
    }

    [SerializeField] private Coroutine _isAttacking = null;
    private bool _isRight = true;
    public int AggroTime;
    private float _aggroTime;
    private RaycastHit _hitInfo;
    void Update()
    {
        #region Raycast 
        Ray ray = new(_shootOrigin.position, _shootOrigin.forward);

        // Shoot raycast
        bool seenPlayer = false;
        if (Physics.Raycast(ray, out _hitInfo, RayLength, ~LayerMask.GetMask("Enemy")))
        {
            seenPlayer = _hitInfo.collider.CompareTag("Player");
        }

        // Set the positions of the LineRenderer
        _lineRenderer.SetPosition(0, ray.origin);
        _lineRenderer.SetPosition(1, ray.origin + ray.direction * RayLength);

        #endregion
        Vector3 directionToPlayer = Vector3.zero;
        #region State machine
        if (CurrentState == State.Searching)
        {
            // Stop attacknig
            if (_isAttacking != null)
            {
                StopCoroutine(_isAttacking);
                _isAttacking = null;
            }

            // Rotate between two angles on Y
            _targetRotation = _isRight ? _rightRotation : _leftRotation;

            if (Quaternion.Angle(_turretHead.rotation, _targetRotation) < 0.1f)
            {
                _isRight = !_isRight;
            }

            // Exit search state
            if (seenPlayer)
            {
                TargetPlayer = _hitInfo.transform;
                CurrentState = State.Hunting;
            }

        }
        else if (CurrentState == State.Hunting)
        {
            directionToPlayer = TargetPlayer.position - _shootOrigin.position; // Calculate direction to the player
            directionToPlayer.y = 0; // Ignore vertical rotation
            directionToPlayer.Normalize(); // Normalize the direction to avoid scaling issues

            // Compute the new rotation
            _targetRotation = Quaternion.FromToRotation(ray.direction, directionToPlayer) * _turretHead.rotation;

            // Start atttacking
            if (_isAttacking == null)
            {
                _isAttacking = StartCoroutine(Attack());
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
        _turretHead.rotation = Quaternion.RotateTowards(_turretHead.rotation, _targetRotation, step);



        // Forward ray
        Debug.DrawRay(_turretHead.position, ray.direction, Color.cyan);
        #endregion
    }



    public float ReloadTime = 5;
    public float SecPerBullet = 5;
    private IEnumerator Attack()
    {
        _gunSystem.AmmoInReserve = int.MaxValue;
        print("Attacking has commenced >:o");

        while (true)
        {
            // If gun empty
            if (0 == _gunSystem.CurrentAmmo)
            {
                yield return new WaitForSeconds(ReloadTime);
                _gunSystem.Reload();
            }

            // Otherwise shoot
            _gunSystem.SetHitInfo(gameObject, _hitInfo);
            _gunSystem.StartFiring();
            _gunSystem.StopFiring();
            print("Pew pew pew");
            yield return new WaitForSeconds(SecPerBullet);
        }
    }
}
