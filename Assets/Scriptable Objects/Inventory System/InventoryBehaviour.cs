using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

public class InventoryBehaviour : MonoBehaviour {
    [SerializeField] private List<ItemStack> Inventory = new();

    // Picking up items
    private void OnTriggerEnter(Collider other) {
        
        Debug.Log("Entered Collider");
        //Assert.IsTrue(TryGetComponent<ItemBehaviour>(out ItemBehaviour ib));
        // Add item to inventory 
        if(other.TryGetComponent(out ItemBehaviour Item)) 
        {
            Inventory.Add(Item.itemStack); 
            Destroy(Item.gameObject); 
        }
        else 
        {
            Debug.Log("ItemBehaviour not found");
        }
    }
}