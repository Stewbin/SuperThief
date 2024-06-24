using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera movement script for the mini-map, and menu screen
/// </summary>
public class CamEncircler : MonoBehaviour
{
    public Transform target;
    public float speed = 1f;
    public Vector3 newPosition;

    void LateUpdate()
    {
        // Aim camera at target
        Vector3 targetDirection = target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        // Circle around target
        float radius = Vector3.Magnitude(Vector3.ProjectOnPlane(targetDirection, Vector3.up)); // Project direction into xz plane, then take Magnitude
        float t = speed * Time.time;
        newPosition = radius * new Vector3(Mathf.Cos(t), 0, Mathf.Sin(t)) + // Circular motion
        new Vector3(target.position.x, transform.position.y, target.position.z);  // Translate center to target and y-coord to cam level

        transform.position = newPosition;
        
    }
}