namespace Quantum
{
    public unsafe class AutoLoadItemPosition : QuantumEntityViewComponent
    {
        int index;
        private void Start()
        {
            index = 0;
        }
        private void Update()
        {
            if (_entityView == null)
            {
                return;
            }
            var itemSpawner = VerifiedFrame.Get<ItemSpawner>(_entityView.EntityRef);
            if (index < itemSpawner.Positions.Length)
            {
                var itemSpawnPosition = VerifiedFrame.FindAsset(itemSpawner.ItemSpawnPosition);
                itemSpawnPosition.positions = new();
                for (index = 0; index < itemSpawner.Positions.Length; index++)
                {
                    itemSpawnPosition.positions.Add(transform.Find($"ItemSpawnPosition - ({index})").position.ToFPVector2());
                }
            }
        }
    }
}
