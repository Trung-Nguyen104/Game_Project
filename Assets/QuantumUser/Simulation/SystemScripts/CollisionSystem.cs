namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class CollisionSystem : SystemSignalsOnly, ISignalOnCollision2D, ISignalOnCollisionEnter2D, ISignalOnTriggerEnter2D, ISignalOnTriggerExit2D, ISignalOnTrigger2D
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
        
        public void OnCollisionEnter2D(Frame frame, CollisionInfo2D info)
        {
            if(frame.TryGet<BulletInfo>(info.Entity, out var bulletInfo))
            {
                if(frame.Unsafe.TryGetPointer<PlayerInfo>(info.Other, out var playerInfo))
                {
                    if(playerInfo->PlayerRef != bulletInfo.OwnerPlayer)
                    {
                        frame.GetPlayerData(playerInfo->PlayerRef).CurrHealth -= bulletInfo.Damage.AsInt;
                        frame.Destroy(info.Entity);
                    }
                    else
                    {
                        info.IgnoreCollision = true;
                    }
                }
                frame.Destroy(info.Entity);
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
                        itemInfo.GunAmmo = frame.RNG->Next(0, item.ammo.AsInt);
                        frame.Set(info.Entity, itemInfo);
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

        public void OnTriggerEnter2D(Frame frame, TriggerInfo2D info)
        {
            if (frame.Unsafe.TryGetPointer<PlayerInfo>(info.Other, out var playerInfo))
            {
                frame.Events.IsHighLight(playerInfo->PlayerRef, info.Entity, true); 
            }
        }

        public void OnTriggerExit2D(Frame frame, ExitInfo2D info)
        {
            if (frame.Unsafe.TryGetPointer<PlayerInfo>(info.Other, out var playerInfo))
            {
                frame.Events.IsHighLight(playerInfo->PlayerRef, info.Entity, false);
            }
        }

        public void OnTrigger2D(Frame frame, TriggerInfo2D info)
        {
            if (frame.Unsafe.TryGetPointer<PlayerInfo>(info.Other, out var playerInfo))
            {
                frame.Unsafe.TryGetPointer<Transform2D>(info.Entity, out var taskTransform);
                frame.Unsafe.TryGetPointer<Transform2D>(info.Other, out var playerTransform);

                var dist = FPVector2.Distance(playerTransform->Position, taskTransform->Position);

                if (frame.GetPlayerInput(playerInfo->PlayerRef)->PickUpItem.WasPressed && dist <= 1)
                {
                    Debug.Log("Initiating Task");
                }
            }
        }
    }
}
