using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class DummyBehaviour : MonoBehaviour
{
    public TrainingRoomManager Manager;
    public float MoveSpeed;
    public float Range = 1f;
    private Vector3 startPosition;
    public int MaxHealth;
    [SerializeField] private int _currentHealth;
    public float RespawnTime;
    private Coroutine _moving = null;
    public AnimationCurve FallMotion;

    // Start is called before the first frame update
    private void Start()
    {
        Manager.DummiesInRoom.Add(this);
        startPosition = transform.position;
    }

    private void OnEnable()
    {
        _currentHealth = MaxHealth;
        StopAllCoroutines();

    }

    #region Health
    private IEnumerator FallOver()
    {
        Manager.IncrementVanquishCounter();
        float time = 0f;
        while (transform.localEulerAngles.x < 90)
        {
            time += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(90, 0, 0), 2 * FallMotion.Evaluate(time));
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void TakeDamage(int damageAmount)
    {
        _currentHealth -= damageAmount;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            StartCoroutine(FallOver());
        }
    }

    #endregion

    #region Movement
    private bool _touchedWall = false;
    public bool IsRight = false;
    public IEnumerator MoveLeftAndRight()
    {
        Vector3 displacement = Range * transform.right;
        while (true)
        {
            Vector3 targetPosition = startPosition + displacement;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * MoveSpeed);

            bool approximatelyEqual = Vector3.Distance(transform.position, targetPosition) < 0.1f;
            if (approximatelyEqual || _touchedWall)
            {
                displacement *= -1;
                _touchedWall = false;
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _touchedWall = other.gameObject.layer == LayerMask.NameToLayer("Ground");
    }

    public void StartMoving()
    {
        if (_moving == null)
        {
            _moving = StartCoroutine(MoveLeftAndRight());
        }
    }

    public void StopMoving()
    {
        if (_moving != null)
        {
            StopCoroutine(_moving);
            _moving = null;
        }
    }
    #endregion
}
