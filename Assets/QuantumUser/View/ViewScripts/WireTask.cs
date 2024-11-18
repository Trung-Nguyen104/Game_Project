using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WireTask : MonoBehaviour
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
            Debug.Log("Task already completed.");
            return;
        }

        if (leftWireMapping[leftWire] == rightWireMapping[rightWire])
        {
            Debug.Log("Correct connection!");
            LightOf(rightWire).color = Color.yellow;
            LightOf(leftWire).color = Color.yellow;
            CheckTaskCompletion();
        }
        else
        {
            Debug.Log("Incorrect connection.");
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
        Debug.Log("Task Completed!");
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
