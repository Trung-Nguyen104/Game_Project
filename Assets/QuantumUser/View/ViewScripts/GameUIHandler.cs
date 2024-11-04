using Photon.Client;
using Photon.Realtime;
using Quantum;
using System;
using UnityEngine;

enum GameSessionCode : byte
{
    GameStart = 0,
    GameEnd = 1,
}

public unsafe class GameUIHandler : QuantumMonoBehaviour, IOnEventCallback
{
    public UnityEngine.UI.Button startGameButton;
    public UnityEngine.UI.Button endGameButton;

    private Frame frame;
    private RealtimeClient client;
    private GameSession* gameSession;

    private void Start()
    {
        frame = QuantumRunner.DefaultGame?.Frames?.Verified;
        client = QuantumRunner.Default?.NetworkClient;
        client.AddCallbackTarget(this);
        Debug.Log($"Client {client.State} {client.CurrentRoom.Name}");
    }

    private void Update()
    {
        frame.Unsafe.TryGetPointerSingleton(out gameSession);

        StartGameButton();
        EndGameButton();
    }

    private void StartGameButton()
    {
        if (!client.LocalPlayer.IsMasterClient || gameSession->GameState == GameState.GameStarted)
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

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case (byte)GameSessionCode.GameStart:
                gameSession->GameState = GameState.GameStarting;
                Invoke(nameof(StartGame), 2f);
                break;
            case (byte)GameSessionCode.GameEnd:
                gameSession->GameState = GameState.GameEnded;
                Invoke(nameof(BackToWaitingState), 2f);
                break;
        }
    }

    private void StartGame() => gameSession->GameState = GameState.GameStarted;

    private void BackToWaitingState() => gameSession->GameState = GameState.Waiting;
}
