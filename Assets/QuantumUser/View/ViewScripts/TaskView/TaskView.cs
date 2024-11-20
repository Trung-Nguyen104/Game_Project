namespace Quantum
{
    using Photon.Client;
    using Photon.Realtime;
    using UnityEngine;

    enum TaskCompletedEventCode : byte
    {
        TaskCompleted = 6,
    }

    public class TaskView : QuantumEntityViewComponent, IOnEventCallback
    {
        public GameObject rememberIndexsTask;
        public GameObject wiresEnergyTask;
        public GameObject enterCodesTask;
        private RealtimeClient client;
        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            QuantumEvent.Subscribe(listener: this, handler: (EventIsHighLight e) => HighLight(e));
            QuantumEvent.Subscribe(listener: this, handler: (EventInitiatingTask e) => InitiatingTask(e));
            client = QuantumRunner.Default.NetworkClient;
            client.AddCallbackTarget(this);
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.enabled = false;
        }

        private void InitiatingTask(EventInitiatingTask e)
        {
            if (QuantumRunner.DefaultGame.GetLocalPlayers()[0] != e.PlayerRef || _entityView.EntityRef != e.TaskRef)
            {
                return;
            }
            switch (e.TaskType)
            {
                case TaskType.WiresEnergy:
                    wiresEnergyTask.GetComponent<WiresEnergyTask>().TaskRef = e.TaskRef;
                    wiresEnergyTask.SetActive(true);
                    break;
                case TaskType.EnterCodes:
                    enterCodesTask.GetComponent<EnterCodesTask>().TaskRef = e.TaskRef;
                    enterCodesTask.SetActive(true);
                    break;
                case TaskType.RememberIndexs:
                    rememberIndexsTask.GetComponent<RememberIndexsTask>().TaskRef = e.TaskRef;
                    rememberIndexsTask.SetActive(true);
                    break;
            }
        }

        private void HighLight(EventIsHighLight e)
        {
            if (QuantumRunner.DefaultGame.GetLocalPlayers()[0] != e.PlayerRef || _entityView.EntityRef != e.TaskRef)
            {
                return;
            }
            spriteRenderer.enabled = e.IsEnter;
        }

        public void OnEvent(EventData photonEvent)
        {
            if(photonEvent.Code == (byte)TaskCompletedEventCode.TaskCompleted)
            {
                GetTaskRef(photonEvent);
            }
        }

        private unsafe void GetTaskRef(EventData photonEvent)
        {
            if (EntityRef.TryParse((string)photonEvent.CustomData, out var taskRef))
            {
                var taskInfo = VerifiedFrame.Unsafe.GetPointer<TaskInfo>(taskRef);
                taskInfo->IsTaskCompleted = true;
                spriteRenderer.enabled = !taskInfo->IsTaskCompleted;
                Debug.Log(taskInfo->TaskType + " isCompleted = " + taskInfo->IsTaskCompleted);
            }
        }
    }
}
