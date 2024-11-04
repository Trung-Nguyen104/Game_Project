using Quantum;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public static InventorySlot Instance { get => instance; }
    private static InventorySlot instance;
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
