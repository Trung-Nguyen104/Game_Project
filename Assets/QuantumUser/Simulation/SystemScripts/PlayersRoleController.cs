namespace Quantum
{
    using Quantum.Collections;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayersRoleController : SystemMainThreadFilter<PlayersRoleController.Filter>
    {
        private QList<EntityRef> listPlayerEntityRef;
        private bool allPlayerHaveRole = false;
        private PlayerInfo* playerInfo;
        private Transform2D* playerTransform;
        private GameSession* gameSession;

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
            HandleGameState(frame, filter);
        }

        private void HandleGameState(Frame frame, Filter filter)
        {
            if (gameSession->GameState == GameState.Waiting)
            {
                return;
            }
            for (int i = 0; i < listPlayerEntityRef.Count; i++)
            {
                playerInfo = frame.Unsafe.GetPointer<PlayerInfo>(listPlayerEntityRef[i]);

                if(gameSession->GameState == GameState.GameEnded)
                {
                    ResetPlayersRole(frame, filter);
                }
                if (allPlayerHaveRole)
                {
                    return;
                }
                if (gameSession->GameState == GameState.GameStarted)
                {
                    SetUpPlayersRole(frame, filter);
                    if (i == listPlayerEntityRef.Count - 1)
                    {
                        allPlayerHaveRole = true;
                    }
                }
            }
        }

        private void ResetPlayersRole(Frame frame, Filter filter)
        {
            playerInfo->PlayerRole = PlayerRole.None;
            allPlayerHaveRole = false;
        }

        private void SetUpPlayersRole(Frame frame, Filter filter)
        {
            while (playerInfo->PlayerRole == PlayerRole.None)
            {
                var roleProfiles = filter.RoleManager->RoleProfiles;
                var randomRole = frame.RNG->Next(0, roleProfiles.Length);
                if (roleProfiles[randomRole].RoleQuantity > 0 && roleProfiles[randomRole].PlayerRole != PlayerRole.None)
                {
                    playerInfo->PlayerRole = roleProfiles[randomRole].PlayerRole;
                    Debug.Log($"Role {playerInfo->PlayerRef} = {playerInfo->PlayerRole}");
                    roleProfiles[randomRole].RoleQuantity--;
                }
            }
        }
    }
}
