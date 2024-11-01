using Photon.Deterministic;
using UnityEngine.Scripting;

namespace Quantum
{
    [Preserve]
    public unsafe class PlayerController : SystemMainThreadFilter<PlayerController.Filter>
    {
        private Transform2D* playerTransform;
        private GameSession* gameSession;
        private Input* input;
        private RuntimePlayer playerData;
        private readonly int playerStartHealth = 30;
        private readonly float stopDistance = 0.1f;
        private readonly float maxDistance = 20f;

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerInfo* PlayerInfo;
            public MousePointerInfo* MousePointerInfo;
            public PhysicsBody2D* Physics2D;
            public Transform2D* Transform;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            input = frame.GetPlayerInput(filter.PlayerInfo->PlayerRef);
            playerData = frame.GetPlayerData(filter.PlayerInfo->PlayerRef);
            playerTransform = filter.Transform;
            gameSession = frame.Unsafe.GetPointerSingleton<GameSession>();

            SetUpPlayer(frame, filter);
            PlayerMovement(frame, filter);
            AimController(filter);
        }

        private void SetUpPlayer(Frame frame, Filter filter)
        {
            if (gameSession->GameState == GameState.GameStarting)
            {
                TeleportPlayerToPosition(0, 0);
            }
            if (gameSession->GameState == GameState.GameEnded)
            {
                filter.PlayerInfo->Health = playerStartHealth;
                TeleportPlayerToPosition(-71, 15);
            }
        }

        private void AimController(Filter filter)
        {
            var lookDirection = (input->mousePosition.XY - filter.Transform->Position);
            var angle = FPMath.Atan2(lookDirection.Y, lookDirection.X) * FP.Rad2Deg;

            filter.MousePointerInfo->AimAngle = angle;
        }

        private void PlayerMovement(Frame frame, Filter filter)
        {
            var gameSession = frame.GetSingleton<GameSession>();
            if (input->RightMouseClick.WasPressed)
            {
                filter.MousePointerInfo->targetPosition = input->mousePosition;
            }
            var distanceToTarget = FPVector2.Distance(filter.Transform->Position, filter.MousePointerInfo->targetPosition);
            if (distanceToTarget.AsFloat < stopDistance || distanceToTarget.AsFloat >= maxDistance)
            {
                filter.Physics2D->Velocity = FPVector2.Zero;
                frame.Events.IsMoving(filter.PlayerInfo->PlayerRef, false);
                return;
            }
            var direction = (filter.MousePointerInfo->targetPosition - filter.Transform->Position).Normalized;
            frame.Events.IsMoving(filter.PlayerInfo->PlayerRef, true);
            filter.Physics2D->Velocity = direction * FPMath.Min(filter.PlayerInfo->Speed, distanceToTarget / frame.DeltaTime);
        }

        private void TeleportPlayerToPosition(int x, int y)
        {
            playerTransform->Position = new FPVector2(x, y);
        }
    }
}
