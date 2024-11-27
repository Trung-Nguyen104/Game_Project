namespace Quantum
{
    using UnityEngine;
    using UnityEngine.UI;

    public class ItemSlotsView : QuantumEntityViewComponent
    {
        public ItemData ItemData { get; private set; }
        public PlayerRef PlayerRef { get; private set; }
        public bool IsFull { get; private set; } = false;

        private Image image;
        private Color originalColor;

        private void Start()
        {
            image = GetComponent<Image>();
            originalColor = image.color;
        }

        public void AddItemSlot(ItemData _item, PlayerRef _playerRef)
        {
            ItemData = _item;
            PlayerRef = _playerRef;
            image.color = Color.white;
            image.sprite = _item.icon;
            IsFull = true;
        }

        public void RemoveItemSlot()
        {
            image.color = originalColor;
            image.sprite = null;
            IsFull = false;
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
