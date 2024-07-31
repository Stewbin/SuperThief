using UnityEngine;
using Photon.Pun;
using System.Collections;

public class TurretBehaviour : EnemyBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _turretHead;
    [SerializeField] private Transform _shootOrigin;
    [SerializeField] private AdvancedGunSystem _advancedGunSystem;
    [SerializeField] private SimpleGunSystem _simpleGunSystem;
    public float RayLength = 5f;
    public float FOV = 60f;
    public float TurnSpeed = 0.01f;
    public int AggroTime;
    private float _aggroTime;
    private bool _isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        // Setup state
        CurrentState = State.Searching;

        // Setup rotating behavior
        _initRotation = _turretHead.rotation;
        step = TurnSpeed * Time.deltaTime;
        targetRotation = Quaternion.Euler(0, FOV / 2, 0);

        // Setup line renderer
        _lineRenderer.positionCount = 2;
    }

    private Quaternion _initRotation;
    private Quaternion targetRotation;
    private float step;
    void Update()
    {
        #region Raycast 
        Ray ray = new(_shootOrigin.position, Quaternion.Euler(0, -90, 0) * _shootOrigin.forward);
        // "Forward" is considered 'right' cuz of model T^T

        // Shoot raycast
        bool seenPlayer = false;
        if (Physics.Raycast(ray, out RaycastHit hit, RayLength, ~LayerMask.GetMask("Enemy")))
        {
            seenPlayer = hit.collider.CompareTag("Player");
        }

        // // Set the positions of the LineRenderer
        // _lineRenderer.SetPosition(0, ray.origin);
        // _lineRenderer.SetPosition(1, ray.origin + ray.direction * RayLength);

        #endregion

        if (CurrentState == State.Searching)
        {
            // Stop attacknig
            if (_isAttacking)
            {
                StopCoroutine(Attack(hit));
                _isAttacking = false;
            }

            // Change target rotation at end of rotation
            if (Quaternion.Angle(_turretHead.rotation, targetRotation) < 0.1f)
            {
                targetRotation = Quaternion.Inverse(targetRotation);
            }

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
            targetRotation = Quaternion.LookRotation(directionToPlayer, Quaternion.Inverse(_initRotation) * transform.up);


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



        #region Rotate turret head
        // Move to target rotation
        _turretHead.localRotation = Quaternion.RotateTowards(_turretHead.localRotation, targetRotation, step);

        #endregion

    }

    private void LateUpdate()
    {
        // print("Current roatation: " + _turretHead.rotation.eulerAngles);
        print("Target rotation: " + targetRotation.eulerAngles);
        print("State: " + CurrentState);
    }
    public float ReloadTime = 5;
    public float SecPerBullet = 5;

    private IEnumerator Attack(RaycastHit hit)
    {
        // Gun gun = _advancedGunSystem.allGuns[_advancedGunSystem.selectedGun];
        Gun gun = _simpleGunSystem.TheGun;
        gun.reservedAmmoCapacity = int.MaxValue;

        while (true)
        {
            // If gun empty
            if (0 == gun.currentAmmoInClip)
            {
                yield return new WaitForSeconds(ReloadTime);
                _simpleGunSystem.Reload();
            }

            // Otherwise shoot
            _simpleGunSystem.Shoot(hit);
            yield return new WaitForSeconds(SecPerBullet);
        }
    }
}
