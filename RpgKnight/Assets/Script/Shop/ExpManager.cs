using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExpManager : MonoBehaviour
{
    public int currentExp;
    public int level;
    public int expToLevel=10;
    public float expToLevelMultiplier = 1.5f;
    public Slider expSlider;
    public TextMeshProUGUI levelText;

    public static event Action<int> OnLevelUp;

    void Start()
    {
        // LoadExpData();
        UpdateExpSlider();
    }

    void Update()
    {
      
    }

    private void OnEnable()
    {
        Enemy_Health.OnMonsterDefeated += GainExp;
        InventoryManager.OnEXperienceGained += GainExp;
    }

    private void OnDisable()
    {
        Enemy_Health.OnMonsterDefeated -= GainExp;
        InventoryManager.OnEXperienceGained -= GainExp;
    }

    public void GainExp(int exp)
    {
        currentExp += exp;
        if(currentExp >= expToLevel)
        {
            LevelUp();
        }
        UpdateExpSlider();
        // SaveExpData();
    }

    private void LevelUp()
    {
        level++;
        currentExp -= expToLevel;
        expToLevel = Mathf.RoundToInt(expToLevel * expToLevelMultiplier);
        OnLevelUp?.Invoke(1);
        // SaveExpData();

    }

    public void UpdateExpSlider()
    {
        expSlider.maxValue = expToLevel;
        expSlider.value = currentExp;
        levelText.text = "Level: " + level;

    }

    private void LoadExpData()
    {
        currentExp = PlayerPrefs.GetInt("CurrentExp", 0);
        level = PlayerPrefs.GetInt("Level", 1);
        expToLevel = PlayerPrefs.GetInt("ExpToLevel", 10);
    }

    private void SaveExpData()
    {
        PlayerPrefs.SetInt("CurrentExp", currentExp);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("ExpToLevel", expToLevel);
        PlayerPrefs.Save();
    }
}
