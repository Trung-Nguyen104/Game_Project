using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameUIHandler : QuantumMonoBehaviour
{
    public UnityEngine.UI.Button startGameButton;
    public UnityEngine.UI.Button endGameButton;

    private RealtimeClient client;
    private Frame frame;

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
        frame.TryGetSingleton<GameSession>(out var gameSession);

        StartGameButton(gameSession);
        EndGameButton(gameSession);
    }

    private void StartGameButton(GameSession gameSession)
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

    private void EndGameButton(GameSession gameSession)
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
        await WaitFor(1, StartingGame);
        await WaitFor(2, StartedGame);
    }

    private async void EndGamePressed()
    {
        await WaitFor(1, EndingGame);
        await WaitFor(2, BackToWaitingState);
        QuantumRunner.DefaultGame.SendCommand(new ChangeSeedCommads() { NewSeed = UnityEngine.Random.Range(-1000, 1000) });
    }

    private void StartingGame() => QuantumRunner.DefaultGame.SendCommand(new ChangeGameStateCommads() { NewGameState = GameState.GameStarting.ToString() });

    private void StartedGame() => QuantumRunner.DefaultGame.SendCommand(new ChangeGameStateCommads() { NewGameState = GameState.GameStarted.ToString() });

    private void EndingGame() => QuantumRunner.DefaultGame.SendCommand(new ChangeGameStateCommads() { NewGameState = GameState.GameEnding.ToString() });

    private void BackToWaitingState() => QuantumRunner.DefaultGame.SendCommand(new ChangeGameStateCommads() { NewGameState = GameState.Waiting.ToString() });

    private async Task WaitFor(FP timeDelay, Action callBack)
    {
        var targetTime = frame.Number + (int)(timeDelay / frame.DeltaTime);
        while (frame.Number < targetTime)
        {
            await Task.Yield();
        }
        callBack.Invoke();
    }
}
