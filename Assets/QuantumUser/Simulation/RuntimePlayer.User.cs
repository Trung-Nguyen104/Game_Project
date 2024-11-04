using Photon.Deterministic;

namespace Quantum
{
    public partial class RuntimePlayer
    {
        public int CurrHealth = 0;
        public int SkinColor = 0;
        public PlayerRole PlayerRole = PlayerRole.None;
        public int MaxHealth { get; set; } = 100;
        public bool HaveRandomSkin { get; set; } = false;
        public FPVector2 ShootPointPosition {  get; set; }
        public FPVector2 ShootPointDirection { get; set; }
        public FP ShootPointRotation { get; set; }
        public FP PlayerSeed { get; private set; } = 6;
    }
}