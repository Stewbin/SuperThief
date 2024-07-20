using UnityEngine;
using Photon.Pun;
using System.Collections;

public class TurretBehaviour : EnemyBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _turretHead;
    [SerializeField] private Transform _shootOrigin;
    [SerializeField] private AdvancedGunSystem _advancedGunSystem;
    public float RayLength = 5f;
    public float FOV = 60f;
    public float TurnSpeed = 0.01f;
    public int AggroTime;
    [SerializeField] private float _aggroTime;
    private Quaternion _initRotation;

    // Start is called before the first frame update
    void Start()
    {
        CurrentState = State.Searching;
        
        _initRotation = _turretHead.rotation;

        // Set the number of positions to 2 (start and end points of the line)
        _lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        #region Common code
            Ray ray = new(_shootOrigin.position, _shootOrigin.right); 
            // "Forward" is considered 'right' cuz of model T^T
            Debug.DrawRay(ray.origin, RayLength * ray.direction, Color.red);
            Debug.Log(CurrentState);
    
            // Shoot raycast
            bool seenPlayer = false;
            if (Physics.Raycast(ray, out RaycastHit hit, RayLength, ~LayerMask.GetMask("Enemy")))
            {
                seenPlayer = hit.collider.CompareTag("Player");
            }
    
            // Set the positions of the LineRenderer
            _lineRenderer.SetPosition(0, _shootOrigin.position);
            _lineRenderer.SetPosition(1, _shootOrigin.position + _shootOrigin.right * RayLength);
    
        #endregion

        if (CurrentState == State.Searching)
        {
            // Stop attacknig
            StopCoroutine(Attack());

            // Rotate head
            float targetY = Mathf.Sin(Time.time * TurnSpeed) * (FOV / 2);
            Quaternion targetRotation = Quaternion.Euler(0, targetY, 0) * _initRotation;
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
            // Calculate direction to the player
            Vector3 directionToPlayer = TargetPlayer.position - _turretHead.position;
            // Ignore vertical rotation
            directionToPlayer.y = 0;
            directionToPlayer.Normalize(); // Normalize the direction to avoid scaling issues
            
            // Compute the new rotation
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer, transform.up);

            // Apply the initial rotation
            _turretHead.rotation = lookRotation;
            _turretHead.Rotate(_initRotation.eulerAngles);

            // Start atttacking
            StartCoroutine(Attack());

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
        /* 
            Note: Enemies can't die until gun system is reworked
        */
        // if(_advancedGunSystem.currentHealth <= 0)
        // {
        //     PhotonNetwork.Destroy(gameObject);
        // }
    }

    public float ReloadTime;

    private IEnumerator Attack()
    {
        Gun gun = _advancedGunSystem.allGuns[_advancedGunSystem.selectedGun];
        gun.clipSize = int.MaxValue;

        while (true)
        {
            if(0 <= gun.currentAmmoInClip)
            {
                yield return new WaitForSeconds(ReloadTime);
                _advancedGunSystem.Reload();
            }

            _advancedGunSystem.Shoot();
        }
    }
}
