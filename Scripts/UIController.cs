using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public Slider healthSlider;
    public Slider shieldSlider;
    public TMP_Text timerText;

    private PlayerAttributes playerAttributes; // Reference to the PlayerAttributes script

    private void Start()
    {
        // Get reference to the PlayerAttributes script
        playerAttributes = PlayerAttributes.Instance;

        // Set max values based on player's max health and shield
        healthSlider.maxValue = playerAttributes.maxHealth;
        shieldSlider.maxValue = playerAttributes.maxShield;

        // Set initial slider values to match current health and shield
        healthSlider.value = playerAttributes.currentHealth;
        shieldSlider.value = playerAttributes.currentShield;
    }

    private void Update()
    {
        // Update the timer display
        UpdateTimerDisplay();

        // Update the health and shield sliders continuously
        UpdateSliders();
    }

    private void UpdateSliders()
    {
        // Update current health and shield values from PlayerAttributes script
        healthSlider.value = playerAttributes.currentHealth;
        shieldSlider.value = playerAttributes.currentShield;
    }

    private void UpdateTimerDisplay()
    {
        // Get remaining time from PlayerAttributes script
        float remainingTime = playerAttributes.GetRemainingTime();

        // Calculate minutes and seconds from the remaining time
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        // Update the timer text
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}