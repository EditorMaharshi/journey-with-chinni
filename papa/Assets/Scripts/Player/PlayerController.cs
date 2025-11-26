using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 500f;
    
    private Rigidbody rb;
    private PlayerCombat playerCombat;
    private Vector3 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        // === Input Handling ===
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        moveDirection = new Vector3(h, 0, v).normalized;

        // Check for attack input (e.g., Left Mouse Button or 'E')
        if (Input.GetButtonDown("Fire1"))
        {
            playerCombat.TryAttack();
        }
        
        // Example for base interaction (e.g., opening base menu)
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Trigger UI logic (e.g., UIManager.Instance.ToggleBaseUI())
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            // 1. Movement
            Vector3 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);

            // 2. Rotation (Face the direction of movement)
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            
            // Trigger walking animation if using an Animator component
            // GetComponent<Animator>().SetBool("IsWalking", true); 
        }
        else
        {
            // GetComponent<Animator>().SetBool("IsWalking", false);
        }
    }
}
