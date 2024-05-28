using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;
   void awake(){

        instance = this; 
    }
    public Transform [] spawnPoints; 

    void Start() {

        foreach(Transform spawn in spawnPoints){

            spawn.gameObject.SetActive(false); 
        }
    }

    void Update() {

    }

    public Transform GetSpawnPoints(){

        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
