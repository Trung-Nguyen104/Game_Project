using Quantum;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public static InventorySlot Instance { get => instance; }
    public ItemSlotsView[] itemSlots;
    public Image inventoryParent;
    
    private static InventorySlot instance;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }
}
