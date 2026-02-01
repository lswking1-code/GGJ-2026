using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// Simple 2D top-down controller using Rigidbody2D.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
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
    public float hurtForce = 10f;
    [SerializeField] private float hurtDuration = 0.15f;

    [Header("Emotion State")]
    [SerializeField] private EmotionState currentState = EmotionState.Happy;
    public bool Angry = false;
    public bool Happy = true;
    public bool Sad = false;

    [Header("Mask")]
    [SerializeField] private MaskType heldMask = MaskType.None;

    [Header("EventRaised")]
    public MaskChangeEventSO maskChangeEventSO;
    [Header("EventListener")]
    public VoidEventSO gameClearEventSO;
    [Header("Camera Confiner")]
    [SerializeField] private CinemachineConfiner2D confiner2D;
    [SerializeField] private string cameraEdgeTag = "CameraEdge";

    [Header("Animator")]
    private Animator animator;

    [Header("Sprites")]
    public GameObject MaskMini;
    public Sprite AngrySprite;
    public Sprite HappySprite;
    public Sprite SadSprite;
    public Sprite AngrySpriteMini;
    public Sprite HappySpriteMini;
    public Sprite SadSpriteMini;

    // ??????????????????????????????????????????
    // Sound Effects
    // ??????????????????????????????????????????
    [Header("Sound Effects")]
    [SerializeField] private AudioClip walkSFX;
    [SerializeField] private AudioClip hurtSFX;
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private AudioClip acquireMaskSFX;
    [SerializeField] private AudioClip useMaskSFX;

    [SerializeField] private float walkSFXInterval = 0.3f;  // ?????????

    private AudioSource audioSource;
    private float walkSFXTimer = 0f;
    // ??????????????????????????????????????????

    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer maskMiniRenderer;
    private Vector2 moveInput;
    private InputSystem_Actions inputActions;
    private InputSystem_Actions.PlayerActions playerActions;
    private float hurtTimer;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        rb2d.gravityScale = 0f;
        inputActions = new InputSystem_Actions();
        playerActions = inputActions.Player;
        animator = GetComponent<Animator>();
        if (MaskMini != null)
        {
            maskMiniRenderer = MaskMini.GetComponent<SpriteRenderer>();
        }
        ApplyState(currentState);
        UpdateMaskMiniSprite();
    }

    private void Start()
    {
        maskChangeEventSO.RaiseEvent(2);
    }

    private void OnEnable()
    {
        inputActions?.Enable();
        gameClearEventSO.OnEventRaised += OnGameClearEventRaised;
    }

    private void OnDisable()
    {
        inputActions?.Disable();
        gameClearEventSO.OnEventRaised -= OnGameClearEventRaised;
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

        HandleWalkSFX();
    }

    private void FixedUpdate()
    {
        if (hurtTimer > 0f)
        {
            hurtTimer -= Time.fixedDeltaTime;
            return;
        }

        rb2d.linearVelocity = moveInput * moveSpeed;
        animator.SetFloat("Speed", rb2d.linearVelocity.magnitude);

        if (rotateTowardsMovement && moveInput.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            rb2d.rotation = angle;
        }
    }

    // ??????????????????????????????????????????
    // Walk SFX Logic
    // ??????????????????????????????????????????
    private void HandleWalkSFX()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.001f;

        if (isMoving)
        {
            walkSFXTimer += Time.deltaTime;
            if (walkSFXTimer >= walkSFXInterval)
            {
                PlaySFX(walkSFX);
                walkSFXTimer = 0f;
            }
        }
        else
        {
            // ??????????????????????
            walkSFXTimer = 0f;
        }
    }
    // ??????????????????????????????????????????

    private void OnCollisionEnter2D(Collision2D collision)
    {
        UpdateConfinerFromCollider(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        UpdateConfinerFromCollider(other);
    }

    private void UpdateConfinerFromCollider(Collider2D other)
    {
        if (confiner2D == null || other == null)
        {
            return;
        }

        if (!other.CompareTag(cameraEdgeTag))
        {
            return;
        }

        confiner2D.BoundingShape2D = other;
        confiner2D.InvalidateBoundingShapeCache();
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
            maskChangeEventSO.RaiseEvent(1);
        }
        else if (heldMask == MaskType.Happy)
        {
            targetState = EmotionState.Happy;
            maskChangeEventSO.RaiseEvent(2);
        }
        else if (heldMask == MaskType.Sad)
        {
            targetState = EmotionState.Sad;
            maskChangeEventSO.RaiseEvent(3);
        }

        if (targetState == currentState)
        {
            return;
        }

        currentState = targetState;
        ApplyState(currentState);
        heldMask = MaskType.None;
        UpdateMaskMiniSprite();

        // ??????
        PlaySFX(useMaskSFX);
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
        UpdateMaskMiniSprite();

        // ??????
        PlaySFX(acquireMaskSFX);
    }

    private void UpdateMaskMiniSprite()
    {
        if (maskMiniRenderer == null)
        {
            return;
        }

        if (heldMask == MaskType.Angry)
        {
            maskMiniRenderer.sprite = AngrySpriteMini;
        }
        else if (heldMask == MaskType.Happy)
        {
            maskMiniRenderer.sprite = HappySpriteMini;
        }
        else if (heldMask == MaskType.Sad)
        {
            maskMiniRenderer.sprite = SadSpriteMini;
        }
        else
        {
            maskMiniRenderer.sprite = null;
        }
    }

    public void GetHurt(Transform attacker)
    {
        if(Happy)
        {
            return;
        }
        if (attacker == null)
        {
            Debug.Log("attacker is null");
            return;
        }

        rb2d.linearVelocity = Vector2.zero;
        Vector2 dir = ((Vector2)transform.position - (Vector2)attacker.position).normalized;
        if (dir.sqrMagnitude < 0.0001f)
        {
            Debug.Log("dir is zero");
            return;
        }

        Debug.Log("GetHurt");
        animator.SetTrigger("Hurt");
        hurtTimer = hurtDuration;
        rb2d.AddForce(dir * hurtForce, ForceMode2D.Impulse);

        // ????
        PlaySFX(hurtSFX);
    }

    /// <summary>
    /// ??????????? HealthSystem????
    /// ????????????????????????
    /// </summary>
    public void Die()
    {
        // ????
        PlaySFX(deathSFX);

        // TODO: ????????????????????????
        Debug.Log("Player has died.");
    }

    // ??????????????????????????????????????????
    // ????
    // ??????????????????????????????????????????
    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || audioSource == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }
    private void OnGameClearEventRaised()
    {
        inputActions?.Disable();
    }
}