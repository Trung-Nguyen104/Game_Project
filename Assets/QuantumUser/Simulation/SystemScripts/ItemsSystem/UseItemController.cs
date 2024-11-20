namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class UseItemController : SystemMainThreadFilter<UseItemController.Filter>
    {
        private RuntimePlayer playerData;
        private Input* input;
        private ItemInfo currItem;

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerInfo* PlayerInfo;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            playerData = frame.GetPlayerData(filter.PlayerInfo->PlayerRef);
            input = frame.GetPlayerInput(filter.PlayerInfo->PlayerRef);
            currItem = filter.PlayerInfo->Inventory[filter.PlayerInfo->CurrSelectItem];

            if (input->InteracAction.WasPressed && frame.TryFindAsset(currItem.Item.ItemData, out var item))
            {   
                if(item.itemType == ItemType.Weapon)
                {
                    return;
                }
                if (item.itemType == ItemType.Heal)
                {
                    playerData.CurrHealth += item.heal.AsInt;
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
