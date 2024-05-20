using UnityEngine;
using System.Collections.Generic;

public class InventoryBehaviour : MonoBehaviour {
    Dictionary<ItemObject, int> Inventory = new(5);

    // Picking up items
    private void OnTriggerEnter(Collider other) {
        // Add item to inventory 
        if(TryGetComponent(out ItemBehaviour item)) {
            Inventory.Add(item.itemObject, item.Amount);
            Destroy(other.gameObject);
        }
    }
}