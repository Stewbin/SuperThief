using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Drawing;
using Photon.Realtime;

public class EnemyManager : MonoBehaviourPunCallbacks
{
    public Transform[] SpawnPoints;
    public GameObject[] EnemyPrefabs;
    public int MaxEnemies = 20;
    public float spawnTime = 10f;

    private void Start()
    {
        PhotonNetwork.OfflineMode = true;
        foreach(var point in SpawnPoints)
        {
            point.gameObject.SetActive(false);
        }

        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinOrCreateRoom("MyRoom", new Photon.Realtime.RoomOptions(), TypedLobby.Default);
            StartCoroutine(SpawnEnemies());
        }
    }

    public IEnumerator SpawnEnemies()
    {
        while(true)
        {
            if(EnemyBehaviour.Instances.Count < MaxEnemies)
            {
                // Pick enemy and spawn point
                int randSpawn = Random.Range(0, SpawnPoints.Length - 1);
                Transform spawnPoint = SpawnPoints[randSpawn];
                
                int randEnemy = Random.Range(0, EnemyPrefabs.Length - 1);
                var enemyPrefab = EnemyPrefabs[randEnemy];
                
                // Instantiate in network
                GameObject enemy = Instantiate(enemyPrefab);
                PhotonView photonView = enemy.GetComponent<PhotonView>();
                
                if(PhotonNetwork.AllocateViewID(photonView))
                {
                    object[] details = new object[]
                    {
                        spawnPoint.position, spawnPoint.rotation, photonView.ViewID
                    };
                }
            
                yield return new WaitForSeconds(spawnTime);
            }
        }
    }  
}