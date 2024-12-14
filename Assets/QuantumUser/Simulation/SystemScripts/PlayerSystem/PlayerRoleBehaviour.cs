namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerRoleBehaviour : SystemMainThreadFilter<PlayerRoleBehaviour.Filter>, ISignalOnMonsterKill, ISignalOnDoctorInject
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PhysicsCollider2D* Collider;
            public PlayerInfo* PlayerInfo;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            var playerData = frame.GetPlayerData(filter.PlayerInfo->PlayerRef);
            var gameSession = frame.GetSingleton<GameSession>();

            ResetTimerCooldown(gameSession, playerData);
            SendEventSkillisCoolingDown(frame, filter, playerData);
            HandleImmunityStatus(frame, playerData);
            HandlePlayerRoleBehaviour(frame, filter, playerData);
        }

        private void SendEventSkillisCoolingDown(Frame frame, Filter filter, RuntimePlayer playerData)
        {
            if (playerData.SkillTimer > 0)
            {
                frame.Events.IsCoolDown(filter.PlayerInfo->PlayerRef, true);
            }
            else
            {
                frame.Events.IsCoolDown(filter.PlayerInfo->PlayerRef, false);
            }
        }

        private void ResetTimerCooldown(GameSession gameSession, RuntimePlayer playerData)
        {
            if (gameSession.GameState != GameState.GameStarted)
            {
                playerData.SkillTimer = 0;
                playerData.CoolDownImmunity = 0;
            }
        }

        private void HandleImmunityStatus(Frame frame, RuntimePlayer playerData)
        {
            Cooldown(frame, ref playerData.CoolDownImmunity);
            if (playerData.IsImmunity)
            {
                if (playerData.CoolDownImmunity <= 0)
                {
                    playerData.IsImmunity = !playerData.IsImmunity;
                }
            }
        }

        private void HandlePlayerRoleBehaviour(Frame frame, Filter filter, RuntimePlayer playerData)
        {
            switch (playerData.PlayerRole)
            {
                case PlayerRole.Monster:
                    Cooldown(frame, ref playerData.SkillTimer);
                    break;
                case PlayerRole.Doctor:
                    Cooldown(frame, ref playerData.SkillTimer);
                    break;
                case PlayerRole.Terrorist:
                    Cooldown(frame, ref playerData.SkillTimer);
                    break;
                case PlayerRole.Detective:
                    Cooldown(frame, ref playerData.SkillTimer);
                    break;
                case PlayerRole.Soldier:
                    Cooldown(frame, ref playerData.SkillTimer);
                    SoldierBehaviour(frame, filter, playerData);
                    break;
            }
        }

        private FP Cooldown(Frame frame, ref FP timeCounter)
        {
            if (timeCounter > 0)
            {
                timeCounter -= frame.DeltaTime;
            }
            else
            {
                timeCounter = 0;
            }
            return timeCounter;
        }

        private void SoldierBehaviour(Frame frame, Filter filter, RuntimePlayer playerData)
        {
            if (playerData.CurrHealth > 0 || playerData.IsMonsterKill || playerData.IsSoldierDeaded)
            {
                return;
            }
           
            if (playerData.SkillTimer > 0)
            {
                playerData.IsSoldierDeaded = true;
                return;
            }

            playerData.SkillTimer = 18;
            filter.Collider->Enabled = true;
            playerData.CurrHealth = playerData.MaxHealth / 2;
            frame.Events.IsPlayerDeaded(filter.PlayerInfo->PlayerRef, false);
        }

        public void OnMonsterKill(Frame frame, EntityRef TargetRef)
        {
            var targetRef = frame.Get<PlayerInfo>(TargetRef).PlayerRef;
            var targetData = frame.GetPlayerData(targetRef);

            if (targetData.IsImmunity)
            {
                targetData.IsImmunity = !targetData.IsImmunity;
            }
            else
            {
                targetData.CurrHealth = 0;
                targetData.IsMonsterKill = true;
            }
        }

        public void OnDoctorInject(Frame frame, EntityRef TargetRef)
        {
            var targetRef = frame.Get<PlayerInfo>(TargetRef).PlayerRef;
            var targetData = frame.GetPlayerData(targetRef);

            targetData.IsImmunity = true;
            targetData.CoolDownImmunity = 30;
        }
    }
}
