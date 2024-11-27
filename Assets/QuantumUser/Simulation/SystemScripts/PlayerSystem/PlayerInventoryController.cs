namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerInventoryController : SystemMainThreadFilter<PlayerInventoryController.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerInfo* PlayerInfo;
            public Transform2D* Transform;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            var input = frame.GetPlayerInput(filter.PlayerInfo->PlayerRef);
            var playerData = frame.GetPlayerData(filter.PlayerInfo->PlayerRef);
            var gameSession = frame.Unsafe.GetPointerSingleton<GameSession>();
            if (playerData.CurrHealth <= 0 || gameSession->GameState == GameState.GameEnding)
            {
                CleanUpInventory(frame, filter, input, playerData);
                return;
            }
            SelectItem(frame, filter, input);
            HandleCurrentItemData(frame, filter, input);
        }

        private void HandleCurrentItemData(Frame frame, Filter filter, Input* input)
        {
            if (filter.PlayerInfo->CurrSelectItem < 0)
            {
                return;
            }
            if (frame.TryFindAsset(filter.PlayerInfo->Inventory[filter.PlayerInfo->CurrSelectItem].Item.ItemData, out var currItemData))
            {
                if (input->DropItem.IsDown && filter.PlayerInfo->CurrSelectItem > -1)
                {
                    DropItem(frame, filter, currItemData, input);
                }
            }
        }

        private void CleanUpInventory(Frame frame, Filter filter, Input* input, RuntimePlayer playerData)
        {
            for (int i = 0; i < filter.PlayerInfo->Inventory.Length; i++)
            {
                if (frame.TryFindAsset(filter.PlayerInfo->Inventory[i].Item.ItemData, out var currItemData))
                {
                    if (playerData.CurrHealth <= 0)
                    {
                        DropItem(frame, filter, currItemData, input);
                    }
                    ClearUpInventorySlot(i, filter.PlayerInfo->Inventory, filter, currItemData);
                    frame.Events.RemoveItem(filter.PlayerInfo->PlayerRef, i, currItemData);
                }
            }
        }

        private void SelectItem(Frame frame, Filter filter, Input* input)
        {
            if (input->SelectItem > 0)
            {
                if (input->SelectItem - 1 == filter.PlayerInfo->CurrSelectItem)
                {
                    filter.PlayerInfo->CurrSelectItem = -1;
                }
                var inputSelecIndex = input->SelectItem - 1;
                filter.PlayerInfo->CurrSelectItem = inputSelecIndex;
                frame.Events.SelectItem(filter.PlayerInfo->PlayerRef, inputSelecIndex);
            }
        }

        private void DropItem(Frame frame, Filter filter, ItemData currItemData, Input* input)
        {
            var currSelectItem = filter.PlayerInfo->CurrSelectItem;
            var playerInventory = filter.PlayerInfo->Inventory;

            if (currItemData != null)
            {
                SetUpDropItem(frame, filter, playerInventory[currSelectItem], input);
                ClearUpInventorySlot(currSelectItem, playerInventory, filter, currItemData);
                frame.Events.RemoveItem(filter.PlayerInfo->PlayerRef, filter.PlayerInfo->CurrSelectItem, currItemData);
            }
        }

        private void SetUpDropItem(Frame frame, Filter filter, ItemInfo currItem, Input* input)
        {
            var itemDropRef = frame.Create(currItem.Item.ItemPrototype);
            var itemDropTransform = frame.Unsafe.GetPointer<Transform2D>(itemDropRef);
            var itemDropPhysis2D = frame.Unsafe.GetPointer<PhysicsBody2D>(itemDropRef);

            if (currItem.BulletPrototype != null)
            {
                HandleItemWeapon(frame, currItem, itemDropRef);
            }

            itemDropPhysis2D->AddLinearImpulse(input->ShootPointDirection * 1);
            itemDropTransform->Rotation = frame.Get<MousePointerInfo>(filter.Entity).AimAngle;
            itemDropTransform->Position = input->ShootPointPosition;
        }

        private void HandleItemWeapon(Frame frame, ItemInfo currItem, EntityRef itemDropRef)
        {
            var itemDropInfo = frame.Unsafe.GetPointer<ItemInfo>(itemDropRef);
            itemDropInfo->CurrentAmmo = currItem.CurrentAmmo;
        }

        private void ClearUpInventorySlot(int currSelectItem, FixedArray<ItemInfo> playerInventory, Filter filter, ItemData currItemData)
        {
            if (currItemData.itemType == ItemType.Weapon)
            {
                filter.PlayerInfo->HadWeapon = false;
            }
            playerInventory[currSelectItem].Item.ItemData = null;
            playerInventory[currSelectItem].Item.ItemPrototype = null;
            playerInventory[currSelectItem].CurrentAmmo = 0;
            playerInventory[currSelectItem].BulletPrototype = null;
        }
    }
}
