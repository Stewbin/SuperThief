using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LootSpawnManager : MonoBehaviourPunCallbacks
{
    public static LootSpawnManager instance;

    public GameObject[] lootPrefabs;
    public Transform[] spawnPoints;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                SpawnRandomLoot(spawnPoint);
            }
        }
    }

    public void SpawnRandomLoot(Transform spawnPoint)
    {
        if (lootPrefabs.Length > 0 && spawnPoint != null)
        {
            int randomIndex = Random.Range(0, lootPrefabs.Length);
            GameObject lootPrefab = lootPrefabs[randomIndex];
            PhotonNetwork.Instantiate(lootPrefab.name, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No loot prefabs found or invalid spawn point.");
        }
    }

    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex];
        }
        else
        {
            Debug.LogError("No spawn points found in LootSpawnManager.");
            return null;
        }
    }
}