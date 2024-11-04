using Photon.Client;
using Photon.Realtime;
using Quantum;
using UnityEngine;

public class TargetPlayer : QuantumEntityViewComponent, IOnEventCallback
{
    [SerializeField] private UnityEngine.LayerMask layerMask;
    public PlayerRef playerRef { get; private set; }
    public RuntimePlayer playerData { get; set; }
    private SpriteRenderer spriteRenderer;
    private RealtimeClient client;
    private Ray ray;
    private RaycastHit2D hitInfo;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        client = QuantumRunner.Default?.NetworkClient;
        client.AddCallbackTarget(this);
    }

    private bool CheckLocalPlayer() => QuantumRunner.DefaultGame.PlayerIsLocal(playerRef);

    private void Update()
    {
        playerRef = VerifiedFrame.Get<PlayerInfo>(_entityView.EntityRef).PlayerRef;
        playerData = VerifiedFrame.GetPlayerData(playerRef);

        if (!CheckLocalPlayer() || playerData.PlayerRole != PlayerRole.Monster)
        {
            return;
        }
        ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
        hitInfo = Physics2D.Raycast(ray.origin, ray.direction, 100, layerMask);
        if (hitInfo)
        {
            var playerTarget = hitInfo.collider.GetComponentInChildren<TargetPlayer>();
            if (playerTarget.playerRef == playerRef)
            {
                return;
            }
            hitInfo.collider.GetComponentInChildren<SpriteRenderer>().color = Color.red;
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                client.OpRaiseEvent(2, (int)playerTarget.playerRef, new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
        }
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = Color.white;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 2)
        {
            Debug.Log(photonEvent.CustomData);
            VerifiedFrame.GetPlayerData((int)photonEvent.CustomData).CurrHealth = 0;
        }
    }
}
