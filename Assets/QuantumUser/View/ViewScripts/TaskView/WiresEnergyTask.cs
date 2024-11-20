using Photon.Client;
using Photon.Realtime;
using Quantum;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WiresEnergyTask : MonoBehaviour
{
    [Header("Wire Settings")]
    public List<Color> wireColors;
    public List<GameObject> leftWires;
    public List<GameObject> rightWires;

    [Header("UI Elements")]
    public TMP_Text statusText;

    private Dictionary<GameObject, Color> leftWireMapping;
    private Dictionary<GameObject, Color> rightWireMapping;
    private bool isTaskCompleted = false;

    [Header("Connection Settings")]
    public float connectThreshold = 140f;

    public EntityRef TaskRef { get; set; }

    void Start()
    {
        ResetTask();
    }

    public void ResetTask()
    {
        statusText.text = "Task Incomplete";
        isTaskCompleted = false;

        List<Color> shuffledColors = new(wireColors);
        ShuffleList(shuffledColors);

        leftWireMapping = new Dictionary<GameObject, Color>();
        rightWireMapping = new Dictionary<GameObject, Color>();

        for (int i = 0; i < leftWires.Count; i++)
        {
            LightOf(leftWires[i]).color = Color.gray;
            leftWireMapping[leftWires[i]] = shuffledColors[i];
            WireStartOf(leftWires[i]).color = shuffledColors[i];
            WireEndOf(leftWires[i]).color = shuffledColors[i];
        }

        ShuffleList(shuffledColors);
        for (int i = 0; i < rightWires.Count; i++)
        {
            LightOf(rightWires[i]).color = Color.gray;
            rightWireMapping[rightWires[i]] = shuffledColors[i];
            WireStartOf(rightWires[i]).color = shuffledColors[i];
            WireEndOf(rightWires[i]).color = shuffledColors[i];
        }
    }

    public void ConnectWires(GameObject leftWire, GameObject rightWire)
    {
        if (isTaskCompleted)
        {
            return;
        }

        if (leftWireMapping[leftWire] == rightWireMapping[rightWire])
        {
            LightOf(rightWire).color = Color.yellow;
            LightOf(leftWire).color = Color.yellow;
            CheckTaskCompletion();
        }
    }

    private void CheckTaskCompletion()
    {
        foreach (var rightWire in rightWires)
        {
            if (LightOf(rightWire).color == Color.gray)
            {
                return;
            }
        }

        isTaskCompleted = true;
        statusText.text = "Task Completed: DONE!";
        gameObject.SetActive(false);
        var client = QuantumRunner.Default.NetworkClient;
        client.OpRaiseEvent((byte)TaskCompletedEventCode.TaskCompleted, TaskRef.ToString(), new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        ResetTask();
    }

    private Image LightOf(GameObject wireGameObject) => wireGameObject.transform.Find("Light").GetComponent<Image>();
    private Image WireStartOf(GameObject wireGameObject) => wireGameObject.transform.Find("WireStart").GetComponent<Image>();
    private Image WireEndOf(GameObject wireGameObject) => wireGameObject.transform.Find("WireEnd").GetComponent<Image>();

    private void ShuffleList(List<Color> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            Color temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
