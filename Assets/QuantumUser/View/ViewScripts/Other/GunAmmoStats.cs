using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GunAmmoStats : MonoBehaviour
{
    public GameObject displayAmmoGameObject;
    public TMP_Text displayAmmo;

    private void Start()
    {
        displayAmmoGameObject.SetActive(false);
    }

    public void SetAmmoValue(int currAmmo, int maxAmmo)
    {
        displayAmmo.text = $"{currAmmo} / {maxAmmo}";
    }

    public void EnableAmmoStats(bool enable)
    {
        displayAmmoGameObject.SetActive(enable);
    }
}
