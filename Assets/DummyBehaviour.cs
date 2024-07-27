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
    private int _currentHealth;
    public float RespawnTime;
    private Coroutine moving;

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
    private void TakeDamage(string damager, int damageAmount, int actor)
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
    private bool hitWall = false;
    public IEnumerator MoveLeftAndRight()
    {
        Vector3 displacement = Range * transform.right;
        float time = Time.deltaTime * MoveSpeed;
        while (true)
        {
            Vector3 targetPosition = startPosition + displacement;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, time);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f || hitWall)
            {
                displacement *= -1;
            }

            Debug.Log("Moving!");
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        hitWall = other.gameObject.name.Equals("Walls");
    }

    public void StartMoving()
    {
        if (moving == null)
        {
            moving = StartCoroutine(MoveLeftAndRight());
        }
    }

    public void StopMoving()
    {
        if (moving != null)
        {
            StopCoroutine(moving);
            moving = null;
        }
    }
    #endregion
}
