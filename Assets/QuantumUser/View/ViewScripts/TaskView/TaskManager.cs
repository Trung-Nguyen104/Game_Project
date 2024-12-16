namespace Quantum
{
    using Photon.Client;
    using Photon.Realtime;
    using UnityEngine;

    public class TaskManager : QuantumMonoBehaviour
    {
        public static TaskManager Instance { get => instance; }
        public GameObject rememberIndexsTask;
        public GameObject wiresEnergyTask;
        public GameObject enterCodesTask;
        private static TaskManager instance;

        private void Start()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;
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
    }
}
