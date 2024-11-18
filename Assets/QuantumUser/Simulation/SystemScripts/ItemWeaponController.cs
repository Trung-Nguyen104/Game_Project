namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class ItemWeaponController : SystemMainThreadFilter<ItemWeaponController.Filter>
    {
        public override void Update(Frame f, ref Filter filter)
        {
        }

        public struct Filter
        {
            public EntityRef Entity;
        }
    }
}
