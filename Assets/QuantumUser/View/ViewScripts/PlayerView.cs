namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    public unsafe class PlayerView : QuantumEntityViewComponent
    {
        private PlayerInfo playerInfo;
        private Transform2D transform2D;
        private Vector3 mousePosition;
        private Animator animator;
        private Light2D spotLight;
        private bool facingLeft = true;

        private void Start()
        {
            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
            QuantumEvent.Subscribe(listener: this, handler: (EventIsMoving e) => IsMoving(e));
            spotLight = GetComponentInChildren<Light2D>();
            animator = GetComponentInChildren<Animator>(); 
        }

        private void Update()
        {
            playerInfo = VerifiedFrame.Get<PlayerInfo>(_entityView.EntityRef);
            transform2D = VerifiedFrame.Get<Transform2D>(_entityView.EntityRef);

            RotationController(VerifiedFrame.GetPlayerInput(playerInfo.PlayerRef)->mousePosition.X);
            CheckOwnerLight();
            CameraHandler();
        }

        private bool CheckLocalPlayer() => QuantumRunner.DefaultGame.PlayerIsLocal(playerInfo.PlayerRef);

        public void IsMoving(EventIsMoving e)
        {
            if (e.PlayerRef == playerInfo.PlayerRef)
            {
                animator.SetBool("isMoving", e.isMoving);
            }
        }

        private void CheckOwnerLight()
        {
            spotLight.enabled = CheckLocalPlayer();
        }

        private void CameraHandler()
        {
            if (CheckLocalPlayer())
            {
                Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -20);
            }
        }

        public void Flip()
        {
            facingLeft = !facingLeft;
            animator.transform.Rotate(0, 180, 0);
        }

        public void RotationController(FP mousePosition)
        {
            if (mousePosition < transform2D.Position.X && !facingLeft || mousePosition > transform2D.Position.X && facingLeft)
            {
                Flip();
            }
        }

        public void PollInput(CallbackPollInput callback)
        {
            if (!CheckLocalPlayer())
            {
                return;
            }

            mousePosition = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

            Input input = new()
            {
                PickUpItem = UnityEngine.Input.GetKey(KeyCode.E),
                DropItem = UnityEngine.Input.GetKey(KeyCode.Q),
                InteracAction = UnityEngine.Input.GetMouseButton(0),
                SelectItem = InputSelectIndex(),
                mousePosition = mousePosition.ToFPVector2(),
                RightMouseClick = UnityEngine.Input.GetMouseButton(1),
            };

            callback.SetInput(input, DeterministicInputFlags.Repeatable);
        }

        private int InputSelectIndex()
        {
            int selectIndex = 0;
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
    }
}
