using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FirstCustomer2 : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Movement")]
    public float stopX = 0f;
    public float exitX = -60f;
    public float speed = 5f;

    [Header("Customer Waiting")]
    public float waitingTime = 600f;

    public CustomerMenuTrigger2 customerOrder;
    public GameObject orderCanvas;

    [Header("Next Customer")]
    public FirstCustomer2 nextCustomer;

    [Header("Cooking")]
    public JollofCookingManager jollofCookingManager;

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
        isLeaving = false;
        isCarrying = false;
        hasStartedWaiting = false;

        serviceStartTime = 0f;

        SetupReferences();

        if (orderCanvas != null)
            orderCanvas.SetActive(false);

        if (customerOrder != null)
            customerOrder.HideOrder();

        if (jollofCookingManager != null)
            jollofCookingManager.ResetCooking();

        if (gameObject.activeInHierarchy)
            StartCoroutine(MoveToStopPoint());
    }

    void SetupReferences()
    {
        if (customerOrder == null)
        {
            customerOrder =
                Object.FindFirstObjectByType<CustomerMenuTrigger2>();
        }

        if (orderCanvas == null)
        {
            orderCanvas = GameObject.Find("OrderCanvas");
        }

        if (jollofCookingManager == null)
        {
            jollofCookingManager =
                Object.FindFirstObjectByType<JollofCookingManager>();
        }
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

    IEnumerator AngryTimer()
    {
        yield return new WaitForSeconds(
            waitingTime
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
            $"🍛 SERVING CUSTOMER = {name}"
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
                $"⚠️ {name}: CustomerMenuManager2 reference is NULL!"
            );
        }

        // =====================================================
        // SET CARRYING FOOD
        // =====================================================

        if (selectedFood == 0)
        {
            // JOLLOF
            animator.SetInteger(
                "carryingFood",
                1
            );

            Debug.Log(
                $"🍛 {name} → carryingFood = 1 (Jollof)"
            );
        }
        else if (selectedFood == 1)
        {
            // EGUSI
            animator.SetInteger(
                "carryingFood",
                2
            );

            Debug.Log(
                $"🍲 {name} → carryingFood = 2 (Egusi)"
            );
        }
        else if (selectedFood == 2)
        {
            // POUNDED YAM
            animator.SetInteger(
                "carryingFood",
                3
            );

            Debug.Log(
                $"🥣 {name} → carryingFood = 3 (Pounded Yam)"
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
        // COOKING
        // =====================================================

        if (jollofCookingManager != null)
        {
            jollofCookingManager.OnFoodServed();
        }

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

            Debug.Log(
                "LEVEL TWO COMPLETE → LEVEL COMPLETE TWO"
            );

            SceneManager.LoadScene(
                "LevelComplete Two"
            );
        }
    }
}