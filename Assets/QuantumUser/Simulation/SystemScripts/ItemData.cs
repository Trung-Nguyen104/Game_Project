namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine;

    public enum ItemType
    {
        Weapon,
        Tool,
        Heal
    }
    public class ItemData : AssetObject
    {
        public ItemType itemType;
        public string itemName;
        public Sprite icon;
        public FP damge;
        public FP ammo;
        public FP heal;
        public FP itemQuantity;
        public FP respawnTime;
    }
}
