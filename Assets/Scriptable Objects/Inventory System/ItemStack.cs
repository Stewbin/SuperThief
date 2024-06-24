using UnityEngine;
using System;

[Serializable]
public class ItemStack
{
    public ItemDetails Item;
    public int Amount;

    public ItemStack(ItemDetails _item, int _amount)
    {
        Item = _item;
        Amount = _amount;
    }

    public ItemStack(ItemStack itemStack) 
    {
        Item = itemStack.Item;
        Amount = itemStack.Amount;
    }
    

    public void AddAmount(int amount) {this.Amount += amount;}

}