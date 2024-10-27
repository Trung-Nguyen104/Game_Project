using Photon.Client;
using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

enum GameSessionCode : byte
{
    GameStart = 0,
    GameEnd = 1,
}

public class GameUIHandler : QuantumMonoBehaviour, IOnEventCallback
{
    public UnityEngine.UI.Button startGameButton;
    public UnityEngine.UI.Button endGameButton;

    private Frame frame;
    private RealtimeClient client;
    private unsafe GameSession* gameSession;

    private void Start()
    {
        frame = QuantumRunner.DefaultGame?.Frames?.Verified;
        client = QuantumRunner.Default?.NetworkClient;
        client.AddCallbackTarget(this);
        Debug.Log(client.CurrentRoom.Name);
    }

    private unsafe void Update()
    {
        try
        {
            frame.Unsafe.TryGetPointerSingleton(out gameSession);
        }catch (Exception ex)
        {
            print(ex.Message);
        }
        Debug.Log($"Client State : {client.State}");
        client.Service();
        StartGameButton();
        EndGameButton();
    }

    private unsafe void StartGameButton()
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
        if (!client.OpRaiseEvent((byte)GameSessionCode.GameStart, null, new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable))
        {
            Debug.Log("RaiseEvent Failed");
            return;
        }
    }

    public void EndGamePressed()
    {
        if (!client.OpRaiseEvent((byte)GameSessionCode.GameEnd, null, new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable))
        {
            Debug.Log("RaiseEvent Failed");
            return;
        }
    }

    private void EndGameHandle()
    {
        var listLocalPlayer = QuantumRunner.DefaultGame.GetLocalPlayers();
        var runtimeConfig = frame.RuntimeConfig;
        runtimeConfig.Seed = Guid.NewGuid().GetHashCode();
        var sessionRunnerArguments = new SessionRunner.Arguments()
        {
            RunnerFactory = QuantumRunnerUnityFactory.DefaultFactory,
            GameParameters = QuantumRunnerUnityFactory.CreateGameParameters,
            SessionConfig = QuantumDeterministicSessionConfigAsset.DefaultConfig,
            RuntimeConfig = runtimeConfig,
            ClientId = client.UserId,
            PlayerCount = 8,
            GameMode = DeterministicGameMode.Multiplayer,
            Communicator = QuantumRunner.Default.Communicator,
            StartGameTimeoutInSeconds = 30f,
            InitialTick = 0
        };
        try
        {
            QuantumRunner.Default.ShutdownAsync();
            SceneManager.LoadSceneAsync(frame.FindAsset(runtimeConfig.Map).Scene, LoadSceneMode.Single);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(frame.FindAsset(runtimeConfig.Map).Scene));
            QuantumRunner.StartGame(sessionRunnerArguments); 
        }
        catch(Exception ex)
        {
            print(ex.Message);
        }
    }

    public unsafe void OnEvent(EventData photonEvent)
    {
        Debug.Log($"Photon Event Code : {photonEvent.Code}");
        switch (photonEvent.Code)
        {
            case (byte)GameSessionCode.GameStart:
                gameSession->GameState = GameState.GameStarting;
                Invoke(nameof(StartGame), 1);
                break;
            case (byte)GameSessionCode.GameEnd:
                gameSession->GameState = GameState.GameEnded;
                Invoke(nameof(BackToWaitingState), 2);
                //EndGameHandle();
                break;
        }
    }

    private unsafe void StartGame() => gameSession->GameState = GameState.GameStarted;

    private unsafe void BackToWaitingState() => gameSession->GameState = GameState.Waiting;
}
