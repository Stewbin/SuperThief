using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SecurityCamBehaviour : MonoBehaviour
{
    [SerializeField] private SphereCollider sphereCollider;
    private List<Transform> FoundPlayers;
    public float AlarmRadius = 5f;
    public float AlertRadius = 15f;
    // Start is called before the first frame update
    void Start()
    {
        sphereCollider.radius = AlarmRadius;
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            AlertOtherEnemies(other.transform);
            FoundPlayers.Add(other.transform);
        }
    }

    void AlertOtherEnemies(Transform player)
    {
        IEnumerable<EnemyBehaviour> nearbyEnemies = 
            from enemy in EnemyBehaviour.Instances
            where Vector3.Distance(transform.position, enemy.transform.position) <= AlertRadius
            select enemy;
        
        foreach(var enemy in nearbyEnemies)
        {
            enemy.SwitchToHuntingState(player);
        }
    }
}
