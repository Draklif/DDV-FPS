using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Health playerHealth;
    public Image healthFillImage;

    void Start()
    {
        playerHealth.OnHealthChanged += UpdateHealthBar;
        UpdateHealthBar(playerHealth.CurrentHealth / playerHealth.MaxHealth);
    }

    private void Update()
    {
        UpdateHealthBar(playerHealth.CurrentHealth / playerHealth.MaxHealth);
    }

    void UpdateHealthBar(float healthPercent)
    {
        if (healthFillImage != null)
        {
            float maxWidth = 512f;
            float newWidth = maxWidth * healthPercent;

            RectTransform rt = healthFillImage.rectTransform;
            rt.offsetMax = new Vector2(-maxWidth * (1f - healthPercent), rt.offsetMax.y);
        }
    }
}
