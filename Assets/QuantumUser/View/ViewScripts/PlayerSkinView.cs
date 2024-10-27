namespace Quantum
{
    using Photon.Realtime;
    using System.Collections.Generic;
    using UnityEditor.Animations;
    using UnityEngine;

    public unsafe class PlayerSkinView : QuantumEntityViewComponent
    {
        public List<AnimatorController> animController;
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }
        private void Update()
        {
            var playerInfo = VerifiedFrame.Get<PlayerInfo>(_entityView.EntityRef);
            var playerData = VerifiedFrame.GetPlayerData(playerInfo.PlayerRef);

            animator.runtimeAnimatorController = animController[playerInfo.PlayerSkinColor];
        }
    }
}
