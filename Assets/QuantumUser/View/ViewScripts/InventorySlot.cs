using Quantum;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    private static InventorySlot instance;
    public static InventorySlot Instance { get => instance; }

    public ItemSlotsView[] itemSlots;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
        itemSlots = GetComponentsInChildren<ItemSlotsView>();
    }
}
