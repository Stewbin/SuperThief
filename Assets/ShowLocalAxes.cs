using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLocalAxes : MonoBehaviour
{
    public float axisLength = 1.0f;
    void OnDrawGizmosSelected()
    {
        // Draw the local X axis in red
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * axisLength);

        // Draw the local Y axis in green
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * axisLength);

        // Draw the local Z axis in blue
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * axisLength);
    }
}

