namespace Quantum
{
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class CollisionSystem : SystemSignalsOnly, ISignalOnCollision2D, ISignalOnCollisionEnter2D
    {
        public void OnCollision2D(Frame frame, CollisionInfo2D info)
        {
            if (frame.Unsafe.TryGetPointer<PlayerInfo>(info.Other, out var playerInfo))
            {
                var input = frame.GetPlayerInput(playerInfo->PlayerRef);
                info.IgnoreCollision = true;

                if (frame.TryGet<ItemInfo>(info.Entity, out var itemInfo))
                {
                    if (input->PickUpItem.WasPressed)
                    {
                        PickUpItem(frame, info, playerInfo, itemInfo);
                    }
                }
            }
        }

        private static void PickUpItem(Frame frame, CollisionInfo2D info, PlayerInfo* playerInfo, ItemInfo itemInfo)
        {
            if (frame.GetPlayerData(playerInfo->PlayerRef).CurrHealth <= 0)
            {
                return;
            }
            var item = frame.FindAsset(itemInfo.Item.ItemData);
            var itemPosition = frame.Get<Transform2D>(info.Entity).Position.XY;
            for (int i = 0; i < playerInfo->Inventory.Length; i++)
            {
                if (playerInfo->HadWeapon && item.itemType == ItemType.Weapon)
                {
                    return;
                }
                if (playerInfo->Inventory[i].Item.ItemData == null)
                {
                    if (item.itemType == ItemType.Weapon)
                    {
                        playerInfo->HadWeapon = true;
                    }
                    playerInfo->Inventory[i] = itemInfo;
                    frame.Events.PickUpItem(playerInfo->PlayerRef, i, item);
                    frame.Signals.OnPickUpItem(itemPosition);
                    frame.Destroy(info.Entity);
                    break;
                }
            }
        }

        public void OnCollisionEnter2D(Frame frame, CollisionInfo2D info)
        {
            if(frame.TryGet<BulletInfo>(info.Entity, out var bulletInfo))
            {
                if(frame.Unsafe.TryGetPointer<PlayerInfo>(info.Other, out var playerInfo))
                {
                    if(playerInfo->PlayerRef != bulletInfo.OwnerPlayer)
                    {
                        frame.GetPlayerData(playerInfo->PlayerRef).CurrHealth -= bulletInfo.Damage.AsInt;
                    }
                    else
                    {
                        info.IgnoreCollision = true;
                    }
                }
                frame.Destroy(info.Entity);
            }
        }
    }
}
