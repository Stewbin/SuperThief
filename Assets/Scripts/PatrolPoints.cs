using UnityEngine;
using UnityEngine.AI;

public class PatrolPoints : MonoBehaviour 
{
    public Transform[] WayPoints;
    [SerializeField] private NavMeshAgent agent;
    private Transform _currentPoint;
    [SerializeField] private int _index;
    
    void Start()
    {
        _currentPoint = WayPoints[0];
    }

    void Update()
    {
        if(!agent.pathPending)
        {
            _currentPoint = WayPoints[_index];
            _index++;
        }

        if(_index > WayPoints.Length)
        {
            _index
        }
    }
}