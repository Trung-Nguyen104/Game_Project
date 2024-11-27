namespace Quantum
{
    using Photon.Client;
    using Photon.Realtime;
    using UnityEngine;

    enum TaskEventCode : byte
    {
        TaskCompleted = 6,
        TaskBeDestroy = 7,
    }

    public class TaskManager : QuantumMonoBehaviour, IOnEventCallback
    {
        public static TaskManager Instance { get => instance; }
        public GameObject rememberIndexsTask;
        public GameObject wiresEnergyTask;
        public GameObject enterCodesTask;
        private RealtimeClient client;
        private Frame frame;
        private static TaskManager instance;

        private void Start()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;
            frame = QuantumRunner.DefaultGame.Frames.Verified;
            client = QuantumRunner.Default.NetworkClient;
            client.AddCallbackTarget(this);
        }

        public void TaskTypeHandler(EventInitiatingTask e)
        {
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

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (byte)TaskEventCode.TaskCompleted:
                    GetTaskRef(photonEvent, true);
                    break;
                case (byte)TaskEventCode.TaskBeDestroy:
                    GetTaskRef(photonEvent, false);
                    break;
            }
        }

        private unsafe void GetTaskRef(EventData photonEvent, bool taskCompleted)
        {
            if (EntityRef.TryParse((string)photonEvent.CustomData, out var taskRef))
            {
                if (!frame.Unsafe.TryGetPointer<TaskInfo>(taskRef, out var taskInfo))
                {
                    Debug.Log("TaskInfo Null");
                }
                taskInfo->IsTaskCompleted = taskCompleted;
                Debug.Log(taskInfo->TaskType + " isCompleted = " + taskInfo->IsTaskCompleted);
            }
        }
    }
}
