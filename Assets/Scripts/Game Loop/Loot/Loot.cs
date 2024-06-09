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
    public int amount;
    public GameObject medkitMesh;
    public GameObject ammoMesh;
    private bool isCollected = false;

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
                if (photonView != null)
                {
                    photonView.RPC("SyncLootCollection", RpcTarget.All);
                }
                else
                {
                    Debug.LogError("PhotonView component not found on the Loot object.");
                }
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
                Destroy(ammoMesh);
                break;
            case LootType.Healing:
                playerController.photonView.RPC("Heal", RpcTarget.All, amount);
                Destroy(medkitMesh);
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
        StartCoroutine(RespawnLoot());
    }

    private IEnumerator RespawnLoot()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(5f); // Adjust the respawn time as needed

        if (PhotonNetwork.IsMasterClient)
        {
            Transform spawnPoint = LootSpawnManager.instance.GetRandomSpawnPoint();
            if (spawnPoint != null)
            {
                transform.position = spawnPoint.position;
                gameObject.SetActive(true);
                photonView.RPC("SyncLootRespawn", RpcTarget.All);
            }
            else
            {
                Debug.LogError("No valid spawn point found in LootSpawnManager.");
            }
        }
    }

    [PunRPC]
    private void SyncLootRespawn()
    {
        isCollected = false;
        gameObject.SetActive(true);
    }
}