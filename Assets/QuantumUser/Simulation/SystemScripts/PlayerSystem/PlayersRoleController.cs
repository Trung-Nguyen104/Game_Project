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

        public override void OnInit(Frame frame)
        {
            base.OnInit(frame);
            listPlayerEntityRef = frame.ResolveList(frame.Global->playerEntityRef);
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            gameSession = frame.Unsafe.GetPointerSingleton<GameSession>();
            if (gameSession->GameState == GameState.Waiting)
            {
                ResetPlayersRole(frame, filter);
                return;
            }
            HandleGameStart(frame, filter);
        }

        private void HandleGameStart(Frame frame, Filter filter)
        {
            for (int i = 0; i < listPlayerEntityRef.Count; i++)
            {
                playerInfo = frame.Unsafe.GetPointer<PlayerInfo>(listPlayerEntityRef[i]);
                var playerData = frame.GetPlayerData(playerInfo->PlayerRef);
                if (gameSession->GameState == GameState.GameStarted)
                {
                    SetUpPlayersRole(frame, filter, playerData);
                    if (i == listPlayerEntityRef.Count - 1)
                    {
                        allPlayerHaveRole = true;
                        resetAllRole = false;
                    }
                }
            }
        }

        private void SetUpPlayersRole(Frame frame, Filter filter, RuntimePlayer playerData)
        {
            if (allPlayerHaveRole)
            {
                return;
            }
            while (playerData.PlayerRole == PlayerRole.None)
            {
                var roleProfiles = filter.RoleManager->RoleProfiles;
                var randomRole = frame.RNG->Next(0, roleProfiles.Length);
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
