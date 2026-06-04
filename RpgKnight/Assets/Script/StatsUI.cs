using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;
    public GameObject statsCanvas;

    private void Start()
    {
        UpdateAll();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            bool newState = !statsCanvas.activeSelf;
            statsCanvas.SetActive(newState);
            // 打开面板时暂停游戏，关闭面板时恢复游戏
            Time.timeScale = newState ? 0 : 1;
            // 当打开面板时，更新所有统计信息
            if(newState)
            {
                UpdateAll();
            } 
        }
    }
 
    public void UpdateDamage( )
    {
        statsSlots[0].GetComponentInChildren<TMP_Text>().text ="Attack: " + StatsManager.Instance.damage;
        
    }
    public void UpdateSpeed( )
    {
        statsSlots[1].GetComponentInChildren<TMP_Text>().text ="Speed: " + StatsManager.Instance.moveSpeed;
    }

    public void UpdateHealth( )
    {
        statsSlots[2].GetComponentInChildren<TMP_Text>().text ="Health: " + StatsManager.Instance.currentHealth;
    }

    public void UpdateMaxHealth( )
    {
        statsSlots[3].GetComponentInChildren<TMP_Text>().text ="maxHealth: " + StatsManager.Instance.maxHealth;
    }




    public void UpdateAll()
    {
        UpdateDamage();
        UpdateSpeed();
        UpdateHealth();
        UpdateMaxHealth();
    }
}
