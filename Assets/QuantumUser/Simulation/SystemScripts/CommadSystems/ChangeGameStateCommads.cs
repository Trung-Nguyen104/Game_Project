namespace Quantum
{
    using Photon.Deterministic;
    using System;
    using UnityEngine;

    public unsafe class ChangeGameStateCommads : DeterministicCommand
    {
        public string NewGameState;

        public override void Serialize(Photon.Deterministic.BitStream stream)
        {
            stream.Serialize(ref NewGameState);
        }

        public void Execute(Frame frame)
        {
            frame.Unsafe.TryGetPointerSingleton<GameSession>(out var gameSession);
            if (Enum.TryParse(typeof(GameState), NewGameState, out var newGameState))
            {
                gameSession->GameState = (GameState)newGameState;
            }
        }
    }
}
