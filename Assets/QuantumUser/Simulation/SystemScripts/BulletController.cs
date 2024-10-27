namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class BulletController : SystemMainThreadFilter<BulletController.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public BulletInfo* BulletInfo;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            filter.BulletInfo->BulletTimeOut -= frame.DeltaTime;
            if(filter.BulletInfo->BulletTimeOut < 0)
            {
                frame.Destroy(filter.Entity);
            }
        }
    }
}
