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
            if (filter.PlayerInfo->CurrSelectItem < 0)
            {
                return;
            }
            var currItem = filter.PlayerInfo->Inventory[filter.PlayerInfo->CurrSelectItem];

            if (!frame.TryFindAsset(currItem.Item.ItemData, out var item))
            {
                return;
            }
            if (item.itemType != ItemType.Weapon /*|| currItem.CurrentAmmo <= 0*/)
            {
                return;
            }
            if (input->UseItemOrShoot.WasPressed)
            {
                Debug.Log($"{filter.PlayerInfo->PlayerRef} Shooting");
                filter.PlayerInfo->Inventory[filter.PlayerInfo->CurrSelectItem].CurrentAmmo -= 1;
                SetUpBulletPrototype(frame, filter, input, currItem, item);
            }
        }

        private void SetUpBulletPrototype(Frame frame, Filter filter, Input* input, ItemInfo currItem, ItemData item)
        {
            var bulletRef = frame.Create(currItem.BulletPrototype);
            var bulletTransform = frame.Unsafe.GetPointer<Transform2D>(bulletRef);
            var bulletPhysis2D = frame.Unsafe.GetPointer<PhysicsBody2D>(bulletRef);
            var bulletInfo = frame.Unsafe.GetPointer<BulletInfo>(bulletRef);

            bulletInfo->IsFirstTouching = false;
            bulletInfo->Damage = item.damge;
            bulletInfo->OwnerPlayer = filter.PlayerInfo->PlayerRef;
            bulletTransform->Rotation = input->ShootPointRotation;
            bulletTransform->Position = input->ShootPointPosition;
            bulletPhysis2D->AddLinearImpulse(input->ShootPointDirection * 20);
        }
    }
}
