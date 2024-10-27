namespace Quantum
{
    using UnityEngine;

    public class AimView : QuantumEntityViewComponent
    {
        public Transform shootPointTransform;
        private Transform aimTransform;
        private RuntimePlayer playerLocal;
        private const float maxAngle = 90;

        private void Start()
        {
            aimTransform = transform.Find("Aim");
        }

        private void Update()
        {
            AimingController();
            SetUpShootPoint();
        }

        private void SetUpShootPoint()
        {
            playerLocal = VerifiedFrame.GetPlayerData(VerifiedFrame.Get<PlayerInfo>(_entityView.EntityRef).PlayerRef);
            playerLocal.ShootPointPosition = shootPointTransform.position.ToFPVector2();
            playerLocal.ShootPointDirection = shootPointTransform.right.ToFPVector2();
            playerLocal.ShootPointRotation = shootPointTransform.rotation.ToFPRotation2D();
        }

        private void AimingController()
        {
            var angle = VerifiedFrame.Get<MousePointerInfo>(_entityView.EntityRef).AimAngle.AsFloat;
            aimTransform.eulerAngles = new Vector3(0, 0, angle);

            Vector3 localScale = Vector3.one;
            if (angle > maxAngle || angle < -maxAngle)
            {
                localScale.y = -1;
            }
            else
            {
                localScale.x = 1;
            }
            aimTransform.localScale = localScale;
        }
    }
}
