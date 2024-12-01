namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class CollisionSystem : SystemSignalsOnly, ISignalOnCollision2D, ISignalOnCollisionEnter2D, ISignalOnTriggerExit2D, ISignalOnTrigger2D
    {
        public void OnCollision2D(Frame frame, CollisionInfo2D info)
        {
            if (frame.Unsafe.TryGetPointer<PlayerInfo>(info.Other, out var playerInfo))
            {
                var input = frame.GetPlayerInput(playerInfo->PlayerRef);
                info.IgnoreCollision = true;
                if (frame.TryGet<ItemInfo>(info.Entity, out var itemInfo))
                {
                    if (!frame.TryFindAsset(itemInfo.Item.ItemData, out var itemData))
                    {
                        Debug.Log("Can Find ItemData");
                        return;
                    }
                    if (itemData.itemType != ItemType.Ammo)
                    {
                        Debug.Log("Can Pick Up");
                        if (input->PickUpOrProcessTask.IsDown)
                        {
                            PickUpItem(frame, info, playerInfo, itemInfo, itemData);
                        }
                        return;
                    }
                    GunAmmoItem(frame, info, playerInfo, itemInfo);
                }
            }
        }
        
        public void OnCollisionEnter2D(Frame frame, CollisionInfo2D info)
        {
            BulletCollision(frame, info);
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
                TaskTrigger(frame, info, playerInfo);
            }
        }

        private void GunAmmoItem(Frame frame, CollisionInfo2D info, PlayerInfo* playerInfo, ItemInfo itemInfo)
        {
            if (playerInfo->CurrSelectItem < 0 || !frame.TryFindAsset(playerInfo->Inventory[playerInfo->CurrSelectItem].Item.ItemData, out var currItemData))
            {
                return;
            }
            if (currItemData.itemType == ItemType.Weapon)
            {
                var itemPosition = frame.Get<Transform2D>(info.Entity).Position.XY;
                playerInfo->Inventory[playerInfo->CurrSelectItem].CurrentAmmo = currItemData.maxAmmo.AsInt;

                frame.Signals.OnUseItem(itemInfo);
                frame.Signals.OnPickUpItem(itemPosition);
                frame.Destroy(info.Entity);
            }
        }

        private void TaskTrigger(Frame frame, TriggerInfo2D info, PlayerInfo* playerInfo)
        {
            if (frame.Unsafe.TryGetPointer<TaskInfo>(info.Entity, out var taskInfo))
            {
                var playerData = frame.GetPlayerData(playerInfo->PlayerRef);
                frame.Unsafe.TryGetPointer<Transform2D>(info.Entity, out var taskTransform);
                frame.Unsafe.TryGetPointer<Transform2D>(info.Other, out var playerTransform);

                if (playerData.PlayerRole == PlayerRole.Monster || (playerData.PlayerRole == PlayerRole.Terrorist && playerData.SkillTimer > 0))
                {
                    return;
                }

                frame.Events.IsHighLight(playerInfo->PlayerRef, info.Entity, true);

                var dist = FPVector2.Distance(playerTransform->Position, taskTransform->Position);

                if (frame.GetPlayerInput(playerInfo->PlayerRef)->PickUpOrProcessTask.IsDown && dist <= 1)
                {
                    frame.Events.InitiatingTask(playerInfo->PlayerRef, info.Entity, taskInfo->TaskType);
                }
            }
        }

        private void PickUpItem(Frame frame, CollisionInfo2D info, PlayerInfo* playerInfo, ItemInfo itemInfo, ItemData itemData)
        {
            var itemPosition = frame.Get<Transform2D>(info.Entity).Position.XY;

            for (int i = 0; i < playerInfo->Inventory.Length; i++)
            {
                if (playerInfo->HadWeapon && itemData.itemType == ItemType.Weapon)
                {
                    return;
                }
                if (playerInfo->Inventory[i].Item.ItemData == null)
                {
                    if (itemData.itemType == ItemType.Weapon)
                    {
                        itemInfo.CurrentAmmo = frame.RNG->Next(0, itemData.maxAmmo.AsInt);
                        frame.Set(info.Entity, itemInfo);
                        playerInfo->HadWeapon = true;
                    }
                    playerInfo->Inventory[i] = itemInfo;
                    frame.Events.PickUpItem(playerInfo->PlayerRef, i, itemData);
                    frame.Signals.OnPickUpItem(itemPosition);
                    frame.Destroy(info.Entity);
                    break;
                }
            }
        }

        private void BulletCollision(Frame frame, CollisionInfo2D info)
        {
            if (!frame.Unsafe.TryGetPointer<BulletInfo>(info.Entity, out var bulletInfo))
            {
                return;
            }
            if (!frame.Unsafe.TryGetPointer<PlayerInfo>(info.Other, out var playerInfo))
            {
                Debug.Log("Collision Other Object");
                frame.Destroy(info.Entity);
                return;
            }
            if (playerInfo->PlayerRef == bulletInfo->OwnerPlayer)
            {
                info.IgnoreCollision = true;
            }
            else
            {
                Debug.Log($"Collision {playerInfo->PlayerRef}");
                frame.Events.IsPlayerHitBullet(playerInfo->PlayerRef, bulletInfo->Damage);
                frame.Destroy(info.Entity);
            }
        }
    }
}
