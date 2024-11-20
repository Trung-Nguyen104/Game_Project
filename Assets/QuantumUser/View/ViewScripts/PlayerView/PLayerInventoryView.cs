namespace Quantum
{
    using UnityEngine;

    public class PLayerInventoryView : QuantumEntityViewComponent
    {
        private PlayerInfo playerInfo;
        private RuntimePlayer playerData;
        private GameSession gameSession;
        private ItemSlotsView[] itemSlots;

        private int lastSelectItem;

        private void Start()
        {
            if (InventorySlot.Instance != null)
            {
                itemSlots = InventorySlot.Instance.itemSlots;
            }
            QuantumEvent.Subscribe(listener: this, handler: (EventPickUpItem e) => AddItemSlotEvent(e));
            QuantumEvent.Subscribe(listener: this, handler: (EventSelectItem e) => SelectItemEvent(e));
            QuantumEvent.Subscribe(listener: this, handler: (EventRemoveItem e) => RemoveItemSlotEvent(e));
        }

        private void Update()
        {
            playerInfo = VerifiedFrame.Get<PlayerInfo>(_entityView.EntityRef);
            playerData = VerifiedFrame.GetPlayerData(playerInfo.PlayerRef);
            gameSession = VerifiedFrame.GetSingleton<GameSession>();
            InventoryHandler();
        }

        private bool CheckPlayerLocal() => QuantumRunner.DefaultGame.PlayerIsLocal(playerInfo.PlayerRef);
        private GameObject FindWeaponObject(string itemName) => transform.Find("Aim/" + itemName).gameObject;
        private GameObject FindHealObject(string itemName) => transform.Find("Animator/Hand/" + itemName).gameObject;
        private ItemData GetItemData(int index) => VerifiedFrame.FindAsset(playerInfo.Inventory[index].Item.ItemData);

        private void AddItemSlotEvent(EventPickUpItem e)
        {
            if (e.PlayerRef == playerInfo.PlayerRef)
            {
                var item = VerifiedFrame.FindAsset(e.ItemPickUpData);
                if (CheckPlayerLocal())
                {
                    itemSlots[e.ItemSlotIndex].AddItemSlot(item, e.PlayerRef);
                }
            }
        }

        private void RemoveItemSlotEvent(EventRemoveItem e)
        {
            if (e.PlayerRef == playerInfo.PlayerRef)
            {
                var itemDropData = VerifiedFrame.FindAsset(e.ItemDropData);
                ItemSetActive(itemDropData, false);
                if (CheckPlayerLocal())
                {
                    itemSlots[e.CurrSelectItem].DeselectItem();
                    itemSlots[e.CurrSelectItem].RemoveItemSlot();
                }
            }
        }

        private void SelectItemEvent(EventSelectItem e)
        {
            if (e.PlayerRef == playerInfo.PlayerRef)
            {
                
                ItemSetActive(GetItemData(e.SelectItem), true);
                for(int i = 0 ; i < playerInfo.Inventory.Length ; i++)
                {
                    if(i != e.SelectItem && playerInfo.Inventory[e.SelectItem].Item.ItemPrototype != playerInfo.Inventory[i].Item.ItemPrototype)
                    {
                        ItemSetActive(GetItemData(i), false);
                    }
                }
                if (CheckPlayerLocal() && itemSlots[e.SelectItem].isFull)
                {
                    itemSlots[lastSelectItem].DeselectItem();
                    itemSlots[e.SelectItem].SelectItem();
                    lastSelectItem = e.SelectItem;
                }
            }
        }

        private void ItemSetActive(ItemData itemData, bool setActive)
        {
            if (itemData == null)
            {
                return;
            }
            if (itemData.itemType == ItemType.Weapon)
            {
                FindWeaponObject(itemData.itemName).SetActive(setActive);
            }
            else if (itemData.itemType == ItemType.Heal)
            {
                FindHealObject(itemData.itemName).SetActive(setActive);
            }
        }

        private void InventoryHandler()
        {
            if (!CheckPlayerLocal())
            {
                return;
            }
            if (gameSession.GameState != GameState.GameStarted || playerData.CurrHealth <= 0)
            {
                SetActiveInventory(false);
            }
            else
            {
                SetActiveInventory(true);
            }
        }

        private void SetActiveInventory(bool boolValue)
        {
            InventorySlot.Instance.gameObject.SetActive(boolValue);
        }
    }
}
