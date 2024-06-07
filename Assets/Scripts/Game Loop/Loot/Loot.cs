using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Loot : MonoBehaviourPunCallbacks
{
    public enum LootType
    {
        Ammo,
        Healing,
        Explosive
    }

    public LootType lootType;
     public static Loot instance;
    public int amount;

    private bool isCollected = false;

    void Awake(){
        instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient || isCollected)
            return;

        if (other.CompareTag("Player"))
        {
            AdvancedGunSystem playerController = other.GetComponent<AdvancedGunSystem>();
            if (playerController != null)
            {
                CollectLoot(playerController);
                photonView.RPC("SyncLootCollection", RpcTarget.All);
            }
            else
            {
                Debug.LogError("AdvancedGunSystem component not found on the player.");
            }
        }
    }

    private void CollectLoot(AdvancedGunSystem playerController)
    {
        switch (lootType)
        {
            case LootType.Ammo:
                playerController.photonView.RPC("AddAmmo", RpcTarget.All, amount);
                break;
            case LootType.Healing:
                playerController.photonView.RPC("Heal", RpcTarget.All, amount);
                break;
            case LootType.Explosive:
                playerController.photonView.RPC("AddExplosive", RpcTarget.All, amount);
                break;
        }
    }

    [PunRPC]
    private void SyncLootCollection()
    {
        isCollected = true;
        gameObject.SetActive(false);
        StartCoroutine(RespawnLoot());
    }

    private System.Collections.IEnumerator RespawnLoot()
    {
        yield return new WaitForSeconds(30f); // Adjust the respawn time as needed
        photonView.RPC("SyncLootRespawn", RpcTarget.All);
    }

    [PunRPC]
    private void SyncLootRespawn()
    {
        isCollected = false;
        Transform spawnPoint = LootSpawnManager.instance.GetRandomSpawnPoint();
        transform.position = spawnPoint.position;
        gameObject.SetActive(true);
    }
}