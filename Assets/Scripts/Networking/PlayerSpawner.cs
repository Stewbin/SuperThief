using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public static PlayerSpawner instance;

    private GameObject player;

    public GameObject playerPrefab;

    //public TMP_Text killFeedText;


 
    //public GameObject nameUI; 

     public GameObject deathEffect;

     public float respawnTime = 5f; 

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }


    public void Die(string damager){

       

        UIController.instance.deathText.text = "Eliminated By " + damager;
        UIController.instance.respawnText.text = "Going Back In 5s "; 
        //PhotonNetwork.Destroy(player); 
       

        //SpawnPlayer();

        print($"Player is dead, killed by {damager}");

        MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1); 


        if(player != null){
            
            
            
            StartCoroutine(DieCoroutine());
            
            
        }

       
            
        
    }   

public IEnumerator DieCoroutine()
{
    PhotonNetwork.Instantiate(deathEffect.name, player.transform.position, Quaternion.identity);

    PhotonNetwork.Destroy(player);
    player = null;

    UIController.instance.deathScreen.SetActive(true);

    UIController.instance.leaderboardComponent.SetActive(false);
    UIController.instance.healthComponent.SetActive(false);
    UIController.instance.statsComponent.SetActive(false);
    UIController.instance.GunComponent.SetActive(false);
    UIController.instance.timeComponent.SetActive(false);
    UIController.instance.optionComponent.SetActive(false);



    float countdownTime = respawnTime;

    while (countdownTime > 0)
    {
        UIController.instance.respawnText.text = "Going Back in " + Mathf.CeilToInt(countdownTime) + "s";
        yield return new WaitForSeconds(1f);
        countdownTime--;
    }

    UIController.instance.deathScreen.SetActive(false);

    UIController.instance.leaderboardComponent.SetActive(true);
    UIController.instance.healthComponent.SetActive(true);
    UIController.instance.statsComponent.SetActive(true);
    UIController.instance.GunComponent.SetActive(true);
    UIController.instance.timeComponent.SetActive(true);
    UIController.instance.optionComponent.SetActive(true);


    if (MatchManager.instance.state == MatchManager.GameState.Playing && player == null)
    {
        SpawnPlayer();

        
    }



}
}