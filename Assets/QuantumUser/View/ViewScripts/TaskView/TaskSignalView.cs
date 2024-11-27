namespace Quantum
{
    using Photon.Client;
    using Photon.Realtime;
    using UnityEngine;

    public class TaskSignalView : QuantumEntityViewComponent
    {
        private SpriteRenderer taskSignal;
        private SpriteRenderer miniMapIcon;
        private RealtimeClient client;
        private bool isTaskCompleted;

        private void Start()
        {
            QuantumEvent.Subscribe(listener: this, handler: (EventIsHighLight e) => HighLight(e));
            QuantumEvent.Subscribe(listener: this, handler: (EventInitiatingTask e) => InitiatingTask(e));
            client = QuantumRunner.Default.NetworkClient;

            taskSignal = GetComponentInChildren<SpriteRenderer>();
            miniMapIcon = transform.Find("MiniMapIcon").GetComponent<SpriteRenderer>();
            taskSignal.enabled = false;
        }

        private void InitiatingTask(EventInitiatingTask e)
        {
            if (QuantumRunner.DefaultGame.GetLocalPlayers()[0] != e.PlayerRef || _entityView.EntityRef != e.TaskRef)
            {
                return;
            }

            if (isTaskCompleted)
            {
                HandleTerroristDestroyBehaviour(e);
                return;
            }

            TaskManager.Instance.TaskTypeHandler(e);
        }

        private void HighLight(EventIsHighLight e)
        {
            if (QuantumRunner.DefaultGame.GetLocalPlayers()[0] != e.PlayerRef || _entityView.EntityRef != e.TaskRef)
            {
                return;
            }

            isTaskCompleted = VerifiedFrame.Get<TaskInfo>(e.TaskRef).IsTaskCompleted;

            if (!isTaskCompleted)
            {
                taskSignal.color = Color.red;
                taskSignal.enabled = e.IsEnter;
            }
            else
            {
                taskSignal.enabled = false;
                miniMapIcon.enabled = false;
            }

            HandleTerroristTaskSignal(e);
        }

        private void HandleTerroristDestroyBehaviour(EventInitiatingTask e)
        {
            var playerData = VerifiedFrame.GetPlayerData(e.PlayerRef);
            if (playerData.PlayerRole != PlayerRole.Terrorist)
            {
                return;
            }
            client.OpRaiseEvent((byte)TaskEventCode.TaskBeDestroy, e.TaskRef.ToString(), new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            Debug.Log($"{playerData.PlayerRole} destroy");
            playerData.SkillTimer = 20;
        }

        private void HandleTerroristTaskSignal(EventIsHighLight e)
        {
            var playerData = VerifiedFrame.GetPlayerData(e.PlayerRef);
            if (playerData.PlayerRole == PlayerRole.Terrorist)
            {
                if (isTaskCompleted)
                {
                    taskSignal.enabled = e.IsEnter;
                    IconSkillManager.Instance.SetIconInteractable(e.IsEnter);
                }
                else
                {
                    taskSignal.enabled = false;
                    IconSkillManager.Instance.SetIconInteractable(false);
                }
            }
        }
    }
}
