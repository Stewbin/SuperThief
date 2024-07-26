using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBehaviour : MonoBehaviour
{
    public bool IsMoving;
    public float MoveSpeed;
    public float Range;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;   
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator MoveLeftAndRight()
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
