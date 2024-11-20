namespace Quantum
{
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerShootingController : SystemMainThreadFilter<PlayerShootingController.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerInfo* PlayerInfo;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            var playerData = frame.GetPlayerData(filter.PlayerInfo->PlayerRef);
            var input = frame.GetPlayerInput(filter.PlayerInfo->PlayerRef);
            var currItem = filter.PlayerInfo->Inventory[filter.PlayerInfo->CurrSelectItem];

            if (input->InteracAction.WasPressed && frame.TryFindAsset(currItem.Item.ItemData, out var item))
            {
                if (item.itemType == ItemType.Weapon && currItem.GunAmmo > 0)
                {
                    filter.PlayerInfo->Inventory[filter.PlayerInfo->CurrSelectItem].GunAmmo -= 1;
                    SetUpBulletPrototype(frame, filter, playerData, currItem, item);
                }
            }
        }

        private static void SetUpBulletPrototype(Frame frame, Filter filter, RuntimePlayer playerLocal, ItemInfo currItem, ItemData item)
        {
            var bulletRef = frame.Create(currItem.BulletPrototype);
            var bulletTransform = frame.Get<Transform2D>(bulletRef);
            var bulletPhysis2D = frame.Get<PhysicsBody2D>(bulletRef);
            var bulletInfo = frame.Get<BulletInfo>(bulletRef);

            bulletPhysis2D.AddLinearImpulse(playerLocal.ShootPointDirection * 70);
            bulletTransform.Rotation = playerLocal.ShootPointRotation;
            bulletTransform.Position = playerLocal.ShootPointPosition;
            bulletInfo.OwnerPlayer = filter.PlayerInfo->PlayerRef;
            bulletInfo.Damage = item.damge;

            frame.Set(bulletRef, bulletTransform);
            frame.Set(bulletRef, bulletPhysis2D);
            frame.Set(bulletRef, bulletInfo);
        }
    }
}
