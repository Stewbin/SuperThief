using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class CompassController : MonoBehaviour
{
    public Transform[] Objectives;
    public RectTransform[] Markers; 
    public GameObject Player;
    public float CompassWidth = 526;
    [SerializeField] private float angle;
    private float fov;
    private Vector2 xzForward;

    // Start is called before the first frame update
    void Start()
    {
        // Player = GameObject.FindWithTag("Player");
        fov = Player.GetComponentInChildren<Camera>().fieldOfView;
        xzForward = new Vector2(Player.transform.forward.x, Player.transform.forward.z);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Objectives.Length; i++)
        {
            SetObjectiveMarker(Objectives[i], Markers[i]);   
        }
    }

    void SetObjectiveMarker(Transform objective, RectTransform marker) 
    {
        // Calculate position on compass
        Vector3 Distance = objective.position - Player.transform.position;
        Vector2 xzDistance = new Vector2(Distance.x, Distance.z);

        angle = Vector2.SignedAngle(xzDistance, xzForward);
        if(Mathf.Abs(angle) > fov)
        {
            marker.gameObject.SetActive(false);
        }
        else 
        {
            Vector2 newPosition = Mathf.Clamp(2 * angle / fov, -1, 1) * Vector2.right;
            marker.anchoredPosition = CompassWidth / 2 * newPosition; 
        }
        
        // Calculate size of marker

        // Gizmos
        /*
        Debug.DrawRay(Player.transform.position, Player.transform.forward, Color.green);
        Debug.DrawRay(Player.transform.position, Distance, Color.red);
        */
    }
}
