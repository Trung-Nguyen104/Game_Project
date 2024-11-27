namespace Quantum
{
    using UnityEngine;

    public class PlayerAimView : QuantumEntityViewComponent
    {
        private Transform aimTransform;
        private const float maxAngle = 90;

        private void Start()
        {
            aimTransform = transform.Find("Aim");
        }

        private void Update()
        {
            AimingController();
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
