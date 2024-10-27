namespace Quantum
{
    using Photon.Deterministic;
    using System.Collections.Generic;
    using UnityEngine.Scripting;
    using UnityEngine;

    [Preserve]
    public unsafe class ItemSpawnController : SystemMainThreadFilter<ItemSpawnController.Filter>, ISignalOnPickUpItem, ISignalOnUseItem
    {
        private List<EntityRef> listItemEntityRef = new();
        private EntityRef _entity;
        private GameSession* gameSession;
        private FP timeCounter;
        private bool canCounter;
        private int index;

        public struct Filter
        {
            public EntityRef Entity;
            public ItemSpawner* ItemSpawner;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            _entity = filter.Entity;
            gameSession = frame.Unsafe.GetPointerSingleton<GameSession>();
            if (gameSession->GameState != GameState.GameStarted)
            {
                ResetItemQuantity(frame, filter);
                return;
            }
            SetUpItemSpawnPosition(frame, filter);
            SpawnItemHandler(frame, filter);
            RespawnItemHandler(frame, filter);
        }

        private static void SetUpItemSpawnPosition(Frame frame, Filter filter)
        {
            var itemSpawnPosition = frame.FindAsset(filter.ItemSpawner->ItemSpawnPosition);
            if (filter.ItemSpawner->LoadAllPosition)
            {
                return;
            }
            for (int i = 0; i < itemSpawnPosition.positions.Count; i++)
            {
                filter.ItemSpawner->Positions[i].Position = itemSpawnPosition.positions[i];
            }
            filter.ItemSpawner->LoadAllPosition = true;
        }

        private void SpawnItemHandler(Frame frame, Filter filter)
        {
            var listItemSpawnPosition = filter.ItemSpawner->Positions;
            var listItem = filter.ItemSpawner->Item;

            var randomPosition = QuantumRandomHandler(frame, 0, filter.ItemSpawner->Positions.Length);
            var randomItem = QuantumRandomHandler(frame, 0, filter.ItemSpawner->Item.Length);
            var itemData = frame.FindAsset(filter.ItemSpawner->Item[randomItem].ItemProfile.ItemData);

            if (!listItemSpawnPosition[randomPosition].isSpawned && listItem[randomItem].ItemQuantity < itemData.itemQuantity)
            {
                var itemEnityRef = frame.Create(listItem[randomItem].ItemProfile.ItemPrototype);
                listItemEntityRef.Add(itemEnityRef);
                var itemInfo = frame.Unsafe.GetPointer<ItemInfo>(itemEnityRef);
                var itemTransform = frame.Unsafe.GetPointer<Transform2D>(itemEnityRef);

                if (itemData.itemType == ItemType.Weapon)
                {
                    itemInfo->GunAmmo = QuantumRandomHandler(frame, 0, (int)itemData.ammo);
                }

                listItemSpawnPosition[randomPosition].isSpawned = true;
                listItem[randomItem].ItemQuantity += 1;
                itemTransform->Rotation = QuantumRandomHandler(frame, -45, 45);
                itemTransform->Position = listItemSpawnPosition[randomPosition].Position;
            }
        }

        private void ResetItemQuantity(Frame frame, Filter filter)
        {
            if(gameSession->GameState != GameState.GameEnded)
            {
                return;
            }
            for (int i = 0; i < filter.ItemSpawner->Item.Length; i++)
            {
                filter.ItemSpawner->Item[i].ItemQuantity = 0;
            }
            for (int i = 0; i < filter.ItemSpawner->Positions.Length; i++)
            {
                filter.ItemSpawner->Positions[i].isSpawned = false;
            }
            if (listItemEntityRef.Count > 0)
            {
                for (int i = 0; i < listItemEntityRef.Count; i++)
                {
                    frame.Destroy(listItemEntityRef[i]);
                }
            }
            listItemEntityRef.Clear();
        }

        private int QuantumRandomHandler(Frame frame, int minInclusive, int maxExclusive)
        {
            var result = frame.RNG->Next(minInclusive, maxExclusive);
            return result;
        }

        private void RespawnItemHandler(Frame frame, Filter filter)
        {
            if (canCounter)
            {
                timeCounter -= frame.DeltaTime;
                if (timeCounter < 0)
                {
                    filter.ItemSpawner->Item[index].ItemQuantity -= 1;
                    canCounter = false;
                }
            }
        }

        public void OnPickUpItem(Frame frame, FPVector2 Position)
        {
            if (frame.Unsafe.TryGetPointer<ItemSpawner>(_entity, out var itemSpawner))
            {
                for (int i = 0; i < itemSpawner->Positions.Length; i++)
                {
                    if (itemSpawner->Positions[i].Position == Position)
                    {
                        itemSpawner->Positions[i].isSpawned = false;
                        break;
                    }
                }
            }
        }

        public void OnUseItem(Frame frame, ItemInfo ItemInfo)
        {
            if (frame.TryGet<ItemSpawner>(_entity, out var itemSpawner))
            {
                for (int i = 0; i < itemSpawner.Item.Length; i++)
                {
                    if (itemSpawner.Item[i].ItemProfile.ItemPrototype == ItemInfo.Item.ItemPrototype)
                    {
                        timeCounter = frame.FindAsset(itemSpawner.Item[i].ItemProfile.ItemData).respawnTime;
                        index = i;
                        canCounter = true;
                        break;
                    }
                }
            }
        }
    }
}
