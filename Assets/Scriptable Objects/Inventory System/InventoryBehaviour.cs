using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class InventoryBehaviour : MonoBehaviour {
    [SerializeField] private List<ItemStack> Inventory = new();

    // Picking up items
    private void OnTriggerEnter(Collider other) {
        // Add item to inventory 
        if(TryGetComponent(out ItemBehaviour itemStack)) {
            
            Destroy(other.gameObject);
        }
    }

    public class ItemStack
    {
        public ItemDetails Item;
        public int amount {get; private set;} = 1;

        public void AddAmount(int amount) {this.amount += amount;}

    }
}