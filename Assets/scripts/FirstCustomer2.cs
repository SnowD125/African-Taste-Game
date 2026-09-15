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
    [Tooltip("Extra wait after waitingTime before the customer gets angry " +
             "and leaves. Same behaviour and value as Level One's angryDelay.")]
    public float angryDelay = 5f;

    public CustomerMenuTrigger2 customerOrder;
    public GameObject orderCanvas;

    [Header("Next Customer")]
    public FirstCustomer2 nextCustomer;

    [Header("Cooking")]
    public JollofCookingManager jollofCookingManager;

    [Header("Walk With Food")]
    [Tooltip("SUPERSEDED. Orientation now follows the direction of travel " +
             "(see Face). Kept only so existing scene data is not lost.")]
    public bool flipXWhenCarrying = false;
    public float carryingYOffset = 10f;

    [Header("Orientation")]
    [Tooltip("Tick ONLY if this customer's side-view artwork (walking and " +
             "walking with food) is drawn facing RIGHT. Facing is derived " +
             "from the direction of travel, never from a serialized flipX.")]
    public bool artFacesRight = false;

    [Header("Coins")]
    public float maxServiceTime = 600f;

    private bool isLeaving = false;
    private bool hasStartedWaiting = false;
    private bool isCarrying = false;

    private Coroutine angryCoroutine;

    private float lockedPositionY;
    private float serviceStartTime = 0f;

    // Served-dish detection. The Egusi and Pounded Yam managers serve the
    // customer the moment their dish is clicked, whatever the menu order was,
    // and leave their state on Served afterwards. Remembering last frame's
    // state lets ServeCustomer() tell a dish served THIS frame from a stale one.
    private EgusiCookingManager egusiCookingManager;
    private PoundedYamCookingManager poundedYamCookingManager;
    private EgusiCookingManager.CookState egusiStateLastFrame;
    private PoundedYamCookingManager.PoundedYamState yamStateLastFrame;

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

        RememberServeStates();

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

        if (egusiCookingManager == null)
        {
            egusiCookingManager =
                Object.FindFirstObjectByType<EgusiCookingManager>();
        }

        if (poundedYamCookingManager == null)
        {
            poundedYamCookingManager =
                Object.FindFirstObjectByType<PoundedYamCookingManager>();
        }
    }

    void LateUpdate()
    {
        RememberServeStates();
    }

    void RememberServeStates()
    {
        if (egusiCookingManager != null)
            egusiStateLastFrame = egusiCookingManager.egusiState;

        if (poundedYamCookingManager != null)
            yamStateLastFrame = poundedYamCookingManager.yamState;
    }

    // =========================================================
    // ORIENTATION
    //
    // Same rule as Level One Customer 2: facing is derived from the actual
    // direction of travel and from nothing else. The old code walked in with
    // whatever m_FlipX the scene held and walked out with flipXWhenCarrying,
    // which was only right for today's spawn/exit sides and artwork.
    // =========================================================

    void Face(float directionX)
    {
        if (spriteRenderer == null)
            return;

        if (Mathf.Abs(directionX) < 0.001f)
            return;

        bool movingRight = directionX > 0f;

        spriteRenderer.flipX =
            artFacesRight ? !movingRight : movingRight;
    }

    IEnumerator MoveToStopPoint()
    {
        if (animator == null)
            yield break;

        animator.SetBool("isIdle", false);
        animator.SetBool("isCarrying", false);

        // Walking in: face the stop point.
        Face(stopX - transform.position.x);

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

        // Same as Level One: after the waiting time, a further angryDelay
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
        // CARRY WHAT WAS ACTUALLY SERVED
        //
        // Egusi and Pounded Yam are served straight from their cooking
        // managers, whatever the menu order was (or with no order at all),
        // and their state flips to Served in this same frame. Jollof only
        // reaches this method through ServePlate1(), which already requires
        // selectedFood == 0, so it needs no override.
        // =====================================================

        if (
            egusiCookingManager != null &&
            egusiCookingManager.egusiState ==
                EgusiCookingManager.CookState.Served &&
            egusiStateLastFrame !=
                EgusiCookingManager.CookState.Served
        )
        {
            selectedFood = 1;
        }
        else if (
            poundedYamCookingManager != null &&
            poundedYamCookingManager.yamState ==
                PoundedYamCookingManager.PoundedYamState.Served &&
            yamStateLastFrame !=
                PoundedYamCookingManager.PoundedYamState.Served
        )
        {
            selectedFood = 2;
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

            // Leaving: face the exit, not a hard-coded flag.
            Face(exitX - transform.position.x);

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

        // Served or not: face the way the customer is about to walk.
        Face(exitX - transform.position.x);

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
            
            // Unlocks the next country in the Food Menu.
            LevelProgress.MarkCompleted(2);

            Debug.Log(
                "LEVEL TWO COMPLETE → LEVEL COMPLETE TWO"
            );

            SceneManager.LoadScene(
                "LevelComplete Two"
            );
        }
    }
}