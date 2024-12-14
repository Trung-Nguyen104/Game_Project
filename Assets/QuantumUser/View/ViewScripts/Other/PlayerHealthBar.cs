using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public GameObject healthBarGameObject;
    public Slider healthBar;

    private void Start()
    {
        healthBarGameObject.SetActive(false);
    }

    public void SetMaxValueHealth(int maxHealth)
    {
        healthBar.maxValue = maxHealth;
    }

    public void DisplayCurrHealth(int currHealth)
    {
        healthBar.value = currHealth;
    }

    public void EnableHealthBar(bool enable)
    {
        healthBarGameObject.SetActive(enable);
    }
}
