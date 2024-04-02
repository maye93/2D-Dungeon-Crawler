using UnityEngine;
using System.Collections;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackRange = 1.1f;
    [SerializeField] private int damage = 20;
    [SerializeField] private AudioSource growlSound;

    private bool isAttacking = false;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0; // Make player kinematic
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (playerTransform == null || isAttacking)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= chaseRange)
        {
            growlSound.Play();
            if (distanceToPlayer <= attackRange)
            {
                // Attack if player is in attack range
                StartCoroutine(AttackCoroutine());
            }
            else
            {
                // Chase the player
                ChasePlayer();
            }
        }
        else
        {
            growlSound.Stop();
            StopChasing();
        }
    }

    private void ChasePlayer()
    {
        // Move towards the player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Flip sprite if needed
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Facing left
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Facing right
        }

        // Play run animation
        animator.SetBool("ifSeePlayer", true);
        animator.SetBool("ifPlayerClose", false);
    }

    private IEnumerator AttackCoroutine()
    {
        // Start attacking animation
        isAttacking = true;
        animator.SetBool("ifSeePlayer", false);
        animator.SetBool("ifPlayerClose", true);

        // Wait for the attack animation to finish
        yield return new WaitForSeconds(0.7f);

        // Check if the player is still within attack range before attacking
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackRange)
        {
            // Deal damage to the player
            PlayerController playerController = playerTransform.GetComponent<PlayerController>();
            if (playerController != null)
            {
                if (playerController.currentShield > 0)
                {
                    playerController.TakeDamageShield(damage);
                }
                else
                {
                    playerController.TakeDamageHealth(damage);
                }
            }
        }

        // End attacking animation
        isAttacking = false;
    }

    private void StopChasing()
    {
        // Stop chasing animation
        animator.SetBool("ifSeePlayer", false);
        animator.SetBool("ifPlayerClose", false);
    }

    // Detect collisions with the player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Start attacking if the player collides with the zombie
            StartCoroutine(AttackCoroutine());
        }
    }

    // Stop attacking when the player exits collision with the zombie
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isAttacking = false;
        }
    }
}