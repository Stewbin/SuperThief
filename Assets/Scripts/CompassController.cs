/**using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
**/

/**
public class CompassController : MonoBehaviour
{
    public Transform[] Objectives;
    public RectTransform[] Markers; 
    public GameObject Player;
    public bool ShouldShrinkMarkers;
    [SerializeField] private RectTransform NSWE;
    private float CompassWidth;
    private float fov;
    private Vector2 xzForward;

    // Start is called before the first frame update
    void Start()
    {
        // Player = GameObject.FindWithTag("Player");
        fov = Player.GetComponentInChildren<Camera>().fieldOfView;
        CompassWidth = GetComponent<RectTransform>().rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Objectives.Length; i++)
        {   
            // Foward vector, translated into XZ plane
            xzForward = new Vector2(Player.transform.forward.x, Player.transform.forward.z);
            // Direction to objective
            Vector3 objectiveDirection = Objectives[i].position - Player.transform.position;
            SetMarker(objectiveDirection, Markers[i]);   
        }

        // Cardinal Directions
        Vector3[] directions = {Vector3.forward, Vector3.back, Vector3.right, Vector3.left};
        for (int i = 0; i < 4; i++)
        {
            SetMarker(directions[i], (RectTransform)NSWE.GetChild(i));
        }
    }

    void SetMarker(Vector3 objectiveDirection, RectTransform marker, float size=1) 
    {
        // Calculate position on compass
        Vector2 xzDirection = new(objectiveDirection.x, objectiveDirection.z);

        float angle = Vector2.SignedAngle(xzDirection, xzForward);
        float percentage = angle / fov;
        
        
        if(Mathf.Abs(percentage) > 1.2)
        {
            marker.gameObject.SetActive(false);
        }
        else 
        {
            marker.gameObject.SetActive(true);
            
            Vector2 newPosition = CompassWidth / 2 * Mathf.Clamp(percentage, -1, 1) * Vector2.right;
            marker.anchoredPosition = newPosition; 
        }
        
        // Update marker size
        marker.localScale *= size;
        
        // Gizmos for testing
       
        Debug.Log($"{objective.name}: {percentage}");
        Debug.DrawRay(Player.transform.position, Player.transform.forward, Color.green);
        Debug.DrawRay(Player.transform.position, Direction, Color.red);
     
    }
}
**/