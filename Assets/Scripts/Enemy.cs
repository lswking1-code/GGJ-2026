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
            if (MoveBackToReturnNav())
            {
                return;
            }

            isReturningToNav = false;
        }

        if (playerInRange)
        {
            if (TryGetReturnNavIfNoNavInRange(out returnNavIndex))
            {
                isReturningToNav = true;
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
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }
    private void OnMaskChange(int value)
    {
        switch (value)
        {
            case 1:
                moveSpeed = AngryMoveSpeed;
                break;
            case 2:
                moveSpeed = HappyMoveSpeed;
                break;
            case 3:
                moveSpeed = SadMoveSpeed;
                break;
        }
    }
}
