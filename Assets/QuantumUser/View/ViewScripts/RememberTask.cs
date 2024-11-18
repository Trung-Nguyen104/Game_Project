using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RememberTask : MonoBehaviour
{
    public TMP_Text taskStatus;
    public Image[] leftButtons;
    public Image[] lights;
    public Button[] rightButtons;
    public float highlightDuration = 1f; 
    public int totalRounds = 6; 
    private List<int> sequence = new();
    private string originalTaskText;
    private int currentRound = 0; 
    private int playerProgress = 0;

    void Start()
    {
        originalTaskText = taskStatus.text;
        taskStatus.text = "Ready....";
        ResetTask();
        Invoke(nameof(StartNewRound), 2.5f);
    }

    private void StartNewRound()
    {
        ResetSequenceAndProgress();
        taskStatus.text = originalTaskText;
        currentRound++;

        if (currentRound > totalRounds)
        {
            CompletedTask();
            return;
        }

        lights[currentRound-1].color = Color.green;

        for (int i = 0; i < currentRound; i++)
        {
            int randomIndex = Random.Range(0, leftButtons.Length);
            sequence.Add(randomIndex);
        }

        StartCoroutine(ShowSequence());
    }

    private void CompletedTask()
    {
        taskStatus.text = "Task Completed !!";
        ResetTask();
    }

    private void ResetTask()
    {
        ResetSequenceAndProgress();
        currentRound = 0;
        foreach (var light in lights)
        {
            light.color = Color.white;
        }
    }

    private void ResetThisRound()
    {
        ResetSequenceAndProgress();
        taskStatus.text = originalTaskText;
        for (int i = 0; i < currentRound; i++)
        {
            int randomIndex = Random.Range(0, leftButtons.Length);
            sequence.Add(randomIndex);
        }

        StartCoroutine(ShowSequence());
    }

    private IEnumerator ShowSequence()
    {
        foreach (int index in sequence)
        {
            HighlightButton(leftButtons[index]);
            yield return new WaitForSeconds(highlightDuration);
        }

        EnablePlayerInput(true);
    }

    private void HighlightButton(Image img)
    {
        Color originalColor = img.color;
        img.color = Color.yellow;
        StartCoroutine(ResetButtonColor(img, originalColor));
    }

    private IEnumerator ResetButtonColor(Image img, Color originalColor)
    {
        yield return new WaitForSeconds(highlightDuration);
        img.color = originalColor; 
    }

    private void EnablePlayerInput(bool enable)
    {
        foreach (Button btn in rightButtons)
        {
            btn.interactable = enable;
        }
    }

    public void OnRightButtonClick(int buttonIndex)
    {
        if (sequence[playerProgress] == buttonIndex)
        {
            playerProgress++;
            if (playerProgress >= sequence.Count)
            {
                EnablePlayerInput(false);
                taskStatus.text = "GOOD JOB...";
                Invoke(nameof(StartNewRound), 2f);
            }
        }
        else
        {
            EnablePlayerInput(false);
            taskStatus.text = "ERROR..!!";
            Invoke(nameof(ResetThisRound), 2f);
        }
    }

    private void ResetSequenceAndProgress()
    {
        sequence.Clear();
        playerProgress = 0;
    }
}
