using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBehaviour : MonoBehaviour
{
    public bool IsMoving;
    public float MoveSpeed;
    public float Range;
    private Vector3 startPosition;
    public static readonly List<DummyBehaviour> Dummies = new();
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;   
        Dummies.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator MoveLeftAndRight()
    {
        Vector3 direction = Range * transform.right;
        float time = Time.deltaTime * MoveSpeed;
        while (true)
        {
            Vector3 targetPosition = startPosition + direction;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, time);

            if (transform.position == targetPosition)
            {
                direction *= -1;
            }
            yield return null;
        }
    }
    
}
