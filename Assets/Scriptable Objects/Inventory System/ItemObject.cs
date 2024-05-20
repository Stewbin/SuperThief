using UnityEngine;

public enum ItemType 
{
    Gun,
    Melee,
    Consumable,
    Throwable
}

public abstract class ItemObject : ScriptableObject 
{
    private GameObject scenePrefab;
    private Sprite displayIcon;
    public ItemType itemType;
    [TextArea(15,20)] public string description;
    public delegate void UseItem();
}