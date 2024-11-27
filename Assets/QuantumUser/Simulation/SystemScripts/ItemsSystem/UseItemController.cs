namespace Quantum
{
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class UseItemController : SystemMainThreadFilter<UseItemController.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerInfo* PlayerInfo;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            var playerData = frame.GetPlayerData(filter.PlayerInfo->PlayerRef);
            var input = frame.GetPlayerInput(filter.PlayerInfo->PlayerRef);
            if (filter.PlayerInfo->CurrSelectItem < 0)
            {
                return;
            }
            var currItem = filter.PlayerInfo->Inventory[filter.PlayerInfo->CurrSelectItem];

            if (!frame.TryFindAsset(currItem.Item.ItemData, out var item) || item.itemType == ItemType.Weapon)
            {
                return;
            }
            if (input->UseItemOrShoot.IsDown)
            {
                Debug.Log($"{filter.PlayerInfo->PlayerRef} UseItem");
                if (item.itemType == ItemType.Heal)
                {
                    playerData.CurrHealth += item.heal;
                    frame.Events.RemoveItem(filter.PlayerInfo->PlayerRef, filter.PlayerInfo->CurrSelectItem, item);
                    frame.Signals.OnUseItem(currItem);
                }
                else if (item.itemType == ItemType.Tool)
                {
                    //Resolve some missions
                }
                RemoveItemFromInventory(filter.PlayerInfo->CurrSelectItem, filter.PlayerInfo->Inventory);
            }
        }

        private static void RemoveItemFromInventory(int currSelectItem, FixedArray<ItemInfo> playerInventory)
        {
            playerInventory[currSelectItem].Item.ItemData = null;
            playerInventory[currSelectItem].Item.ItemPrototype = null;
        }
    }
}
