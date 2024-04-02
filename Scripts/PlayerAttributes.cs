using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    // Singleton instance
    public static PlayerAttributes Instance;

    // Player attributes
    public int maxHealth = 100;
    public int maxShield = 50;
    public int currentHealth;
    public int currentShield;
    
    // Timer attributes
    public float maxTime = 60f; // Maximum time in seconds
    private float remainingTime; // Remaining time

    private void Awake()
    {
        // Ensure only one instance of PlayerAttributes exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize player attributes
        currentHealth = maxHealth;
        currentShield = maxShield;

        // Initialize remaining time
        remainingTime = maxTime;
    }

    private void Update()
    {
        // Update the remaining time
        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Clamp(remainingTime, 0f, maxTime);
    }

    // Method to get the remaining time
    public float GetRemainingTime()
    {
        return remainingTime;
    }

    // Method to update player attributes based on changes
    public void UpdatePlayerAttributes(int healthChange, int shieldChange)
    {
        currentHealth += healthChange;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        currentShield += shieldChange;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
    }

    public void TakeDamage(int healthDamage, int shieldDamage)
    {
        currentHealth -= healthDamage;
        currentHealth = Mathf.Max(0, currentHealth);

        currentShield -= shieldDamage;
        currentShield = Mathf.Max(0, currentShield);
    }
}