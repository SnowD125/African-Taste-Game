using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FirstCustomer : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public float stopX = 0f;
    public float exitX = -60f;
    public float speed = 5f;
    public float idleTime = 18f;
    public float angryDelay = 120f;

    public CustomerMenuTrigger customerOrder;
    public GameObject orderCanvas;

    [Header("Next Customer")]
    public FirstCustomer nextCustomer;

    [Header("Cooking")]
    public CookingManager cookingManager;
    public VegCookingManager vegCookingManager;
    public CoconutClick coconutClick;

    [Header("Walk With Food")]
    public bool flipXWhenCarrying = false;
    public float carryingYOffset = 10f;

    [Header("Coins")]
    public float maxServiceTime = 60f;

    // =========================================================
    // CURRENT ACTIVE CUSTOMER
    // =========================================================

    public static FirstCustomer CurrentCustomer { get; private set; }

    private bool isLeaving = false;
    private bool hasStartedWaiting = false;
    private bool isCarrying = false;

    private Coroutine angryCoroutine;

    private float lockedPositionY;
    private float serviceStartTime = 0f;

    public bool IsLeaving => isLeaving;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SetupReferences();
    }

    void OnEnable()
    {
        // =====================================================
        // THIS CUSTOMER IS NOW THE ACTIVE CUSTOMER
        // =====================================================

        CurrentCustomer = this;

        Debug.Log(
            $"⭐ CURRENT CUSTOMER = {name}"
        );

        isLeaving = false;
        isCarrying = false;
        hasStartedWaiting = false;

        serviceStartTime = 0f;

        SetupReferences();

        if (orderCanvas != null)
            orderCanvas.SetActive(false);

        if (customerOrder != null)
            customerOrder.HideOrder();

        if (vegCookingManager != null)
            vegCookingManager.ResetCooking();

        if (gameObject.activeInHierarchy)
            StartCoroutine(MoveToStopPoint());
    }

    void OnDisable()
    {
        // Only clear if THIS is the current customer.
        if (CurrentCustomer == this)
        {
            CurrentCustomer = null;
        }
    }

    void SetupReferences()
    {
        if (customerOrder == null)
            customerOrder =
                Object.FindFirstObjectByType<CustomerMenuTrigger>();

        if (orderCanvas == null)
            orderCanvas =
                GameObject.Find("OrderCanvas");

        if (cookingManager == null)
            cookingManager =
                Object.FindFirstObjectByType<CookingManager>();

        if (vegCookingManager == null)
            vegCookingManager =
                Object.FindFirstObjectByType<VegCookingManager>();

        if (coconutClick == null)
            coconutClick =
                Object.FindFirstObjectByType<CoconutClick>();

        Debug.Log(
            $"[{name}] SetupReferences — " +
            $"customerOrder null? {customerOrder == null}, " +
            $"orderCanvas null? {orderCanvas == null}"
        );
    }

    IEnumerator MoveToStopPoint()
    {
        if (animator == null)
            yield break;

        animator.SetBool("isIdle", false);
        animator.SetBool("isCarrying", false);

        Vector2 targetPosition =
            new Vector2(
                stopX,
                transform.position.y
            );

        while (
            Vector2.Distance(
                transform.position,
                targetPosition
            ) > 0.01f
        )
        {
            transform.position =
                Vector2.MoveTowards(
                    transform.position,
                    targetPosition,
                    speed * Time.deltaTime
                );

            yield return null;
        }

        transform.position =
            targetPosition;

        animator.SetBool(
            "isIdle",
            true
        );

        if (!hasStartedWaiting)
        {
            hasStartedWaiting = true;

            Debug.Log(
                $"[{name}] Reached stopX. " +
                $"customerOrder null? {customerOrder == null}"
            );

            if (customerOrder != null)
            {
                Debug.Log(
                    $"[{name}] Calling ShowOrder(). " +
                    $"menuManager null? " +
                    $"{customerOrder.menuManager == null}"
                );

                customerOrder.ShowOrder();
            }
            else
            {
                Debug.LogWarning(
                    $"[{name}] customerOrder is NULL — menu hazitaonekana!"
                );
            }

            serviceStartTime =
                Time.time;

            yield return new WaitForSeconds(
                idleTime
            );

            angryCoroutine =
                StartCoroutine(
                    AngryTimer()
                );
        }
    }

    IEnumerator AngryTimer()
    {
        yield return new WaitForSeconds(
            angryDelay
        );

        if (!isLeaving)
        {
            if (customerOrder != null)
                customerOrder.HideOrder();

            if (orderCanvas != null)
                orderCanvas.SetActive(false);

            animator.SetBool(
                "isIdle",
                false
            );

            animator.ResetTrigger(
                "isHappy"
            );

            animator.SetTrigger(
                "isAngry"
            );

            isLeaving = true;

            StartCoroutine(
                Leave()
            );
        }
    }

    // =========================================================
    // SERVE CUSTOMER
    // =========================================================

    public void ServeCustomer()
    {
        if (isLeaving)
        {
            Debug.LogWarning(
                $"[{name}] ServeCustomer ignored because customer is already leaving."
            );

            return;
        }

        Debug.Log(
            $"🍛 SERVING CUSTOMER = {name} ✅"
        );

        // =====================================================
        // READ CUSTOMER ORDER BEFORE RESETTING ANYTHING
        // =====================================================

        int selectedFood = -1;

        if (customerOrder != null &&
            customerOrder.menuManager != null)
        {
            selectedFood =
                customerOrder.menuManager.selectedFood;

            Debug.Log(
                $"🍽️ {name} ORDER = selectedFood {selectedFood}"
            );
        }
        else
        {
            Debug.LogWarning(
                $"⚠️ {name}: CustomerMenuManager reference is NULL!"
            );
        }

        // =====================================================
        // SET CARRYING FOOD
        // =====================================================

        if (selectedFood == 0)
        {
            // Ugali + Dagaa
            animator.SetInteger(
                "carryingFood",
                1
            );

            Debug.Log(
                $"🐟 {name} → carryingFood = 1 (Ugali + Dagaa)"
            );
        }
        else if (selectedFood == 4)
        {
            // Ugali + Tembele
            animator.SetInteger(
                "carryingFood",
                2
            );

            Debug.Log(
                $"🥬 {name} → carryingFood = 2 (Ugali + Tembele)"
            );
        }
        else
        {
            // Other foods
            animator.SetInteger(
                "carryingFood",
                0
            );

            Debug.Log(
                $"🍽️ {name} → carryingFood = 0"
            );
        }

        // =====================================================
        // STOP ANGRY TIMER
        // =====================================================

        if (angryCoroutine != null)
        {
            StopCoroutine(
                angryCoroutine
            );

            angryCoroutine = null;
        }

        animator.SetBool(
            "isIdle",
            false
        );

        animator.ResetTrigger(
            "isAngry"
        );

        animator.SetTrigger(
            "isHappy"
        );

        // =====================================================
        // COINS
        // =====================================================

        float timeUsed =
            Time.time -
            serviceStartTime;

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AwardCoins(
                timeUsed,
                maxServiceTime
            );
        }

        // =====================================================
        // START CARRYING ANIMATION
        // =====================================================

        StartCoroutine(
            SetWalkWithFoodAfterDelay()
        );

        // =====================================================
        // RESET COOKING
        // =====================================================

        cookingManager?.OnFoodServed();

        vegCookingManager?.ResetCooking();

        coconutClick?.ResetCoconut();

        // =====================================================
        // LEAVE
        // =====================================================

        StartCoroutine(
            LeaveAfterDelay()
        );
    }

    IEnumerator SetWalkWithFoodAfterDelay()
    {
        yield return new WaitForSeconds(
            1.5f
        );

        if (animator != null)
        {
            // Keep EXACTLY the same Y position.
            lockedPositionY =
                transform.position.y;

            isCarrying = true;

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX =
                    flipXWhenCarrying;
            }

            animator.SetBool(
                "isCarrying",
                true
            );

            transform.position =
                new Vector3(
                    transform.position.x,
                    lockedPositionY,
                    transform.position.z
                );

            Debug.Log(
                $"🍛 {name} is now carrying food."
            );
        }
    }

    IEnumerator LeaveAfterDelay()
    {
        yield return new WaitForSeconds(
            2f
        );

        isLeaving = true;

        StartCoroutine(
            Leave()
        );
    }

    IEnumerator Leave()
    {
        float leaveSpeed =
            speed * 1.5f;

        if (customerOrder != null)
            customerOrder.HideOrder();

        if (orderCanvas != null)
            orderCanvas.SetActive(false);

        float exitY =
            isCarrying
                ? lockedPositionY
                : transform.position.y;

        Vector2 exitPosition =
            new Vector2(
                exitX,
                exitY
            );

        while (
            Vector2.Distance(
                new Vector2(
                    transform.position.x,
                    transform.position.y
                ),
                exitPosition
            ) > 0.01f
        )
        {
            float targetY =
                isCarrying
                    ? lockedPositionY
                    : transform.position.y;

            Vector2 current =
                new Vector2(
                    transform.position.x,
                    targetY
                );

            transform.position =
                Vector2.MoveTowards(
                    current,
                    exitPosition,
                    leaveSpeed *
                    Time.deltaTime
                );

            yield return null;
        }

        if (animator != null)
        {
            animator.SetBool(
                "isCarrying",
                false
            );
        }

        isCarrying = false;

        // =====================================================
        // CLEAR CURRENT CUSTOMER
        // =====================================================

        if (CurrentCustomer == this)
        {
            CurrentCustomer = null;
        }

        gameObject.SetActive(false);

        // =====================================================
        // NEXT CUSTOMER
        // =====================================================

        if (nextCustomer != null)
        {
            Debug.Log(
                $"➡️ Next customer = {nextCustomer.name}"
            );

            nextCustomer.gameObject.SetActive(true);
        }
        else
        {
            int totalCoins = 0;

            if (CoinManager.Instance != null)
            {
                totalCoins =
                    CoinManager.Instance.GetTotalCoins();
            }

            PlayerPrefs.SetInt(
                "TotalCoins",
                totalCoins
            );

            PlayerPrefs.Save();

            SceneManager.LoadScene(
                "LevelComplete"
            );
        }
    }
}