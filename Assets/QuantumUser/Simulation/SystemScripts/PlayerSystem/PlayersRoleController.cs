namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayersRoleController : SystemMainThreadFilter<PlayersRoleController.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerRoleManager* RoleManager;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            var gameSession = frame.Unsafe.GetPointerSingleton<GameSession>();
            filter.RoleManager->RNGValue = new RNGSession(frame.RuntimeConfig.Seed);

            HandleGameSession(frame, filter, gameSession);
        }

        private void HandleGameSession(Frame frame, Filter filter, GameSession* gameSession)
        {
            var roleProfiles = filter.RoleManager->RoleProfiles;

            if (gameSession->GameState == GameState.Waiting)
            {
                return;
            }
            if (gameSession->GameState == GameState.GameEnding)
            {
                ResetPlayersRole(frame, filter);
            }
            if (gameSession->GameState == GameState.GameStarting)
            {
                SetUpPlayersRole(frame, filter);
            }
        }

        private void SetUpPlayersRole(Frame frame, Filter filter)
        {
            var listPlayerEntityRef = frame.ResolveList(frame.Global->playerEntityRefList);
            var roleProfiles = filter.RoleManager->RoleProfiles;
            for (int i = 0; i < listPlayerEntityRef.Count; i++)
            {
                var playerInfo = frame.Unsafe.GetPointer<PlayerInfo>(listPlayerEntityRef[i]);
                var playerData = frame.GetPlayerData(playerInfo->PlayerRef);
                HandleRandomRole(frame, filter, playerData, roleProfiles);
            }
        }

        private void HandleRandomRole(Frame frame, Filter filter, RuntimePlayer playerData, FixedArray<RoleProfile> roleProfiles)
        {
            while (playerData.PlayerRole == PlayerRole.None)
            {
                var randomRole = filter.RoleManager->RNGValue.Next(0, roleProfiles.Length);
                if (!roleProfiles[randomRole].IsDone)
                {
                    playerData.PlayerRole = roleProfiles[randomRole].PlayerRole;
                }
            }
            for (int i = 0; i < filter.RoleManager->RoleProfiles.Length; i++)
            {
                if (roleProfiles[i].PlayerRole == playerData.PlayerRole)
                {
                    roleProfiles[i].IsDone = true;
                }
            }
        }

        private void ResetPlayersRole(Frame frame, Filter filter)
        {
            var listPlayerEntityRef = frame.ResolveList(frame.Global->playerEntityRefList);
            for (int i = 0; i < listPlayerEntityRef.Count; i++)
            {
                var playerInfo = frame.Unsafe.GetPointer<PlayerInfo>(listPlayerEntityRef[i]);
                var playerData = frame.GetPlayerData(playerInfo->PlayerRef);
                playerData.PlayerRole = PlayerRole.None;
            }
            var roleProfiles = filter.RoleManager->RoleProfiles;
            for (int i = 0; i < roleProfiles.Length; i++)
            {
                roleProfiles[i].IsDone = false;
            }
        }
    }
}
