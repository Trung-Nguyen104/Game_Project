using Photon.Client;
using Photon.Realtime;
using Quantum;
using TMPro;
using UnityEngine;

public class EnterCodesTask : MonoBehaviour
{
    public EntityRef TaskRef { get; set; }
    public TMP_Text randomNumberText;
    public TMP_Text inputNumberText;
    public UnityEngine.UI.Button[] numberButtons;
    public UnityEngine.UI.Button clearButton;
    public UnityEngine.UI.Button submitButton;
    public UnityEngine.UI.Button closeTaskButton;
    public int numberLength = 8;

    private string targetNumber;
    private string currentInput;

    private void Start()
    {
        closeTaskButton.onClick.AddListener(() => CloseTask());
        SetupButtons();
        ResetTask();
    }

    private void GenerateTargetNumber()
    {
        targetNumber = "";
        for (int i = 0; i < numberLength; i++)
        {
            targetNumber += Random.Range(0, 10).ToString();
        }
        randomNumberText.text = targetNumber;
    }

    private void SetupButtons()
    {
        foreach (UnityEngine.UI.Button btn in numberButtons)
        {
            string number = btn.GetComponentInChildren<TMP_Text>().text;
            btn.onClick.AddListener(() => AppendNumber(number));
        }
        clearButton.onClick.AddListener(ClearInput);
        submitButton.onClick.AddListener(CheckInput);
    }

    private void AppendNumber(string number)
    {
        if (currentInput.Length < numberLength)
        {
            currentInput += number;
            inputNumberText.text = currentInput;
        }
    }

    private void ClearInput()
    {
        currentInput = "";
        inputNumberText.fontSize = 100;
        inputNumberText.text = currentInput;
    }

    private void CheckInput()
    {
        if (currentInput == targetNumber)
        {
            inputNumberText.fontSize = 70;
            inputNumberText.text = "Task Completed";
            CompleteTask();
        }
        else
        {
            inputNumberText.fontSize = 80;
            inputNumberText.text = "!! Error !!";
            Invoke(nameof(ClearInput), 0.5f);
        }
    }

    private void ResetTask()
    {
        ClearInput();
        GenerateTargetNumber();
    }

    private void CompleteTask()
    {
        var client = QuantumRunner.Default.NetworkClient;
        client.OpRaiseEvent((byte)TaskEventCode.TaskCompleted, TaskRef.ToString(), new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        CloseTask();
    }

    private void CloseTask()
    {
        ResetTask();
        gameObject.SetActive(false);
    }
}
