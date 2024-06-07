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
        instance = this;
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
        int randomIndex = Random.Range(0, lootPrefabs.Length);
        GameObject lootPrefab = lootPrefabs[randomIndex];
        PhotonNetwork.Instantiate(lootPrefab.name, spawnPoint.position, Quaternion.identity);
    }

    public Transform GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex];
    }
}