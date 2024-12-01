namespace Quantum
{
    using System.Collections.Generic;
    using UnityEngine;

    public unsafe class PlayerSkinView : QuantumEntityViewComponent
    {
        public List<RuntimeAnimatorController> animController;
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            var playerInfo = VerifiedFrame.Get<PlayerInfo>(_entityView.EntityRef);
            animator.runtimeAnimatorController = animController[playerInfo.PlayerSkinColor];
        }
    }
}
