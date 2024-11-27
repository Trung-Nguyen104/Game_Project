using Photon.Deterministic;

namespace Quantum
{
    public partial class RuntimePlayer
    {
        public int SkinColor = 0;
        public FP CurrHealth = 0;
        public FP SkillTimer = 0;
        public FP CoolDownImmunity = 0;
        public PlayerRole PlayerRole = PlayerRole.None;
        public bool IsImmunity = false;
        public bool IsMonsterKill = false;
        public int MaxHealth { get; set; } = 100;
        public bool HaveRandomSkin { get; set; } = false;
        public FP PlayerSeed { get; private set; } = 6;
    }
}