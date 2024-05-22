using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform Target;
    public float CameraHeight = 16f;
    public bool FollowYRotation = false;
    public bool FollowPosition = false;

    // Update is called once per frame
    void LateUpdate()
    {
        // Update position
        if (FollowPosition)
        {
            Vector3 NewPosition = Target.position;
            NewPosition.y = Target.position.y + CameraHeight;
            transform.position = NewPosition;
        }
        // Update rotation
        if (FollowYRotation)
        {
            transform.eulerAngles = new Vector3(0f, Target.eulerAngles.y, 0f);
        }
    }
}
