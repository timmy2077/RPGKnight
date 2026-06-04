using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    public StatsUI statsUI;
    public TextMeshProUGUI healthText;


[Header("Combat Stats")]
public int damage;
public float weaponRange;
public float knockbackForce;
public float knockbackTime;
public float stunTime;

[Header("Movement Stats")]
public int moveSpeed;

[Header("Health Stats")]
public int maxHealth;
public int currentHealth;

public float duration;

private void Awake()
{
    if (Instance == null)
        Instance = this;
    else
        Destroy(gameObject);
}   

    public void UpdateMaxHealth(int amount)
    {
        maxHealth += amount;
        healthText.text = "HP: " + currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        if(currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthText.text = "HP: " + currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void UpdateSpeed(int amount)
    {
        moveSpeed += amount;
        statsUI.UpdateAll();
    }

    public void UpdateDamage(int amount)
    {
        damage += amount;
        statsUI.UpdateAll();
    }
}
