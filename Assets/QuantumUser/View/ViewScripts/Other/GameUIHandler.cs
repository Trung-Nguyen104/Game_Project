using Photon.Client;
using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class GameUIHandler : QuantumMonoBehaviour, IOnEventCallback
{
    public UnityEngine.UI.Button startGameButton;
    public UnityEngine.UI.Button endGameButton;

    private RealtimeClient client;
    private Frame frame;
    private GameSession gameSession;

    private void Start()
    {
        frame = QuantumRunner.DefaultGame.Frames.Verified;
        client = QuantumRunner.Default?.NetworkClient;
        client.AddCallbackTarget(this);
        startGameButton.onClick.AddListener(() => StartGamePressed());
        endGameButton.onClick.AddListener(() => EndGamePressed());
        Debug.Log($"Client {client.State} {client.CurrentRoom.Name}");
    }

    private void Update()
    {
        frame.TryGetSingleton<GameSession>(out gameSession);

        StartGameButton();
        EndGameButton();
    }

    private void StartGameButton()
    {
        if (!client.LocalPlayer.IsMasterClient || gameSession.GameState != GameState.Waiting)
        {
            startGameButton.gameObject.SetActive(false);
        }
        else
        {
            startGameButton.gameObject.SetActive(true);
        }
    }

    private void EndGameButton()
    {
        if (!client.LocalPlayer.IsMasterClient || gameSession.GameState != GameState.GameStarted)
        {
            endGameButton.gameObject.SetActive(false);
        }
        else
        {
            endGameButton.gameObject.SetActive(true);
        }
    }

    private async void StartGamePressed()
    {
        client.OpRaiseEvent(0, true, new RaiseEventArgs {  Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        await WaitFor(StartingGame, 2);
        await WaitFor(StartedGame, 3);
        client.OpRaiseEvent(0, false, new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    private async void EndGamePressed()
    {
        client.OpRaiseEvent(1, true, new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        await WaitFor(EndingGame, 1);
        await WaitFor(BackToWaitingState, 3);
        client.OpRaiseEvent(1, false, new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    private void StartingGame() => QuantumRunner.DefaultGame.SendCommand(new ChangeGameStateCommads() { NewGameState = GameState.GameStarting.ToString() });

    private void StartedGame() => QuantumRunner.DefaultGame.SendCommand(new ChangeGameStateCommads() { NewGameState = GameState.GameStarted.ToString() });

    private void EndingGame() 
    {
        QuantumRunner.DefaultGame.SendCommand(new ChangeGameStateCommads() { NewGameState = GameState.GameEnding.ToString() }); 
        QuantumRunner.DefaultGame.SendCommand(new ChangeSeedCommads() { NewSeed = UnityEngine.Random.Range(1, 100) });
    }

    private void BackToWaitingState() => QuantumRunner.DefaultGame.SendCommand(new ChangeGameStateCommads() { NewGameState = GameState.Waiting.ToString() });

    private async Task WaitFor(Action callBack, FP timeDelay)
    {
        var targetTime = frame.Number + (int)(timeDelay / frame.DeltaTime);
        while (frame.Number < targetTime)
        {
            await Task.Yield();
        }
        callBack.Invoke();
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 0)
        {
            if ((bool)photonEvent.CustomData)
            {
                SetActivePanel("Loading......", (bool)photonEvent.CustomData);
            }
            else
            {
                SetActivePanel("", (bool)photonEvent.CustomData);
            }
        }
        if (photonEvent.Code == 1)
        {
            if ((bool)photonEvent.CustomData)
            {
                SetActivePanel("Game Over......", (bool)photonEvent.CustomData);
            }
            else
            {
                SetActivePanel("", (bool)photonEvent.CustomData);
            }
        }
    }

    private void SetActivePanel(string Context, bool enableValue)
    {
        PlayerUIController.Instance.LoadingGamePanel.SetPanelContext(Context);
        PlayerUIController.Instance.LoadingGamePanel.EnableGamePanel(enableValue);
    }
}
