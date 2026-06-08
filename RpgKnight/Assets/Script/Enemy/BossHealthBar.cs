using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;
    [SerializeField] private Enemy_Health bossHealth;
    [SerializeField] private TextMeshProUGUI bossName;
    [SerializeField] private Image bossImage;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI bossHealthText;

    private void Start()
    {
        SetupUI();

        if (bossHealth != null)
            bossHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
            bossHealth.OnHealthChanged -= UpdateHealthBar;
    }

    private void SetupUI()
    {
        if (enemySO != null)
        {
            if (bossName != null)
                bossName.text = enemySO.enemyName;

            if (bossImage != null)
                bossImage.sprite = enemySO.enemySprite;
        }

        if (bossHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = bossHealth.maxHealth;
            healthSlider.value = bossHealth.currentHealth;
        }

        UpdateHealthText();
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (bossHealthText != null && bossHealth != null)
        {
            bossHealthText.text = bossHealth.currentHealth + "/" + bossHealth.maxHealth;
        }
    }
}
