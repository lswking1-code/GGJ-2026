using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject[] NavPoints;
    public float moveSpeed = 5f;
    public float AngryMoveSpeed = 5f;
    public float HappyMoveSpeed = 0f;
    public float SadMoveSpeed = 2.5f;
    public float arriveDistance = 0.1f;
    public float attackDistance = 10f;
    public float targetLostDistance = 15f;
    public float returnTeleportDelay = 3f;

    private int currentIndex = 0;
    private int direction = 1;
    private GameObject player;
    private Transform self;
    private Rigidbody2D rb;
    private float arriveDistanceSqr;
    private float attackDistanceSqr;
    private float targetLostDistanceSqr;
    private bool isReturningToNav;
    private int returnNavIndex = -1;
    private bool hasPendingMoveSpeed;
    private float pendingMoveSpeed;
    private float returnTimer;
    private int currentMask = -1;

    [Header("Sprite")]
    public Sprite AngrySprite;
    public Sprite HappySprite;
    public Sprite SadSprite;
    private SpriteRenderer spriteRenderer;

    private Attack attack;
    private Animator animator;

    [Header("EventListener")]
    public MaskChangeEventSO maskChangeEventSO;

    private void OnEnable()
    {
        maskChangeEventSO.OnEventRaised += OnMaskChange;
    }
    private void OnDisable()
    {
        maskChangeEventSO.OnEventRaised -= OnMaskChange;
    }


    private void Awake()
    {
        self = transform;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        CacheDistances();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attack = GetComponent<Attack>();
        animator = GetComponent<Animator>();
    }

    private void CacheDistances()
    {
        arriveDistanceSqr = arriveDistance * arriveDistance;
        attackDistanceSqr = attackDistance * attackDistance;
        targetLostDistanceSqr = targetLostDistance * targetLostDistance;
    }

    private void FixedUpdate()
    {
        CacheDistances();
        bool hasNavPoints = NavPoints != null && NavPoints.Length > 0;
        if (!hasNavPoints)
        {
            return;
        }

        if (currentIndex < 0 || currentIndex >= NavPoints.Length)
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, NavPoints.Length - 1);
            direction = 1;
        }

        Vector2 playerPos;
        bool playerInRange = TryGetPlayerInRange(out playerPos);

        if (isReturningToNav)
        {
            if (!IsHappy())
            {
                returnTimer += Time.fixedDeltaTime;
                if (returnTeleportDelay > 0f && returnTimer >= returnTeleportDelay)
                {
                    TeleportToReturnNav();
                    return;
                }
            }
            if (MoveBackToReturnNav())
            {
                return;
            }

            isReturningToNav = false;
            returnTimer = 0f;
            ApplyPendingMoveSpeed();
        }

        if (playerInRange)
        {
            if (TryGetReturnNavIfNoNavInRange(out returnNavIndex))
            {
                isReturningToNav = true;
                returnTimer = 0f;
                MoveBackToReturnNav();
                return;
            }

            MoveTowards(playerPos);
            return;
        }

        Patrol();
    }

    private void AdvanceIndex()
    {
        if (NavPoints == null || NavPoints.Length == 0)
        {
            return;
        }

        if (currentIndex == NavPoints.Length - 1)
        {
            direction = -1;
        }
        else if (currentIndex == 0)
        {
            direction = 1;
        }

        currentIndex += direction;
        currentIndex = Mathf.Clamp(currentIndex, 0, NavPoints.Length - 1);
    }

    private bool TryGetReturnNavIfNoNavInRange(out int navIndex)
    {
        navIndex = -1;

        Vector2 position = rb.position;
        for (int i = 0; i < NavPoints.Length; i++)
        {
            GameObject nav = NavPoints[i];
            if (nav == null)
            {
                continue;
            }

            float distanceSqr = ((Vector2)nav.transform.position - position).sqrMagnitude;
            if (distanceSqr <= targetLostDistanceSqr)
            {
                return false;
            }
        }

        float closestNavDistanceSqr = float.MaxValue;
        for (int i = 0; i < NavPoints.Length; i++)
        {
            GameObject nav = NavPoints[i];
            if (nav == null)
            {
                continue;
            }

            float distanceSqr = ((Vector2)nav.transform.position - position).sqrMagnitude;
            if (distanceSqr < closestNavDistanceSqr)
            {
                closestNavDistanceSqr = distanceSqr;
                navIndex = i;
            }
        }

        return navIndex != -1;
    }

    private bool MoveBackToReturnNav()
    {
        if (returnNavIndex < 0 || returnNavIndex >= NavPoints.Length)
        {
            return false;
        }

        GameObject closestNav = NavPoints[returnNavIndex];
        if (closestNav == null)
        {
            return false;
        }

        Vector2 navPos = closestNav.transform.position;
        MoveTowards(navPos);
        if ((navPos - rb.position).sqrMagnitude <= arriveDistanceSqr)
        {
            currentIndex = returnNavIndex;
            AdvanceIndex();
            return false;
        }

        return true;
    }

    private void TeleportToReturnNav()
    {
        if (returnNavIndex < 0 || returnNavIndex >= NavPoints.Length)
        {
            isReturningToNav = false;
            returnTimer = 0f;
            return;
        }

        GameObject targetNav = NavPoints[returnNavIndex];
        if (targetNav == null)
        {
            isReturningToNav = false;
            returnTimer = 0f;
            return;
        }

        Vector2 navPos = targetNav.transform.position;
        rb.position = navPos;
        rb.MovePosition(navPos);
        currentIndex = returnNavIndex;
        AdvanceIndex();
        isReturningToNav = false;
        returnTimer = 0f;
        ApplyPendingMoveSpeed();
    }

    private bool IsHappy()
    {
        return currentMask == 2;
    }

    private int GetClosestNavIndex()
    {
        if (NavPoints == null || NavPoints.Length == 0)
        {
            return -1;
        }

        int closestIndex = -1;
        float closestNavDistanceSqr = float.MaxValue;
        Vector2 position = rb.position;
        for (int i = 0; i < NavPoints.Length; i++)
        {
            GameObject nav = NavPoints[i];
            if (nav == null)
            {
                continue;
            }

            float distanceSqr = ((Vector2)nav.transform.position - position).sqrMagnitude;
            if (distanceSqr < closestNavDistanceSqr)
            {
                closestNavDistanceSqr = distanceSqr;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    private void ApplyPendingMoveSpeed()
    {
        if (!hasPendingMoveSpeed)
        {
            return;
        }

        moveSpeed = pendingMoveSpeed;
        hasPendingMoveSpeed = false;
    }

    private bool TryGetPlayerInRange(out Vector2 playerPos)
    {
        playerPos = default;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player == null)
        {
            return false;
        }

        playerPos = player.transform.position;
        if ((playerPos - rb.position).sqrMagnitude >= attackDistanceSqr)
        {
            return false;
        }

        return true;
    }

    private void Patrol()
    {
        GameObject target = NavPoints[currentIndex];
        if (target == null)
        {
            AdvanceIndex();
            return;
        }

        Vector2 targetPos = target.transform.position;
        MoveTowards(targetPos);

        if ((targetPos - rb.position).sqrMagnitude <= arriveDistanceSqr)
        {
            AdvanceIndex();
        }
    }

    private void MoveTowards(Vector2 targetPos)
    {
        Vector2 currentPos = rb.position;
        Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, moveSpeed * Time.fixedDeltaTime);
        float currentSpeed = (newPos - currentPos).magnitude / Time.fixedDeltaTime;
        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
        }
        rb.MovePosition(newPos);
    }
    private void OnMaskChange(int value)
    {
        currentMask = value;
        switch (value)
        {
            case 1:
                moveSpeed = AngryMoveSpeed;
                spriteRenderer.sprite = AngrySprite;
                if (attack != null)
                {
                    attack.damage = 1;
                }
                break;
            case 2:
                moveSpeed = HappyMoveSpeed;
                spriteRenderer.sprite = HappySprite;
                if (attack != null)
                {
                    attack.damage = 0;
                }
                break;
            case 3:
                moveSpeed = SadMoveSpeed;
                spriteRenderer.sprite = SadSprite;
                if (attack != null)
                {
                    attack.damage = 1;
                }
                break;
        }
    }
}
