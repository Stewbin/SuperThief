using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Item", menuName = "Invetory System/GunItem")]
public class GunItem : ItemDetails {
    public event UseItem OnShoot;
    [SerializeField] private int BulletsPerClip; 
    [SerializeField] private int BulletsPerSecond;
    [SerializeField] private float Damage;

    private void Awake() {
        itemType = ItemType.Gun;
    }
}