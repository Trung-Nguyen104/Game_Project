namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class ItemSpawnController : SystemMainThreadFilter<ItemSpawnController.Filter>, ISignalOnPickUpItem, ISignalOnUseItem
    {
        private EntityRef _entity;
        private FP timeCounter;
        private int index;
        private bool canCounter = false;

        public struct Filter
        {
            public EntityRef Entity;
            public ItemSpawner* ItemSpawner;
        }

        public override void OnInit(Frame frame)
        {
            base.OnInit(frame);
            frame.Global->listItemEntityRef = frame.AllocateList<EntityRef>();
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            _entity = filter.Entity;
            var gameSession = frame.Unsafe.GetPointerSingleton<GameSession>();
            filter.ItemSpawner->RNGValue = new RNGSession(frame.RuntimeConfig.Seed);

            HandleGameSession(frame, filter, gameSession);
        }

        private void HandleGameSession(Frame frame, Filter filter, GameSession* gameSession)
        {
            if (gameSession->GameState == GameState.Waiting)
            {
                return;
            }
            if (gameSession->GameState == GameState.GameStarting)
            {
                SetUpItemSpawnPosition(frame, filter);
                return;
            }
            if (gameSession->GameState == GameState.GameEnding)
            {
                CleanUpItem(frame, filter);
                return;
            }
            SpawnItemHandler(frame, filter);
            RespawnItemHandler(frame, filter);
        }

        private void SetUpItemSpawnPosition(Frame frame, Filter filter)
        {
            var itemSpawnPosition = frame.FindAsset(filter.ItemSpawner->ItemSpawnPosition);
            for (int i = 0; i < itemSpawnPosition.positions.Count; i++)
            {
                filter.ItemSpawner->Positions[i].Position = itemSpawnPosition.positions[i];
            }
        }

        private void SpawnItemHandler(Frame frame, Filter filter)
        {
            var listItemEntityRef = frame.ResolveList(frame.Global->listItemEntityRef);
            var listItemSpawnPosition = filter.ItemSpawner->Positions;
            var listItem = filter.ItemSpawner->Item;

            for (int i = 0; i < listItem.Length; i++)
            {
                var itemData = frame.FindAsset(filter.ItemSpawner->Item[i].ItemProfile.ItemData);
                while (listItem[i].ItemQuantity < itemData.itemQuantity)
                {
                    var randomPosition = filter.ItemSpawner->RNGValue.Next(0, listItemSpawnPosition.Length);
                    if (!listItemSpawnPosition[randomPosition].isSpawned)
                    {
                        var itemEnityRef = frame.Create(listItem[i].ItemProfile.ItemPrototype);
                        var itemInfo = frame.Unsafe.GetPointer<ItemInfo>(itemEnityRef);
                        var itemTransform = frame.Unsafe.GetPointer<Transform2D>(itemEnityRef);

                        itemTransform->Rotation = filter.ItemSpawner->RNGValue.Next(-(FP)45, (FP)45);
                        itemTransform->Position = listItemSpawnPosition[randomPosition].Position;
                        listItem[i].ItemQuantity += 1;
                        listItemSpawnPosition[randomPosition].isSpawned = true;
                        listItemEntityRef.Add(itemEnityRef);
                    }
                }
            }
        }

        private void CleanUpItem(Frame frame, Filter filter)
        {
            var listItemEntityRef = frame.ResolveList(frame.Global->listItemEntityRef);
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
                listItemEntityRef.Clear();
            }
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
