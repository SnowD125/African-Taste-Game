using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FirstCustomer3 : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Movement")]
    public float stopX = 0f;
    public float exitX = -60f;
    public float speed = 5f;

    [Header("Customer Waiting")]
    public float waitingTime = 600f;
    [Tooltip("Extra wait after waitingTime before the customer gets angry " +
             "and leaves. Same behaviour and value as Level Two's angryDelay.")]
    public float angryDelay = 5f;

    public CustomerMenuTrigger3 customerOrder;
    public GameObject orderCanvas;

    [Header("Next Customer")]
    public FirstCustomer3 nextCustomer;

    [Header("Walk With Food")]
    public bool flipXWhenCarrying = false;
    public float carryingYOffset = 10f;

    [Header("Coins")]
    public float maxServiceTime = 600f;

    private bool isLeaving = false;
    private bool hasStartedWaiting = false;
    private bool isCarrying = false;

    private Coroutine angryCoroutine;

    private float lockedPositionY;
    private float serviceStartTime = 0f;

    public bool IsLeaving => isLeaving;


    // =========================================================
    // AWAKE
    // =========================================================

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        SetupReferences();
    }


    // =========================================================
    // ON ENABLE
    // =========================================================

    void OnEnable()
    {
        isLeaving = false;
        isCarrying = false;
        hasStartedWaiting = false;

        serviceStartTime = 0f;

        SetupReferences();

        if (orderCanvas != null)
            orderCanvas.SetActive(false);

        if (customerOrder != null)
            customerOrder.HideOrder();

        if (gameObject.activeInHierarchy)
            StartCoroutine(MoveToStopPoint());
    }


    // =========================================================
    // SETUP REFERENCES
    // =========================================================

    void SetupReferences()
    {
        if (customerOrder == null)
        {
            customerOrder =
                Object.FindFirstObjectByType<CustomerMenuTrigger3>();
        }

        if (orderCanvas == null)
        {
            orderCanvas = GameObject.Find("OrderCanvas");
        }
    }


    // =========================================================
    // MOVE TO STOP POINT
    // =========================================================

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

        transform.position = targetPosition;

        animator.SetBool("isIdle", true);

        if (!hasStartedWaiting)
        {
            hasStartedWaiting = true;

            if (customerOrder != null)
            {
                customerOrder.ShowOrder();
            }
            else
            {
                Debug.LogWarning(
                    $"[{name}] customerOrder is NULL — menu hazitaonekana!"
                );
            }

            serviceStartTime = Time.time;

            angryCoroutine =
                StartCoroutine(
                    AngryTimer()
                );
        }
    }


    // =========================================================
    // ANGRY TIMER
    // =========================================================

    IEnumerator AngryTimer()
    {
        yield return new WaitForSeconds(
            waitingTime
        );

        // Same as Level Two: after the waiting time, a further angryDelay
        // before the customer gets angry and leaves.
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
            $"🍛 SERVING LEVEL 3 CUSTOMER = {name}"
        );


        // =====================================================
        // READ ORDER BEFORE RESET
        // =====================================================

        int selectedFood = -1;

        if (
            customerOrder != null &&
            customerOrder.menuManager != null
        )
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
                $"⚠️ {name}: CustomerMenuManager3 reference is NULL!"
            );
        }


        // =====================================================
        // SET CARRYING FOOD
        // =====================================================

        if (selectedFood == 0)
        {
            // WAAKYE

            animator.SetInteger(
                "carryingFood",
                1
            );

            Debug.Log(
                $"🍛 {name} → carryingFood = 1 (Waakye)"
            );
        }
        else if (selectedFood == 1)
        {
            // BANKU & TILAPIA

            animator.SetInteger(
                "carryingFood",
                2
            );

            Debug.Log(
                $"🐟 {name} → carryingFood = 2 (Banku & Tilapia)"
            );
        }
        else if (selectedFood == 2)
        {
            // FUFU & LIGHT SOUP

            animator.SetInteger(
                "carryingFood",
                3
            );

            Debug.Log(
                $"🍲 {name} → carryingFood = 3 (Fufu & Light Soup)"
            );
        }
        else
        {
            // NO VALID FOOD

            animator.SetInteger(
                "carryingFood",
                0
            );

            Debug.LogWarning(
                $"⚠️ {name} → No valid food selected."
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


        // =====================================================
        // HAPPY ANIMATION
        // =====================================================

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
            Time.time - serviceStartTime;

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AwardCoins(
                timeUsed,
                maxServiceTime
            );
        }


        // =====================================================
        // START CARRYING
        // =====================================================

        StartCoroutine(
            SetWalkWithFoodAfterDelay()
        );


        // =====================================================
        // LEAVE
        // =====================================================

        StartCoroutine(
            LeaveAfterDelay()
        );
    }


    // =========================================================
    // START WALKING WITH FOOD
    // =========================================================

    IEnumerator SetWalkWithFoodAfterDelay()
    {
        yield return new WaitForSeconds(
            1.5f
        );

        if (animator != null)
        {
            // Keep EXACT Y position.

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
                $"🍛 {name} is now carrying Level 3 food."
            );
        }
    }


    // =========================================================
    // LEAVE AFTER DELAY
    // =========================================================

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


    // =========================================================
    // LEAVE
    // =========================================================

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


        // =====================================================
        // STOP CARRYING
        // =====================================================

        if (animator != null)
        {
            animator.SetBool(
                "isCarrying",
                false
            );
        }

        isCarrying = false;

        gameObject.SetActive(false);


        // =====================================================
        // NEXT CUSTOMER
        // =====================================================

        if (nextCustomer != null)
        {
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
            
            // Unlocks the next country in the Food Menu.
            LevelProgress.MarkCompleted(3);

            Debug.Log(
                "LEVEL THREE COMPLETE → LEVEL COMPLETE THREE"
            );

            SceneManager.LoadScene(
                "LevelComplete Three"
            );
        }
    }
}