using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputNumberTask : MonoBehaviour
{
    public TMP_Text randomNumberText;
    public TMP_Text inputNumberText;
    public Button[] numberButtons;
    public Button clearButton;
    public Button submitButton;
    public int numberLength = 8;

    private string targetNumber;
    private string currentInput;

    private void Start()
    {
        GenerateTargetNumber();
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
        foreach (Button btn in numberButtons)
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
        currentInput = "";
        inputNumberText.fontSize = 100;
        inputNumberText.text = currentInput;
    }

    private void CompleteTask()
    {
        Debug.Log("Congratulations, you've completed the task!");
    }
}
