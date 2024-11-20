namespace Quantum
{
    using UnityEngine;
    using UnityEngine.UI;

    public class ItemSlotsView : QuantumEntityViewComponent
    {
        private Image image;
        public ItemData itemData { get; private set; }
        public PlayerRef playerRef { get; private set; }
        public bool isFull { get; private set; } = false;

        private void Start()
        {
            image = GetComponent<Image>();
        }

        public void AddItemSlot(ItemData _item, PlayerRef _playerRef)
        {
            itemData = _item;
            playerRef = _playerRef;
            image.color = Color.white;
            image.sprite = _item.icon;
            isFull = true;
        }

        public void RemoveItemSlot()
        {
            image.color = Color.white;
            image.sprite = null;
            isFull = false;
        }

        public void SelectItem()
        {
            image.color = Color.gray;
        }

        public void DeselectItem()
        {
            image.color = Color.white;
        }
    }
}
