using UnityEngine;

namespace Quantum
{
    public unsafe class AutoLoadItemPosition : QuantumEntityViewComponent
    {
        bool loadAllItemPosition;

        private void Start()
        {
            loadAllItemPosition = false;
        }

        private void Update()
        {
            if (_entityView == null || loadAllItemPosition)
            {
                return;
            }

            var itemSpawner = VerifiedFrame.Get<ItemSpawner>(_entityView.EntityRef);
            var itemSpawnPosition = VerifiedFrame.FindAsset(itemSpawner.ItemSpawnPosition);

            itemSpawnPosition.positions = new();

            for (var i = 0; i < itemSpawner.Positions.Length; i++)
            {
                itemSpawnPosition.positions.Add(transform.Find($"ItemSpawnPosition - ({i})").position.ToFPVector2());
            }

            loadAllItemPosition = true;
        }
    }
}
