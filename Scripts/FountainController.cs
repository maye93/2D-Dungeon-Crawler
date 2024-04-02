using UnityEngine;

public class FountainController : MonoBehaviour
{
    public bool isConsumed = false;
    [SerializeField] private AudioSource fountainSound;
    private Animator animator;

    void Start()
    {
        isConsumed = false;
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isConsumed && other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (gameObject.CompareTag("Health"))
            {
                isConsumed = true;
                animator.SetBool("IsConsumed", true);
                fountainSound.Play();
                playerController.HealPlayer();
            }
            else if (gameObject.CompareTag("Shield"))
            {
                isConsumed = true;
                animator.SetBool("IsConsumed", true);
                fountainSound.Play();
                playerController.ShieldPlayer();
            }
        }
    }
}