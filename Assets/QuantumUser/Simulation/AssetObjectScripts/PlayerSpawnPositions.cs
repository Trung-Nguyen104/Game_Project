using Photon.Deterministic;
using System.Collections.Generic;

namespace Quantum
{
    public class PlayerSpawnPositions : AssetObject
    {
        public List<FPVector2> waitingPositions;
        public List<FPVector2> inGamePositions;
    }
}
