using Quantum;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconSkillManager : QuantumMonoBehaviour
{
    public static IconSkillManager Instance { get => instance; }
    public Image iconKill;
    public Image iconDetect;
    public Image iconInject;
    public Image iconDestroy;
    public Toggle toggleMap;
    public GameObject miniMap;

    private Frame frame;
    private Image currIcon = null;
    private List<PlayerRef> playerRef;
    private RuntimePlayer playerData;
    private GameSession gameSession;
    private static IconSkillManager instance;

    private void Start()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    void Update()
    {
        frame = QuantumRunner.DefaultGame.Frames.Verified;
        playerRef = QuantumRunner.DefaultGame.GetLocalPlayers();
        gameSession = frame.GetSingleton<GameSession>();
        
        if (playerRef.Count <= 0)
        {
            return;
        }
        playerData = frame.GetPlayerData(playerRef[0]);

        CheckGameSession();
    }

    private void CheckGameSession()
    {
        if (gameSession.GameState == GameState.GameStarting)
        {
            HandlePlayerRoleIcon(true);
        }
        if (gameSession.GameState == GameState.GameEnding)
        {
            miniMap.SetActive(false);
            EnableMapButton(false);
            EnableIcon(currIcon, false);
        }
    }

    private void HandlePlayerRoleIcon(bool enableIcon)
    {
        switch (playerData.PlayerRole)
        {
            case PlayerRole.Monster:
                EnableIcon(iconKill, enableIcon);
                break;
            case PlayerRole.Detective:
                EnableIcon(iconDetect, enableIcon);
                break;
            case PlayerRole.Terrorist:
                EnableIcon(iconDestroy, enableIcon);
                break;
            case PlayerRole.Doctor:
                EnableIcon(iconInject, enableIcon);
                break;
            case PlayerRole.Engineer:
                EnableMapButton(enableIcon);
                break;
        }
    }

    private void EnableIcon(Image icon, bool enableIcon)
    {
        if (icon == null)
        {
            return;
        }
        icon.gameObject.SetActive(enableIcon);
        currIcon = icon;
        SetIconInteractable(false);
    }

    private void EnableMapButton(bool enable)
    {
        if (toggleMap == null)
        {
            return;
        }
        toggleMap.gameObject.SetActive(enable);
        if (enable == false)
        {
            toggleMap.onValueChanged.RemoveAllListeners();
            return;
        }
        toggleMap.onValueChanged.AddListener(state => TurnOnAndOffMap(state));
    }

    private void TurnOnAndOffMap(bool state)
    {
        miniMap.SetActive(state);
    }

    public void SetIconInteractable(bool setActive)
    {
        if(currIcon == null)
        {
            return;
        }
        if (!setActive)
        {
            currIcon.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);
        }
        else
        {
            currIcon.color = Color.white;
        }
    }
}