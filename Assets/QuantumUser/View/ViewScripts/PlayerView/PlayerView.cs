namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    public unsafe class PlayerView : QuantumEntityViewComponent
    {
        public Transform shootPointTransform;
        public GameObject miniMapIcon;

        private PlayerInfo playerInfo;
        private Vector3 mousePosition;
        private RuntimePlayer playerData;
        private Light2D spotLight;

        private void Start()
        {
            QuantumEvent.Subscribe(listener: this, handler: (EventIsPlayerHitBullet e) => HandlePlayerHitBullet(e));
            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
            spotLight = GetComponentInChildren<Light2D>();
        }

        private void Update()
        {
            playerInfo = VerifiedFrame.Get<PlayerInfo>(_entityView.EntityRef);
            playerData = VerifiedFrame.GetPlayerData(playerInfo.PlayerRef);
            
            SetActivePlayerMiniMapIcon();
            HandlePlayerHealthBar();
            CheckOwnerLight();
            CameraHandler();
        }

        private void HandlePlayerHitBullet(EventIsPlayerHitBullet e)
        {
            if (playerInfo.PlayerRef != e.PlayerRef)
            {
                return;
            }
            playerData.CurrHealth -= e.Damage;
        }

        private void HandlePlayerHealthBar()
        {
            if (!CheckLocalPlayer())
            {
                return;
            }

            var gameSession = VerifiedFrame.GetSingleton<GameSession>();
            var healthBar = PlayerUIController.Instance.PlayerHealthBar;
            healthBar.SetMaxValueHealth(playerData.MaxHealth);

            if (gameSession.GameState != GameState.GameStarted || playerData.CurrHealth <= 0)
            {
                healthBar.EnableHealthBar(false);
                return;
            }

            healthBar.EnableHealthBar(true);
            healthBar.DisplayCurrHealth(playerData.CurrHealth.AsInt);

            
        }

        private void SetActivePlayerMiniMapIcon()
        {
            if (playerData.PlayerRole != PlayerRole.Engineer)
            {
                miniMapIcon.SetActive(false);
            }
            else
            {
                miniMapIcon.SetActive(true);
            }
        }

        private void CameraHandler()
        {
            if (CheckLocalPlayer())
            {
                Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -20);
            }
        }

        public void PollInput(CallbackPollInput callback)
        {
            if (!CheckLocalPlayer() || playerData.CurrHealth <= 0)
            {
                return;
            }

            mousePosition = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            var inputX = UnityEngine.Input.GetAxisRaw("Horizontal");
            var inputY = UnityEngine.Input.GetAxisRaw("Vertical");
            Input input = new()
            {
                SelectItem = InputSelectIndex(),
                PickUpOrProcessTask = UnityEngine.Input.GetKey(KeyCode.E),
                DropItem = UnityEngine.Input.GetKey(KeyCode.Q),
                UseSkill = UnityEngine.Input.GetKey(KeyCode.R),
                MousePosition = mousePosition.ToFPVector2(),
                ShootPointPosition = shootPointTransform.position.ToFPVector2(),
                ShootPointDirection = shootPointTransform.right.ToFPVector2(),
                ShootPointRotation = shootPointTransform.rotation.ToFPRotation2D(),
                Direction = new Vector2(inputX, inputY).ToFPVector2(),
                UseItemOrShoot = UnityEngine.Input.GetMouseButton(0),
            };
            callback.SetInput(input, DeterministicInputFlags.Repeatable);
        }

        private int InputSelectIndex()
        {
            int selectIndex = -1;
            if (UnityEngine.Input.GetKey(KeyCode.Alpha1))
            {
                selectIndex = 1;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.Alpha2))
            {
                selectIndex = 2;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.Alpha3))
            {
                selectIndex = 3;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.Alpha4))
            {
                selectIndex = 4;
            }
            return selectIndex;
        }

        private void CheckOwnerLight() => spotLight.enabled = CheckLocalPlayer();

        private bool CheckLocalPlayer() => QuantumRunner.DefaultGame.PlayerIsLocal(playerInfo.PlayerRef);

    }
}
