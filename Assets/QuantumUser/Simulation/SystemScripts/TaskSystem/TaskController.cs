namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class TaskController : SystemMainThreadFilter<TaskController.Filter>
    {
        private GameSession* gameSession;

        public struct Filter
        {
            public EntityRef Entity;
            public PhysicsCollider2D* Collider2D;
            public TaskInfo* TaskInfo;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            gameSession = frame.Unsafe.GetPointerSingleton<GameSession>();
            var isTaskCompleted = filter.TaskInfo->IsTaskCompleted;
            if (gameSession->GameState == GameState.Waiting || gameSession->GameState == GameState.GameEnded)
            {
                isTaskCompleted = false;
            }
            filter.Collider2D->Enabled = !isTaskCompleted;
        }
    }
}
