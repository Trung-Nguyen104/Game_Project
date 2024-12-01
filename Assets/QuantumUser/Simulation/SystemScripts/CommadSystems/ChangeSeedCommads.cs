namespace Quantum
{
    using Photon.Deterministic;
    using System;
    using UnityEngine;

    public unsafe class ChangeSeedCommads : DeterministicCommand
    {
        public int NewSeed;

        public override void Serialize(Photon.Deterministic.BitStream stream)
        {
            stream.Serialize(ref NewSeed);
        }

        public void Execute(Frame frame)
        {
            frame.RuntimeConfig.Seed = NewSeed;
            Debug.Log(frame.RuntimeConfig.Seed);
        }
    }
}
