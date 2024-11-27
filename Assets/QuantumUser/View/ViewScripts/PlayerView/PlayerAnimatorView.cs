namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine;

    public unsafe class PlayerAnimatorView : QuantumEntityViewComponent
    {
        private Animator animator;
        private RuntimePlayer playerData;
        private PlayerInfo playerInfo;
        private Transform2D transform2D;
        private Vector3 animatorOriginalPosition;
        private Quaternion animatorOriginalRotation;
        private bool FacingLeft { get; set; } = true;

        private void Start()
        {
            QuantumEvent.Subscribe(listener: this, handler: (EventIsMoving e) => IsMoving(e));
            QuantumEvent.Subscribe(listener: this, handler: (EventIsPlayerDeaded e) => IsPlayerDeaded(e));
            animator = GetComponent<Animator>();
            animatorOriginalPosition = animator.transform.localPosition;
            animatorOriginalRotation = animator.transform.localRotation;
        }

        private void Update()
        {
            playerInfo = VerifiedFrame.Get<PlayerInfo>(_entityView.EntityRef);
            playerData = VerifiedFrame.GetPlayerData(playerInfo.PlayerRef);
            transform2D = VerifiedFrame.Get<Transform2D>(_entityView.EntityRef);
            RotationController();
            Debug.Log("FacingLeft = " + FacingLeft);
        }

        private void RotationController()
        {
            if (playerInfo.Inventory[playerInfo.CurrSelectItem].Item.ItemPrototype != null)
            {
                var itemType = VerifiedFrame.FindAsset(playerInfo.Inventory[playerInfo.CurrSelectItem].Item.ItemData).itemType;
                if (itemType == ItemType.Weapon)
                {
                    var mouseXPosition = VerifiedFrame.GetPlayerInput(playerInfo.PlayerRef)->MousePosition.X;
                    RotationWithMousePosition(mouseXPosition);
                }
                return;
            }
            var physicsBody2D = VerifiedFrame.Get<PhysicsBody2D>(_entityView.EntityRef);
            RotationWithVelocity(physicsBody2D.Velocity.X);
        }

        private void IsPlayerDeaded(EventIsPlayerDeaded e)
        {
            if (e.PlayerRef != playerInfo.PlayerRef)
            {
                return;
            }
            if (e.IsPlayerDead)
            {
                if (playerData.IsMonsterKill)
                {
                    animator.SetBool("isMonsterKilled", playerData.IsMonsterKill);
                }
                else
                {
                    animator.SetBool("isGunKilled", !playerData.IsMonsterKill);
                }
            }
            else
            {
                animator.SetBool("isMonsterKilled", false);
                animator.SetBool("isGunKilled", false);
                FacingLeft = true;
                animator.transform.SetLocalPositionAndRotation(animatorOriginalPosition, animatorOriginalRotation);
            }
        }

        public void IsMoving(EventIsMoving e)
        {
            if (e.PlayerRef == playerInfo.PlayerRef)
            {
                animator.SetBool("isMoving", e.isMoving);
            }
        }

        private void Flip()
        {
            FacingLeft = !FacingLeft;
            animator.transform.Rotate(0, 180, 0);
        }

        private void RotationWithMousePosition(FP mousePosition)
        {
            if (mousePosition < transform2D.Position.X && !FacingLeft || mousePosition > transform2D.Position.X && FacingLeft)
            {
                Flip();
            }
        }

        private void RotationWithVelocity(FP xVelocity)
        {
            if (xVelocity < 0 && !FacingLeft || xVelocity > 0 && FacingLeft)
            {
                Flip();
            }
        }
    }
}
