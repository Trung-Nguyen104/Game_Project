namespace Quantum
{
    using Photon.Deterministic;
    using Quantum.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerSpawnController : SystemMainThreadFilter<PlayerSpawnController.Filter>, ISignalOnPlayerAdded, ISignalOnPlayerRemoved
    {
        private FixedArray<Positions> waitingPosition;

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerSpawnPosition* SpawnPosition;
        }

        public override void OnInit(Frame frame)
        {
            base.OnInit(frame);
            frame.Global->playerEntityRefList = frame.AllocateList<EntityRef>();
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            var gameSession = frame.Unsafe.GetPointerSingleton<GameSession>();
            var listPlayerEntityRef = frame.ResolveList(frame.Global->playerEntityRefList);

            HandleGameSession(frame, filter, gameSession, listPlayerEntityRef);
        }

        private void HandleGameSession(Frame frame, Filter filter, GameSession* gameSession, QList<EntityRef> listPlayerEntityRef)
        {
            if (gameSession->GameState == GameState.Waiting)
            {
                ManageLoadPosition(frame, filter);
                return;
            }
            if (gameSession->GameState == GameState.GameStarting)
            {
                for (var i = 0; i < listPlayerEntityRef.Count; i++)
                {
                    var playerTransform = frame.Unsafe.GetPointer<Transform2D>(listPlayerEntityRef[i]);
                    var randomIndex = frame.RNG->Next(0, 8);

                    while(playerTransform->Position == filter.SpawnPosition->WaitingPosition[i].Position 
                        && !filter.SpawnPosition->InGamePosition[randomIndex].isSpawned)
                    {
                        playerTransform->Position = filter.SpawnPosition->InGamePosition[randomIndex].Position;
                        filter.SpawnPosition->InGamePosition[randomIndex].isSpawned = true;
                    }
                }
                return;
            }
            if (gameSession->GameState == GameState.GameEnding)
            {
                for (var i = 0; i < listPlayerEntityRef.Count; i++)
                {
                    var playerTransform = frame.Unsafe.GetPointer<Transform2D>(listPlayerEntityRef[i]);

                    playerTransform->Position = filter.SpawnPosition->WaitingPosition[i].Position;
                    filter.SpawnPosition->InGamePosition[i].isSpawned = false;
                }
                return;
            }
        }

        private void ManageLoadPosition(Frame frame, Filter filter)
        {
            var playerSpawnPosition = frame.FindAsset(filter.SpawnPosition->PlayerSpawnPositions);
            waitingPosition = filter.SpawnPosition->WaitingPosition;
            LoadPositionToComponent(filter.SpawnPosition->WaitingPosition, playerSpawnPosition.waitingPositions);
            LoadPositionToComponent(filter.SpawnPosition->InGamePosition, playerSpawnPosition.inGamePositions);
        }

        private void LoadPositionToComponent(FixedArray<Positions> componentPositions, List<FPVector2> assetPositions)
        {
            if (assetPositions.Count > 0)
            {
                for (var i = 0; i < componentPositions.Length; i++)
                {
                    componentPositions.GetPointer(i)->Position = assetPositions[i];
                }
            }
        }

        public void OnPlayerAdded(Frame frame, PlayerRef player, bool firstTime)
        {
            var playerData = frame.GetPlayerData(player);
            var playerEntityRef = frame.Create(playerData.PlayerAvatar); //Create Player Entity
            var playerInfo = frame.Unsafe.GetPointer<PlayerInfo>(playerEntityRef);
            var playerTransform = frame.Unsafe.GetPointer<Transform2D>(playerEntityRef);

            //Random Skin Color
            if (!playerData.HaveRandomSkin)
            {
                playerData.SkinColor = frame.RNG->Next(0, 5);
                playerData.HaveRandomSkin = true;
            }

            playerData.CurrHealth = playerData.MaxHealth;
            playerInfo->PlayerRef = player;
            playerInfo->PlayerSkinColor = playerData.SkinColor;
            //Set Start Position
            playerTransform->Position = waitingPosition[playerInfo->PlayerRef].Position;

            //Add Player To List<EntityRef>
            frame.ResolveList(frame.Global->playerEntityRefList).Add(playerEntityRef);
        }

        public void OnPlayerRemoved(Frame frame, PlayerRef player)
        {
            var listPlayerInfo = frame.GetComponentIterator<PlayerInfo>();
            foreach (var playerInfo in listPlayerInfo)
            {
                if (playerInfo.Component.PlayerRef == player)
                {
                    frame.Destroy(playerInfo.Entity);
                }
            }
        }
    }
}
