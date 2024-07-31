using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class DummyBehaviour : MonoBehaviourPunCallbacks
{
    public static readonly List<DummyBehaviour> Dummies = new();
    public float MoveSpeed;
    public float Range = 1f;
    private Vector3 startPosition;
    public int MaxHealth;
    [SerializeField] private int _currentHealth;
    public float RespawnTime;
    private Coroutine _moving;

    // Start is called before the first frame update
    private void Start()
    {
        startPosition = transform.position;
        Dummies.Add(this);

        _currentHealth = MaxHealth;
    }

    private void OnDestroy()
    {
        Dummies.Remove(this);
    }

    #region Health
    [PunRPC]
    public void TakeDamage(int damageAmount)
    {
        print("Ouch");
        _currentHealth -= damageAmount;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(RespawnTime);
        gameObject.SetActive(true);
    }

    #endregion

    #region Movement
    private bool _hitWall = false;
    public bool IsRight = false;
    public IEnumerator MoveLeftAndRight(bool isRight)
    {
        int direction = isRight ? 1 : -1;
        Vector3 displacement = Range * direction * transform.right;
        float time = Time.deltaTime * MoveSpeed;
        while (true)
        {
            Vector3 targetPosition = startPosition + displacement;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, time);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                displacement *= -1;
                print("Changing");
            }
            print("Stepping");
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _hitWall = other.gameObject.layer == LayerMask.NameToLayer("Ground");

        // Reverse direction
        StopMoving();
        IsRight = !IsRight;
        Debug.Assert(_moving == null);
        StartMoving(IsRight);

        print("Hit wall?: " + _hitWall);
    }

    public void StartMoving(bool isRight)
    {
        print("started");
        if (_moving == null)
        {
            _moving = StartCoroutine(MoveLeftAndRight(isRight));
        }
    }

    public void StopMoving()
    {
        print("stopped");
        if (_moving != null)
        {
            StopCoroutine(_moving);
            _moving = null;
        }
    }
    #endregion
}
