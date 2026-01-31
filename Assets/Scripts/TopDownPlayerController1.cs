using UnityEngine;
using UnityEngine.InputSystem;

// Simple 2D top-down controller using Rigidbody2D.
[RequireComponent(typeof(Rigidbody2D))]
public class TopDownPlayerController1 : MonoBehaviour
{
    private enum EmotionState
    {
        Angry,
        Happy,
        Sad
    }

    public enum MaskType
    {
        None,
        Angry,
        Happy,
        Sad
    }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool rotateTowardsMovement = true;
    [Header("Emotion State")]
    [SerializeField] private EmotionState currentState = EmotionState.Happy;
    public bool Angry = false;
    public bool Happy = true;
    public bool Sad = false;

    [Header("Mask")]
    [SerializeField] private MaskType heldMask = MaskType.None;

    

    public Sprite AngrySprite;
    public Sprite HappySprite;
    public Sprite SadSprite;

    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private InputSystem_Actions inputActions;
    private InputSystem_Actions.PlayerActions playerActions;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d.gravityScale = 0f;
        inputActions = new InputSystem_Actions();
        playerActions = inputActions.Player;
        ApplyState(currentState);
    }

    private void OnEnable()
    {
        inputActions?.Enable();
    }

    private void OnDisable()
    {
        inputActions?.Disable();
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

    private void Update()
    {
        moveInput = playerActions.Move.ReadValue<Vector2>();
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        if (playerActions.SwitchR.WasPressedThisFrame())
        {
            TrySwitchToHeldMask();
        }
    }

    private void FixedUpdate()
    {
        rb2d.linearVelocity = moveInput * moveSpeed;

        if (rotateTowardsMovement && moveInput.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            rb2d.rotation = angle;
        }
    }

    private void TrySwitchToHeldMask()
    {
        if (heldMask == MaskType.None)
        {
            return;
        }

        EmotionState targetState = currentState;
        if (heldMask == MaskType.Angry)
        {
            targetState = EmotionState.Angry;
        }
        else if (heldMask == MaskType.Happy)
        {
            targetState = EmotionState.Happy;
        }
        else if (heldMask == MaskType.Sad)
        {
            targetState = EmotionState.Sad;
        }

        if (targetState == currentState)
        {
            return;
        }

        currentState = targetState;
        ApplyState(currentState);
    }

    private void ApplyState(EmotionState state)
    {
        Angry = state == EmotionState.Angry;
        Happy = state == EmotionState.Happy;
        Sad = state == EmotionState.Sad;

        if (spriteRenderer == null)
        {
            return;
        }

        if (state == EmotionState.Angry)
        {
            spriteRenderer.sprite = AngrySprite;
        }
        else if (state == EmotionState.Happy)
        {
            spriteRenderer.sprite = HappySprite;
        }
        else
        {
            spriteRenderer.sprite = SadSprite;
        }
    }

    public MaskType HeldMask => heldMask;

    public void AcquireMask(MaskType mask)
    {
        if (mask == MaskType.None)
        {
            return;
        }

        heldMask = mask;
    }
}
