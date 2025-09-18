using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerStateMachine : MonoBehaviour
{
    public enum PlayerState { Idle, Walking, Jumping }
    public PlayerState current = PlayerState.Idle;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;   // << novo

    [Header("Jump")]
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;
    public float coyoteTime = 0.12f;
    public float jumpBuffer = 0.12f;

    [Header("Ground Check")]
    public Transform groundCheck;        // empty no pé do player
    private float groundRadius = 0.2f;
    private LayerMask groundMask = ~0;

    private CharacterController controller;
    private float verticalVelocity = 0f;
    private float lastGroundedTime = -999f;
    private float lastJumpPressedTime = -999f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        // Sugestão:
        // controller.center = new Vector3(0f, 1f, 0f);
        // controller.height = 2f;
    }

    void Update()
    {
        // 1) Inputs
        bool forward = Input.GetKey(forwardKey);
        bool backward = Input.GetKey(backwardKey);
        float moveInput = 0f;
        if (forward)  moveInput += 1f;
        if (backward) moveInput -= 1f;
        moveInput = Mathf.Clamp(moveInput, -1f, 1f);

        if (Input.GetKeyDown(jumpKey)) lastJumpPressedTime = Time.time;

        // 2) Grounded robusto
        bool grounded = IsGroundedRobust() || controller.isGrounded;
        if (grounded) lastGroundedTime = Time.time;

        // 3) Estados
        switch (current)
        {
            case PlayerState.Idle:
                if (Mathf.Abs(moveInput) > 0.01f) current = PlayerState.Walking;
                TryJump(grounded);
                break;

            case PlayerState.Walking:
                if (Mathf.Abs(moveInput) <= 0.01f) current = PlayerState.Idle;
                TryJump(grounded);
                break;

            case PlayerState.Jumping:
                if (grounded && verticalVelocity <= 0f)
                    current = Mathf.Abs(moveInput) > 0.01f ? PlayerState.Walking : PlayerState.Idle;
                break;
        }

        // 4) Movimento horizontal (frente/trás) — também no ar
        Vector3 horizontal = transform.forward * (moveInput * moveSpeed);

        // 5) Gravidade + cola no chão
        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        // 6) Integra gravidade
        verticalVelocity += gravity * Time.deltaTime;

        // 7) Move final
        Vector3 velocity = horizontal;
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    void TryJump(bool groundedNow)
    {
        bool canCoyote = (Time.time - lastGroundedTime) <= coyoteTime;
        bool buffered  = (Time.time - lastJumpPressedTime) <= jumpBuffer;

        if (buffered && (groundedNow || canCoyote))
        {
            verticalVelocity = Mathf.Sqrt(Mathf.Abs(gravity) * 2f * jumpHeight);
            current = PlayerState.Jumping;
            lastJumpPressedTime = -999f;
        }
    }

    bool IsGroundedRobust()
    {
        if (!groundCheck) return controller.isGrounded;
        return Physics.CheckSphere(groundCheck.position, groundRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = new Color(0f, 1f, 0f, 0.35f);
        Gizmos.DrawSphere(groundCheck.position, groundRadius);
    }
#endif
}
