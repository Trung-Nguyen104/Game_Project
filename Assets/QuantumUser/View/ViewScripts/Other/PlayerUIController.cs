using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    public static PlayerUIController Instance { get => instance; }
    private static PlayerUIController instance;

    public LoadingGamePanel LoadingGamePanel {  get; private set; }
    public PlayerHealthBar PlayerHealthBar { get; private set; }
    public GunAmmoStats GunAmmoStats { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    private void Start()
    {
        LoadingGamePanel = GetComponent<LoadingGamePanel>();
        PlayerHealthBar = GetComponent<PlayerHealthBar>();
        GunAmmoStats = GetComponent<GunAmmoStats>();
    }
}
