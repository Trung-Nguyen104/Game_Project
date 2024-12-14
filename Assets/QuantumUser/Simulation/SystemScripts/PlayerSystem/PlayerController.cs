using Photon.Deterministic;
using UnityEngine.Scripting;

namespace Quantum
{
    [Preserve]
    public unsafe class PlayerController : SystemMainThreadFilter<PlayerController.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerInfo* PlayerInfo;
            public MousePointerInfo* MousePointerInfo;
            public PhysicsBody2D* Physics2D;
            public Transform2D* Transform;
            public PhysicsCollider2D* Collider;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            var input = frame.GetPlayerInput(filter.PlayerInfo->PlayerRef);
            var playerData = frame.GetPlayerData(filter.PlayerInfo->PlayerRef);
            var gameSession = frame.Unsafe.GetPointerSingleton<GameSession>();
            var playerTransform = filter.Transform;

            SetUpPlayerStatus(frame, filter, gameSession, playerData);
            if (gameSession->GameState == GameState.GameStarting || gameSession->GameState == GameState.GameEnding)
            {
                filter.MousePointerInfo->AimAngle = 0;
                return;
            }

            HandlePlayerDeath(frame, filter, playerData);
            PlayerMovement(frame, filter, playerData, input);
            AimController(filter, input);
        }

        private static void HandlePlayerDeath(Frame frame, Filter filter, RuntimePlayer playerData)
        {
            if (playerData.CurrHealth <= 0)
            {
                filter.Collider->Enabled = false;
                frame.Events.IsPlayerDeaded(filter.PlayerInfo->PlayerRef, true);
                return;
            }
        }

        private void SetUpPlayerStatus(Frame frame, Filter filter, GameSession* gameSession, RuntimePlayer playerData)
        {
            if (gameSession->GameState == GameState.GameEnding)
            {
                playerData.CurrHealth = playerData.MaxHealth;
                playerData.IsMonsterKill = false;
                playerData.IsSoldierDeaded = false;
                filter.Collider->Enabled = true;
                frame.Events.IsPlayerDeaded(filter.PlayerInfo->PlayerRef, false);
            }
        }

        private void AimController(Filter filter, Input* input)
        {
            var lookDirection = (input->MousePosition.XY - filter.Transform->Position);
            var angle = FPMath.Atan2(lookDirection.Y, lookDirection.X) * FP.Rad2Deg;

            filter.MousePointerInfo->AimAngle = angle;
        }

        private void PlayerMovement(Frame frame, Filter filter, RuntimePlayer playerData, Input* input)
        {
            filter.Physics2D->Velocity = input->Direction * playerData.PlayerSpeed;
            if (filter.Physics2D->Velocity.Magnitude > 0)
            {
                frame.Events.IsMoving(filter.PlayerInfo->PlayerRef, true);
            }
            else
            {
                frame.Events.IsMoving(filter.PlayerInfo->PlayerRef, false);
            }
        }

        private void TeleportPlayerToPosition(int x, int y, Transform2D* playerTransform)
        {
            playerTransform->Position = new FPVector2(x, y);
        }
    }
}
