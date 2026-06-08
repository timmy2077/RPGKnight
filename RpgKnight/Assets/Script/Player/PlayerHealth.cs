using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public Animator healthTextanim;

    private void Start()
    {
        healthText.text ="HP: " + StatsManager.Instance.currentHealth + "/" + StatsManager.Instance.maxHealth;
    }
    public void ChangeHealth(int amount)
    {
        StatsManager.Instance.currentHealth += amount;
        healthTextanim.Play("TextChange");
       healthText.text ="HP: " + StatsManager.Instance.currentHealth + "/" + StatsManager.Instance.maxHealth;
        if (StatsManager.Instance.currentHealth <= 0)   
        {
            healthText.text ="HP: " + "0" + "/" + StatsManager.Instance.maxHealth;
            StartCoroutine(HealthTextCooldown());
        }
    }

    IEnumerator HealthTextCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
