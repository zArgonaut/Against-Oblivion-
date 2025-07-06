using UnityEngine;

// Unified 3D movement script replacing legacy PlayerController variants

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    [Header("Run Settings")]
    public float runMultiplier = 2f;
    public int runCost = 1;

    [Header("Dodge Settings")]
    public float dodgeDistance = 3f;
    public int dodgeCost = 20;

    private CharacterController controller;
    private PlayerStamina stamina;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        stamina = GetComponent<PlayerStamina>();
    }

    void Update()
    {
        // Determine movement speed
        float speed = moveSpeed;
        if (Input.GetButton("Run"))
        {
            if (stamina == null || stamina.Use(runCost))
                speed *= runMultiplier;
        }

        // Move constantly forward
        Vector3 forward = transform.forward * speed * Time.deltaTime;
        controller.Move(forward);

        // Dodge input instantly moves the player
        if (Input.GetButtonDown("Dodge"))
        {
            if (stamina == null || stamina.Use(dodgeCost))
                controller.Move(transform.forward * dodgeDistance);
        }

        // Rotate with horizontal input
        float h = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, h * rotationSpeed * Time.deltaTime);
    }
}
