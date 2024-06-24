using UnityEngine;

public class RotateObjectOnTouch : MonoBehaviour
{
    public float MobileRotationSpeed = 0.4f;
    public Camera cam;

    private bool isRotating = false;
    private Vector2 previousTouchPosition;
    private GameObject selectedObject;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    selectedObject = GetObjectUnderTouch(touch.position);
                    if (selectedObject == gameObject)
                    {
                        isRotating = true;
                        previousTouchPosition = touch.position;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isRotating)
                    {
                        Vector2 currentTouchPosition = touch.position;
                        float deltaX = currentTouchPosition.x - previousTouchPosition.x;
                        float deltaY = currentTouchPosition.y - previousTouchPosition.y;

                        transform.Rotate(cam.transform.up, -deltaX * MobileRotationSpeed, Space.World);
                        transform.Rotate(cam.transform.right, deltaY * MobileRotationSpeed, Space.World);

                        previousTouchPosition = currentTouchPosition;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isRotating = false;
                    selectedObject = null;
                    break;
            }
        }
    }

    private GameObject GetObjectUnderTouch(Vector2 touchPosition)
    {
        Ray ray = cam.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject;
        }

        return null;
    }
}