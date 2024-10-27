namespace Quantum
{
    using Photon.Deterministic;
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
            var playerLocal = frame.GetPlayerData(filter.PlayerInfo->PlayerRef);
            var input = frame.GetPlayerInput(filter.PlayerInfo->PlayerRef);
            var currItem = filter.PlayerInfo->Inventory[filter.PlayerInfo->CurrSelectItem];

            if (input->InteracAction.WasPressed && frame.TryFindAsset(currItem.Item.ItemData, out var item))
            {   
                if(item.itemType == ItemType.Weapon)
                {
                    return;
                }
                if (item.itemType == ItemType.Heal)
                {
                    filter.PlayerInfo->Health += item.heal;
                    frame.Events.DropItem(filter.PlayerInfo->PlayerRef, filter.PlayerInfo->CurrSelectItem, item);
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
