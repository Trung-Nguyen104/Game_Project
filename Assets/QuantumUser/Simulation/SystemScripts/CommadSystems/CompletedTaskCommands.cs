using Photon.Deterministic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quantum
{
    public unsafe class CompletedTaskCommands : DeterministicCommand
    {
        public EntityRef taskRef;
        public bool taskCompleted;

        public override void Serialize(Photon.Deterministic.BitStream stream)
        {
            stream.Serialize(ref taskRef);
            stream.Serialize(ref taskCompleted);
        }

        public void Execute(Frame frame)
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
