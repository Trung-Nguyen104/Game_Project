using Photon.Client;
using Photon.Realtime;
using Quantum;
using UnityEngine;

enum RoleBehavior : byte
{
    Monster = 2, 
    Soldier = 3,
    Doctor = 4,
    Scientist = 5,
}

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
        if (!CheckLocalPlayer() || playerData.PlayerRole == PlayerRole.Astronaut)
        {
            return;
        }
        if (playerData.SkillTimer > 0)
        {
            playerData.SkillTimer -= Time.deltaTime;
            Debug.Log(playerData.SkillTimer);
        }
        if (playerData.PlayerRole == PlayerRole.Soldier && playerData.CurrHealth <= 0 && !playerData.IsMonsterKill)
        {
            Invoke(nameof(SoldierReporn), 15);
        }
        ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
        hitInfo = Physics2D.Raycast(ray.origin, ray.direction, 100, layerMask);
        if (hitInfo)
        {
            var playerTarget = hitInfo.collider.GetComponentInChildren<TargetPlayer>();
            var spriteRendererTarget = hitInfo.collider.GetComponentInChildren<SpriteRenderer>();
            if (playerTarget.playerRef == playerRef)
            {
                return;
            }
            switch (playerData.PlayerRole)
            {
                case PlayerRole.Monster:
                    spriteRendererTarget.color = Color.red;
                    if (UnityEngine.Input.GetMouseButtonDown(0) && playerData.SkillTimer <= 0)
                    {
                        client.OpRaiseEvent((byte)RoleBehavior.Monster, (int)playerTarget.playerRef, new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                        playerData.SkillTimer = 30;
                    }
                    break;
                case PlayerRole.Detective:
                    spriteRendererTarget.color = Color.gray;
                    if (UnityEngine.Input.GetMouseButtonDown(0) && playerData.SkillTimer <= 0)
                    {
                        Debug.Log(playerTarget.playerData.PlayerRole);
                        playerData.SkillTimer = 30;
                    }
                    break;
                case PlayerRole.Doctor:
                    spriteRendererTarget.color = Color.green;
                    if (UnityEngine.Input.GetMouseButtonDown(0) && playerData.SkillTimer <= 0)
                    {
                        client.OpRaiseEvent((byte)RoleBehavior.Doctor, (int)playerTarget.playerRef, new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                        playerData.SkillTimer = 30;
                    }
                    break;
                case PlayerRole.Scientist:
                    spriteRendererTarget.color = Color.blue;
                    if (UnityEngine.Input.GetMouseButtonDown(0))
                    {
                        client.OpRaiseEvent((byte)RoleBehavior.Scientist, (int)playerTarget.playerRef, new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                    }
                    break;
            }
            
        }
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = Color.white;
    }

    public void OnEvent(EventData photonEvent)
    {
        RuntimePlayer playerData = null;
        switch (photonEvent.Code)
        {
            case (byte)RoleBehavior.Monster:
                playerData = VerifiedFrame.GetPlayerData((int)photonEvent.CustomData);
                if (playerData.PlayerStatus == PlayerStatus.Immunity)
                {
                    playerData.PlayerStatus = PlayerStatus.Alive;
                }
                else
                {
                    playerData.CurrHealth = 0;
                    //playerData.IsMonsterKill = true;
                }
                break;
            case (byte)RoleBehavior.Soldier:
                playerData = VerifiedFrame.GetPlayerData((int)photonEvent.CustomData);
                playerData.CurrHealth = playerData.MaxHealth / 2;
                break;  
            case (byte)RoleBehavior.Doctor:
                playerData = VerifiedFrame.GetPlayerData((int)photonEvent.CustomData);
                playerData.PlayerStatus = PlayerStatus.Immunity;
                break;
            case (byte)RoleBehavior.Scientist:
                break;
        }
    }

    private void SoldierReporn()
    {
        client.OpRaiseEvent((byte)RoleBehavior.Soldier, (int)playerRef, new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
}
