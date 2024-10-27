using Photon.Deterministic;

namespace Quantum
{
    public partial class RuntimePlayer
    {
        public int SkinColor = 0;
        public bool HaveRandomSkin { get; set; } = false;
        public FPVector2 ShootPointPosition {  get; set; }
        public FPVector2 ShootPointDirection { get; set; }
        public FP ShootPointRotation { get; set; }
    }
}