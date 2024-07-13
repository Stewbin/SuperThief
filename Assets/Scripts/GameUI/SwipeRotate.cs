using UnityEngine;

public class SwipeRotate : MonoBehaviour
{
    private Touch touch;
    private Vector2 touchPosition;
    private Quaternion rotationY;
    [SerializeField] private float rotateSpeed = 0.1f;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                // Calculate rotation based on touch movement
                float rotationAmount = touch.deltaPosition.x * rotateSpeed;
                rotationY = Quaternion.Euler(0f, -rotationAmount, 0f);

                // Apply rotation to the object
                transform.rotation *= rotationY;
            }
        }
    }
}