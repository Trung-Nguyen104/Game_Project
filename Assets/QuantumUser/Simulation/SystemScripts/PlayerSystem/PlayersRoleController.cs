namespace Quantum
{
    using Quantum.Collections;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayersRoleController : SystemMainThreadFilter<PlayersRoleController.Filter>
    {
        private QList<EntityRef> listPlayerEntityRef;
        private PlayerInfo* playerInfo;
        private Transform2D* playerTransform;
        private GameSession* gameSession;
        private bool allPlayerHaveRole = false;
        private bool resetAllRole = false;

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerRoleManager* RoleManager;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            gameSession = frame.Unsafe.GetPointerSingleton<GameSession>();
            if (gameSession->GameState == GameState.Waiting)
            {
                return;
            }
            if (gameSession->GameState == GameState.GameEnding)
            {
                ResetPlayersRole(frame, filter);
                return;
            }
            SetUpPlayersRole(frame, filter);
        }

        private void SetUpPlayersRole(Frame frame, Filter filter)
        {
            if (allPlayerHaveRole)
            {
                return;
            }
            listPlayerEntityRef = frame.ResolveList(frame.Global->playerEntityRefList);
            for (int i = 0; i < listPlayerEntityRef.Count; i++)
            {
                playerInfo = frame.Unsafe.GetPointer<PlayerInfo>(listPlayerEntityRef[i]);
                var playerData = frame.GetPlayerData(playerInfo->PlayerRef);
                HandleRandomRole(frame, filter, playerData);
            }
            allPlayerHaveRole = true;
            resetAllRole = false;
        }

        private void HandleRandomRole(Frame frame, Filter filter, RuntimePlayer playerData)
        {
            while (playerData.PlayerRole == PlayerRole.None)
            {
                var roleProfiles = filter.RoleManager->RoleProfiles;
                var randomRole = frame.Global->RngSession.Next(0, roleProfiles.Length);
                if (roleProfiles[randomRole].RoleQuantity > 0 && roleProfiles[randomRole].PlayerRole != PlayerRole.None)
                {
                    playerData.PlayerRole = roleProfiles[randomRole].PlayerRole;
                    Debug.Log($"Role {playerInfo->PlayerRef} = {playerData.PlayerRole}");
                    roleProfiles[randomRole].RoleQuantity--;
                }
            }
        }

        private void ResetPlayersRole(Frame frame, Filter filter)
        {
            if (resetAllRole)
            {
                return;
            }
            listPlayerEntityRef = frame.ResolveList(frame.Global->playerEntityRefList);
            for (int i = 0; i < listPlayerEntityRef.Count; i++)
            {
                playerInfo = frame.Unsafe.GetPointer<PlayerInfo>(listPlayerEntityRef[i]);
                var playerData = frame.GetPlayerData(playerInfo->PlayerRef);
                playerData.PlayerRole = PlayerRole.None;
            }
            var roleProfiles = filter.RoleManager->RoleProfiles;
            for (int i = 0; i < roleProfiles.Length; i++)
            {
                roleProfiles[i].RoleQuantity = 1;
            }
            allPlayerHaveRole = false;
            resetAllRole = true;
        }
    }
}
