namespace Quantum
{
    using Photon.Client;
    using Photon.Realtime;
    using UnityEngine;

    public class TaskSignalView : QuantumEntityViewComponent
    {
        private SpriteRenderer[] taskSignal;
        private SpriteRenderer miniMapIcon;
        private RealtimeClient client;
        private bool isTaskCompleted;

        private void Start()
        {
            QuantumEvent.Subscribe(listener: this, handler: (EventIsHighLight e) => HighLight(e));
            QuantumEvent.Subscribe(listener: this, handler: (EventInitiatingTask e) => InitiatingTask(e));
            client = QuantumRunner.Default.NetworkClient;

            taskSignal = transform.Find("TaskSignal").GetComponentsInChildren<SpriteRenderer>();
            miniMapIcon = transform.Find("MiniMapIcon").GetComponent<SpriteRenderer>();
            SetTaskSiganl(false, Color.white);
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
                SetTaskSiganl(e.IsEnter, Color.red);
            }
            else
            {
                SetTaskSiganl(false, Color.white);
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
                    SetTaskSiganl(e.IsEnter, Color.white);
                    IconSkillManager.Instance.SetIconInteractable(e.IsEnter);
                }
                else
                {
                    SetTaskSiganl(false, Color.white);
                    IconSkillManager.Instance.SetIconInteractable(false);
                }
            }
        }

        private void SetTaskSiganl(bool enableValue, Color color)
        {
            foreach (var spriteRender in taskSignal)
            {
                spriteRender.color = color;
                spriteRender.enabled = enableValue;
            }
        }
    }
}
