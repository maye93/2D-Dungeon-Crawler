using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int currentShield;
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;

    private PlayerAttributes playerAttributes; // Reference to PlayerAttributes script

    [SerializeField] private AudioSource runSound;
    [SerializeField] private AudioSource hitHealthSound;
    [SerializeField] private AudioSource hitShieldSound;
    [SerializeField] private AudioSource deathSound;

    private bool isMovementEnabled = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0; // Make player kinematic
        rb.freezeRotation = true; // Freeze player rotation

        playerAttributes = PlayerAttributes.Instance; // Get reference to PlayerAttributes script

        currentShield = playerAttributes.currentShield;
    }

    private void Update()
    {
        currentShield = playerAttributes.currentShield;
        if (!isMovementEnabled)
            return; // Disable controls if the player is dead

        // Movement input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        // Move the player
        rb.velocity = movement * moveSpeed;

        animator.SetBool("IsRunning", movement.magnitude > 0.1f);
        
        // Rotate the character based on input
        if (moveHorizontal != 0)
        {
            // Determine the direction
            transform.localScale = new Vector3(Mathf.Sign(moveHorizontal), 1, 1);
            runSound.Play();
        }
        else
        {
            runSound.Stop();
        }

        if (playerAttributes.currentHealth <= 0)
        {
            isMovementEnabled = false;
            animator.SetBool("IsRunning", false);
            StartCoroutine(DeathCoroutine());
        }
    }

    public IEnumerator DeathCoroutine()
    {
        animator.SetBool("isDead", true);
        deathSound.Play();

        yield return new WaitForSeconds(1f);
        deathSound.Stop();
        SceneManager.LoadScene("Lose");
    }

    public void TakeDamageShield(int damage)
    {
        hitShieldSound.Play();
        playerAttributes.currentShield -= damage;
        playerAttributes.currentShield = Mathf.Max(0, playerAttributes.currentShield);

        // Update PlayerAttributes script with current shield value
        playerAttributes.UpdatePlayerAttributes(0, -damage);
    }

    public void TakeDamageHealth(int damage)
    {
        hitHealthSound.Play();
        playerAttributes.currentHealth -= damage;
        playerAttributes.currentHealth = Mathf.Max(0, playerAttributes.currentHealth);

        // Update PlayerAttributes script with current health value
        playerAttributes.UpdatePlayerAttributes(-damage, 0);
    }

    public void ShieldPlayer()
    {
        playerAttributes.currentShield += 10;
        playerAttributes.currentShield = Mathf.Clamp(playerAttributes.currentShield, 0, playerAttributes.maxShield);

        // Update PlayerAttributes script with current shield value
        playerAttributes.UpdatePlayerAttributes(0, 10);
    }

    public void HealPlayer()
    {
        playerAttributes.currentHealth += 10;
        playerAttributes.currentHealth = Mathf.Clamp(playerAttributes.currentHealth, 0, playerAttributes.maxHealth);

        // Update PlayerAttributes script with current health value
        playerAttributes.UpdatePlayerAttributes(10, 0);
    }
}