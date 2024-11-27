using Photon.Client;
using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

enum GameSessionCode : byte
{
    GameStart = 0,
    GameEnd = 1,
}

public class GameUIHandler : QuantumEntityViewComponent, IOnEventCallback
{
    public UnityEngine.UI.Button startGameButton;
    public UnityEngine.UI.Button endGameButton;

    private RealtimeClient client;
    private unsafe GameSession* gameSession;

    private void Start()
    {
        client = QuantumRunner.Default?.NetworkClient;
        client.AddCallbackTarget(this);
        startGameButton.onClick.AddListener(() => StartGamePressed());
        endGameButton.onClick.AddListener(() => EndGamePressed());
        Debug.Log($"Client {client.State} {client.CurrentRoom.Name}");
    }

    private unsafe void Update()
    {
        VerifiedFrame.Unsafe.TryGetPointerSingleton(out gameSession);

        StartGameButton();
        EndGameButton();
    }

    private unsafe void StartGameButton()
    {
        if (!client.LocalPlayer.IsMasterClient || gameSession->GameState != GameState.Waiting)
        {
            startGameButton.gameObject.SetActive(false);
        }
        else
        {
            startGameButton.gameObject.SetActive(true);
        }
    }

    private unsafe void EndGameButton()
    {
        if (!client.LocalPlayer.IsMasterClient || gameSession->GameState != GameState.GameStarted)
        {
            endGameButton.gameObject.SetActive(false);
        }
        else
        {
            endGameButton.gameObject.SetActive(true);
        }
    }

    public void StartGamePressed()
    {
        MasterClientRaiseEvent((byte)GameSessionCode.GameStart);
    }

    public void EndGamePressed()
    {
        MasterClientRaiseEvent((byte)GameSessionCode.GameEnd);
    }

    private void MasterClientRaiseEvent(byte gameSessionCode)
    {
        if (!client.OpRaiseEvent(gameSessionCode, null, new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable))
        {
            Debug.Log("RaiseEvent Failed");
            return;
        }
    }

    public async void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case (byte)GameSessionCode.GameStart:
                await WaitFor(1, StartingGame);
                await WaitFor(2, StartGame);
                break;
            case (byte)GameSessionCode.GameEnd:
                await WaitFor(1, EndingGame);
                await WaitFor(2, BackToWaitingState);
                break;
        }
    }

    private unsafe void StartingGame() => gameSession->GameState = GameState.GameStarting;
    private unsafe void StartGame() => gameSession->GameState = GameState.GameStarted;
    private unsafe void EndingGame() => gameSession->GameState = GameState.GameEnding;
    private unsafe void BackToWaitingState() => gameSession->GameState = GameState.Waiting;

    public async Task WaitFor(FP delayInSeconds, Action callback)
    {
        var targetTick = VerifiedFrame.Number + (int)(delayInSeconds / VerifiedFrame.DeltaTime);

        while (VerifiedFrame.Number < targetTick)
        {
            await Task.Yield();
        }
        callback.Invoke();
    }
}
