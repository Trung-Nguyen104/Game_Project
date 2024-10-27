using Photon.Client;
using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomUI : QuantumMonoBehaviour, IOnEventCallback
{
    public RealtimeClient client;
    public RuntimeConfig runtimeConfig;

    [Space]
    public RuntimePlayer runtimePlayer;
    public int MaxPlayers;

    [Space]
    public UnityEngine.UI.Button startGameButton;

    private AppSettings appSettings;
    private Frame frame;
    private List<PlayerRef> listPlayerRef;
    private GameSession gameSession;
    private QuantumRunner runner;
    private const byte StartGame = 0;


    private void Awake()
    {
        frame = QuantumRunner.DefaultGame?.Frames?.Verified;
        client = QuantumRunner.Default.NetworkClient;

        //if (client == null)
        //{
        //    appSettings = PhotonServerSettings.Global.AppSettings;
        //    client = new RealtimeClient(appSettings.Protocol);
        //    client.AddCallbackTarget(this);
        //}
        //var connectionArguments = new MatchmakingArguments
        //{
        //    PhotonSettings = appSettings,
        //    PluginName = "QuantumPlugin",
        //    MaxPlayers = Constants.PLAYER_COUNT,
        //    UserId = Guid.NewGuid().ToString(),
        //    //NetworkClient = client
        //};
        //client = await MatchmakingExtensions.ConnectToRoomAsync(connectionArguments);

        client.AddCallbackTarget(this);
        listPlayerRef = QuantumRunner.DefaultGame.GetLocalPlayers();

        Debug.Log($"Client State == {client.State}");
        Debug.Log($"Is Master Client == {client.LocalPlayer.IsMasterClient}");

    }

    private void Update()
    {
        gameSession = frame.GetSingleton<GameSession>();
        client.Service();
        if (client.LocalPlayer.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(true);
        }
        else
        {
            startGameButton.gameObject.SetActive(false);
        }
    }

    public void StartGamePressed()
    {
        if (!client.OpRaiseEvent(StartGame, null, new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable))
        {
            Debug.Log($"Client Can't RaiseEvent");
        }
        Debug.Log($"Client Already RaiseEvent");
    }

    public void OnEvent(EventData photonEvent)
    {
        Debug.Log($"Photon Event Code : {photonEvent.Code}");
        if (photonEvent.Code == StartGame)
        {
            StartQuantumGame();
        }
    }

    private async void StartQuantumGame()
    {
        runtimeConfig.Seed = Guid.NewGuid().GetHashCode();

        var sessionRunnerArguments = new SessionRunner.Arguments()
        {
            RunnerFactory = QuantumRunnerUnityFactory.DefaultFactory,
            GameParameters = QuantumRunnerUnityFactory.CreateGameParameters,
            SessionConfig = QuantumDeterministicSessionConfigAsset.DefaultConfig,
            RuntimeConfig = runtimeConfig,
            ClientId = client.UserId,
            PlayerCount = MaxPlayers,
            GameMode = DeterministicGameMode.Multiplayer,
            Communicator = new QuantumNetworkCommunicator(client, ShutdownConnectionOptions.None),
            StartGameTimeoutInSeconds = 30f
        };


        try
        {
            await QuantumRunner.Default.ShutdownAsync();
            runner = await QuantumRunner.StartGameAsync(sessionRunnerArguments);
        }
        catch (Exception e)
        {
            print(e.Message);
        }
        
        runner.Game.AddPlayer(runtimePlayer);
    }
}
