using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera movement script for the mini-map, and menu screen
/// </summary>
public class CamController : MonoBehaviour
{
    public Transform target;
    public float damping;
    [SerializeField] private Mode _mode;
    [Header("Rotation Motion")]
    public float speed = 1f;

    void LateUpdate()
    {
        switch(_mode)
        {
            case Mode.Rotator:
                CircleAround();
                break;
            case Mode.Minimap:
                FollowTarget();
                break;
        }
    }

    void CircleAround()
    {
        // Aim camera at target
        Vector3 targetDirection = target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        // Circle around target
        float radius = Vector3.Magnitude(Vector3.ProjectOnPlane(targetDirection, Vector3.up)); // Project direction into xz plane, then take Magnitude
        float t = speed * Time.time;
        transform.position = radius * new Vector3(Mathf.Cos(t), 0, Mathf.Sin(t)) + // Actual circular motion
        target.position + // Translate center to target
        transform.position.y * Vector3.up; // Translate y-coord to cam level
        
    }

    void FollowTarget()
    {

    }
}


enum Mode 
{
    Rotator,
    Minimap
}