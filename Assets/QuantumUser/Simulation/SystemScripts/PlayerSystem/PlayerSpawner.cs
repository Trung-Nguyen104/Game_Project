namespace Quantum
{
    using Photon.Deterministic;
    using Quantum.Collections;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerSpawner : SystemSignalsOnly, ISignalOnPlayerAdded, ISignalOnPlayerRemoved
    {
        private QList<EntityRef> listPlayerEntityRef;

        public override void OnInit(Frame frame)
        {
            base.OnInit(frame);
            frame.Global->playerEntityRefList = frame.AllocateList<EntityRef>();
        }

        public void OnPlayerAdded(Frame frame, PlayerRef player, bool firstTime)
        {
            var playerData = frame.GetPlayerData(player);
            var playerEntityRef = frame.Create(playerData.PlayerAvatar);
            var playerInfo = frame.Unsafe.GetPointer<PlayerInfo>(playerEntityRef);

            if (!playerData.HaveRandomSkin)
            {
                playerData.SkinColor = frame.RNG->Next(0, 5);
                playerData.HaveRandomSkin = true;
            }

            playerData.CurrHealth = playerData.MaxHealth;
            playerInfo->PlayerSkinColor = playerData.SkinColor;
            playerInfo->PlayerRef = player;

            listPlayerEntityRef = frame.ResolveList(frame.Global->playerEntityRefList);
            listPlayerEntityRef.Add(playerEntityRef);

            var playerTransform = frame.Unsafe.GetPointer<Transform2D>(playerEntityRef);
            playerTransform->Position = new FPVector2(-63, -4);
        }

        public void OnPlayerRemoved(Frame frame, PlayerRef player)
        {
            var listPlayerInfo = frame.GetComponentIterator<PlayerInfo>();
            foreach (var playerInfo in listPlayerInfo)
            {
                if (playerInfo.Component.PlayerRef == player)
                {
                    frame.Destroy(playerInfo.Entity);
                }
            }
        }
    }
}
