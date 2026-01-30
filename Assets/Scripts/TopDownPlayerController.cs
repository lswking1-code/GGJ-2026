using UnityEngine;
using UnityEngine.InputSystem;

// Simple 2D top-down controller using Rigidbody2D.
[RequireComponent(typeof(Rigidbody2D))]
public class TopDownPlayerController : MonoBehaviour
{
    private enum EmotionState
    {
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
            SwitchNextState();
        }
        else if (playerActions.SwitchL.WasPressedThisFrame())
        {
            SwitchPreviousState();
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

    private void SwitchNextState()
    {
        if (currentState == EmotionState.Sad)
        {
            currentState = EmotionState.Angry;
        }
        else
        {
            currentState = (EmotionState)((int)currentState + 1);
        }

        ApplyState(currentState);
    }

    private void SwitchPreviousState()
    {
        if (currentState == EmotionState.Angry)
        {
            currentState = EmotionState.Sad;
        }
        else
        {
            currentState = (EmotionState)((int)currentState - 1);
        }

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
}
