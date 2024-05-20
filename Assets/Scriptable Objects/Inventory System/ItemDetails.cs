using UnityEngine;

public enum ItemType 
{
    Gun,
    Melee,
    Consumable,
    Throwable
}

/// <summary>
/// The Scriptable Object class from which all Item Scriptable Objects iherit from. 
/// Despite the name, this class is NOT meant to be instantiated or live as any sort of
/// object anywhere.
/// </summary>
public abstract class ItemDetails : ScriptableObject 
{
    [SerializeField] private GameObject scenePrefab;
    [SerializeField] private Sprite displayIcon;
    public ItemType itemType;
    [TextArea(10,5)] public string description;
    public delegate void UseItem();
}