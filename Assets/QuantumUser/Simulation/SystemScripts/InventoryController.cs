namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class InventoryController : SystemMainThreadFilter<InventoryController.Filter>
    {
        private Input* input;
        private RuntimePlayer playerLocal;
        private GameSession* gameSession;
        private EntityRef itemDropRef;

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerInfo* PlayerInfo;
            public Transform2D* Transform;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            input = frame.GetPlayerInput(filter.PlayerInfo->PlayerRef);
            playerLocal = frame.GetPlayerData(filter.PlayerInfo->PlayerRef);
            gameSession = frame.Unsafe.GetPointerSingleton<GameSession>();

            SelectItemHandler(frame, filter);
            if (frame.TryFindAsset(filter.PlayerInfo->Inventory[filter.PlayerInfo->CurrSelectItem].Item.ItemData, out var currItemData))
            {
                DropItemHandler(frame, filter, currItemData);
                ResetInventory(frame, filter, currItemData);
            }
        }

        private void ResetInventory(Frame frame, Filter filter, ItemData currItemData)
        {
            if (gameSession->GameState != GameState.GameStarted)
            {
                if (gameSession->GameState == GameState.GameEnded)
                {
                    for (int i = 0; i < filter.PlayerInfo->Inventory.Length; i++)
                    {
                        ClearUpInventorySlot(i, filter.PlayerInfo->Inventory, filter, currItemData);
                        frame.Events.DropItem(filter.PlayerInfo->PlayerRef, i, currItemData);
                    }
                }
                return;
            }
        }

        private void SelectItemHandler(Frame frame, Filter filter)
        {
            if (input->SelectItem > 0)
            {
                var inputSelecIndex = input->SelectItem - 1;
                filter.PlayerInfo->CurrSelectItem = inputSelecIndex;
                frame.Events.SelectItem(filter.PlayerInfo->PlayerRef, inputSelecIndex);
            }
        }

        private void DropItemHandler(Frame frame, Filter filter, ItemData currItemData)
        {
            if (input->DropItem.WasPressed && filter.PlayerInfo->CurrSelectItem > -1)
            {
                var currSelectItem = filter.PlayerInfo->CurrSelectItem;
                var playerInventory = filter.PlayerInfo->Inventory;

                if (currItemData != null)
                {
                    SetUpDropItem(frame, filter, playerInventory[currSelectItem]);
                    HandleItemWeapon(frame, playerInventory[currSelectItem]);
                    ClearUpInventorySlot(currSelectItem, playerInventory, filter, currItemData);
                    frame.Events.DropItem(filter.PlayerInfo->PlayerRef, filter.PlayerInfo->CurrSelectItem, currItemData);
                }
            }
        }

        private void SetUpDropItem(Frame frame, Filter filter, ItemInfo currItem)
        {
            itemDropRef = frame.Create(currItem.Item.ItemPrototype);
            var itemDropTransform = frame.Unsafe.GetPointer<Transform2D>(itemDropRef);
            var itemDropPhysis2D = frame.Unsafe.GetPointer<PhysicsBody2D>(itemDropRef);

            itemDropPhysis2D->AddLinearImpulse(playerLocal.ShootPointDirection * 1);
            itemDropTransform->Rotation = frame.Get<MousePointerInfo>(filter.Entity).AimAngle;
            itemDropTransform->Position = playerLocal.ShootPointPosition;
        }

        private void HandleItemWeapon(Frame frame, ItemInfo currItem)
        {
            var itemDropInfo = frame.Unsafe.GetPointer<ItemInfo>(itemDropRef);
            itemDropInfo->GunAmmo = currItem.GunAmmo;
        }

        private void ClearUpInventorySlot(int currSelectItem, FixedArray<ItemInfo> playerInventory, Filter filter, ItemData currItemData)
        {
            if (currItemData.itemType == ItemType.Weapon)
            {
                filter.PlayerInfo->HadWeapon = false;
            }
            playerInventory[currSelectItem].Item.ItemData = null;
            playerInventory[currSelectItem].Item.ItemPrototype = null;
            playerInventory[currSelectItem].GunAmmo = 0;
            playerInventory[currSelectItem].BulletPrototype = null;
        }
    }
}
