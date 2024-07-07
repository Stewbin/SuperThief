using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Drawing;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public Transform[] SpawnPoints;
    public GameObject[] EnemyPrefabs;
    public int MaxEnemies = 20;
    public float spawnTime = 10f;

    private void Start()
    {
        foreach(var point in SpawnPoints)
        {
            point.gameObject.SetActive(false);
        }
        StartCoroutine(SpawnEnemies());
    }

    public IEnumerator SpawnEnemies()
    {
        while(true)
        {
            if(EnemyBehaviour.instances.Count < MaxEnemies)
            {
                int randSpawn = Random.Range(0, SpawnPoints.Length);
                Transform spawnPoint = SpawnPoints[randSpawn];
                
                int randEnemy = Random.Range(0, EnemyBehaviour.instances.Count);
                var enemyPrefab = EnemyPrefabs[randEnemy];
                PhotonNetwork.Instantiate(enemyPrefab.name, spawnPoint.position, spawnPoint.rotation);
                
                yield return new WaitForSeconds(spawnTime);
            }
        }
    }  
}