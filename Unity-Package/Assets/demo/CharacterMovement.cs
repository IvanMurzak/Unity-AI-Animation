using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    private Animator animator;
    private Animation legacyAnimation;

    void Start()
    {
        animator = GetComponent<Animator>();
        legacyAnimation = GetComponent<Animation>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Move
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

            // Rotate
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Animation
            SetWalking(true);
        }
        else
        {
            SetWalking(false);
        }
    }

    void SetWalking(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", isWalking);
        }
        else if (legacyAnimation != null)
        {
            if (isWalking)
            {
                if (!legacyAnimation.IsPlaying("Walk"))
                    legacyAnimation.Play("Walk");
            }
            else
            {
                legacyAnimation.Stop("Walk");
            }
        }
    }
}
