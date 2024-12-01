namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerCommadsSystems : SystemMainThread
    {
        public override void Update(Frame frame)
        {
            for (int i = 0; i < frame.PlayerCount; i++)
            {
                var changeGameState = frame.GetPlayerCommand(i) as ChangeGameStateCommads;
                var changeSeed = frame.GetPlayerCommand(i) as ChangeSeedCommads;
                changeGameState?.Execute(frame);
                changeSeed?.Execute(frame);
            }
        }
    }
}
