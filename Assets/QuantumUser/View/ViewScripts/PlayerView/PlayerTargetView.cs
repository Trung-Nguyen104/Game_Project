using Photon.Client;
using Photon.Realtime;
using Quantum;
using System;
using UnityEngine;

enum RoleBehavior : byte
{
    Monster = 2, 
    Doctor = 4,
}

public class PlayerTargetView : QuantumEntityViewComponent, IOnEventCallback
{
    public UnityEngine.LayerMask layerMask;
    public PlayerRef PlayerRef { get; private set; }
    public RuntimePlayer PlayerData { get; private set; }
    public SpriteRenderer SpriteRenderer { get; set; }

    private RealtimeClient client;
    private Transform playerTransform;
    private PlayerTargetView doctorTargeting;
    private Ray ray;
    private RaycastHit2D hitInfo;
    private PlayerInfo playerInfo;
    private bool isCoolingDown;

    private void Start()
    {
        QuantumEvent.Subscribe(listener: this, handler: (EventIsCoolDown e) => IsCoolDown(e));
        SpriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = transform;
        client = QuantumRunner.Default?.NetworkClient;
        client.AddCallbackTarget(this);
    }

    private void IsCoolDown(EventIsCoolDown e)
    {
        if (e.PlayerRef != playerInfo.PlayerRef)
        {
            return;
        }
        isCoolingDown = e.IsCoolDown;
    }

    private void Update()
    {
        playerInfo = VerifiedFrame.Get<PlayerInfo>(_entityView.EntityRef);
        PlayerRef = playerInfo.PlayerRef;
        PlayerData = VerifiedFrame.GetPlayerData(PlayerRef);

        if (PlayerData.CurrHealth <= 0)
        {
            return;
        }

        RemoveDoctorTarget();

        if (!CheckLocalPlayer() || PlayerData.PlayerRole == PlayerRole.Astronaut || PlayerData.PlayerRole == PlayerRole.None)
        {
            return;
        }

        ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
        hitInfo = Physics2D.Raycast(ray.origin, ray.direction, 100, layerMask);
        HandlePlayerRoleTarget();
    }

    private void HandlePlayerRoleTarget()
    {
        if (!TryGetPlayerTarget(hitInfo, out var playerTarget))
        {
            return;
        }
        if (isCoolingDown || playerTarget.PlayerRef == PlayerRef)
        {
            IconSkillManager.Instance.SetIconInteractable(false);
            return;
        }
        switch (PlayerData.PlayerRole)
        {
            case PlayerRole.Monster:
                if (SkillDistance(playerTarget, 3f))
                {
                    return;
                }
                MonsterTarget(playerTarget);
                break;
            case PlayerRole.Detective:
                if (SkillDistance(playerTarget, 10f))
                {
                    return;
                }
                DetectiveTarget(playerTarget);
                break;
            case PlayerRole.Doctor:
                if (SkillDistance(playerTarget, 3f))
                {
                    return;
                }
                DoctorTarget(playerTarget);
                break;
        }
    }

    private void DoctorTarget(PlayerTargetView playerTarget)
    {
        playerTarget.SpriteRenderer.color = Color.green;
        IconSkillManager.Instance.SetIconInteractable(true);

        if (CheckUseSkillInput())
        {
            Debug.Log($"{PlayerData.PlayerRole} inject");
            if (playerTarget.PlayerData.IsImmunity)
            {
                return;
            }
            doctorTargeting = playerTarget;
            PlayerData.SkillTimer = 30;
            PlayerTargetRaiseEvent((byte)RoleBehavior.Doctor, playerTarget._entityView.EntityRef);
        }
    }

    private void RemoveDoctorTarget()
    {
        if (doctorTargeting != null)
        {
            if (doctorTargeting.PlayerData.IsImmunity)
            {
                doctorTargeting.SpriteRenderer.color = Color.green;
            }
            else
            {
                doctorTargeting.SpriteRenderer.color = Color.white;
            }
        }
    }

    private void DetectiveTarget(PlayerTargetView playerTarget)
    {
        playerTarget.SpriteRenderer.color = Color.gray;
        IconSkillManager.Instance.SetIconInteractable(true);

        if (CheckUseSkillInput())
        {
            Debug.Log($"{PlayerData.PlayerRole} detect");
            Debug.Log(playerTarget.PlayerData.PlayerRole);
            PlayerData.SkillTimer = 30;
        }
    }

    private void MonsterTarget(PlayerTargetView playerTarget)
    {
        playerTarget.SpriteRenderer.color = Color.red;
        IconSkillManager.Instance.SetIconInteractable(true);

        if (CheckUseSkillInput())
        {
            Debug.Log($"{PlayerData.PlayerRole} kill");
            PlayerData.SkillTimer = 30;
            PlayerTargetRaiseEvent((byte)RoleBehavior.Monster, playerTarget._entityView.EntityRef);
        }
    }

    private bool TryGetPlayerTarget(RaycastHit2D hitInfo, out PlayerTargetView playerTarget)
    {
        if (hitInfo)
        {
            playerTarget = hitInfo.collider.GetComponentInChildren<PlayerTargetView>();
            return true;
        }
        playerTarget = null;
        return false;
    }

    private void PlayerTargetRaiseEvent(byte roleEventCode, EntityRef targetRef)
    {
        client.OpRaiseEvent(roleEventCode, targetRef.ToString(), new RaiseEventArgs { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    private EntityRef GetTargetRef(EventData photonEvent)
    {
        EntityRef targetRef = default;
        if (EntityRef.TryParse((string)photonEvent.CustomData, out var _targetRef))
        {
            targetRef = _targetRef;
        }
        return targetRef;
    }

    private bool CheckLocalPlayer() => QuantumRunner.DefaultGame.PlayerIsLocal(PlayerRef);
    private bool CheckUseSkillInput() => UnityEngine.Input.GetKeyDown(KeyCode.R);
    private bool SkillDistance(PlayerTargetView playerTarget, float availableDist) => Vector2.Distance(playerTarget.playerTransform.position, playerTransform.position) > availableDist;

    private void OnMouseExit()
    {
        if (PlayerData.PlayerRole == PlayerRole.None)
        {
            return;
        }
        SpriteRenderer.color = Color.white;
        IconSkillManager.Instance.SetIconInteractable(false);
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case (byte)RoleBehavior.Monster:
                VerifiedFrame.Signals.OnMonsterKill(GetTargetRef(photonEvent));
                break;
            case (byte)RoleBehavior.Doctor:
                VerifiedFrame.Signals.OnDoctorInject(GetTargetRef(photonEvent));
                break;
        }
    }
}
