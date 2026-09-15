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
    [Tooltip("SUPERSEDED. Orientation now follows the direction of travel " +
             "(see Face). Kept only so existing scene data is not lost.")]
    public bool flipXWhenCarrying = false;
    [Tooltip("Not used by any code path.")]
    public float carryingYOffset = 10f;

    [Header("Orientation")]
    [Tooltip("Tick ONLY if the artwork is drawn facing RIGHT. The Level One " +
             "customer art faces the camera, so this stays off and the " +
             "customer is never mirrored while walking left.")]
    public bool artFacesRight = false;

    [Header("Carried Food")]
    [Tooltip("Shows the plate the customer walks away with. Found on this " +
             "GameObject if left empty.")]
    public CustomerCarry carry;

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

    // What the serving system actually put on the plate. Read once, in
    // ServeCustomer(), and never re-read afterwards.
    private LevelOneDish servedDish = LevelOneDish.None;

    // False when the customer walks out on a timeout. Used to decide whether
    // the abandoned order's food has to be cleared out of the kitchen.
    private bool wasServed = false;

    public bool IsLeaving => isLeaving;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (carry == null)
            carry = GetComponent<CustomerCarry>();
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
        servedDish = LevelOneDish.None;
        wasServed = false;

        if (carry != null)
            carry.Hide();

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

    // =========================================================
    // ORIENTATION
    //
    // The old code did `spriteRenderer.flipX = flipXWhenCarrying` the moment
    // the customer picked up its food. In the scene m_FlipX was 1 and
    // flipXWhenCarrying was 0, so the customer flipped to face right and then
    // walked LEFT to exitX = -60 -- the moonwalk. Facing is now derived from
    // the actual direction of travel, and from nothing else.
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

        wasServed = true;

        Debug.Log(
            $"🍛 SERVING CUSTOMER = {name} ✅"
        );

        // =====================================================
        // READ CUSTOMER ORDER BEFORE RESETTING ANYTHING
        // =====================================================

        int selectedFood = -1;

        // A single ingredient is NOT a valid Level One order: no plate, no coins.
        bool validOrder = false;

        if (customerOrder != null &&
            customerOrder.menuManager != null)
        {
            selectedFood =
                customerOrder.menuManager.selectedFood;

            // The plate latched this the instant it committed the serve, so a
            // pending Invoke(HideMenu, 5f) can no longer change it.
            validOrder = customerOrder.menuManager.isValidOrder;

            servedDish = customerOrder.menuManager.servedDish;

            if (servedDish == LevelOneDish.None)
                servedDish = customerOrder.menuManager.CurrentDish();

            Debug.Log(
                $"🍽️ {name} ORDER = selectedFood {selectedFood}, " +
                $"served dish = {servedDish}, valid combo = {validOrder}"
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

        if (!validOrder)
        {
            // Single ingredient: no carry state at all.
            animator.SetInteger(
                "carryingFood",
                0
            );

            Debug.Log(
                $"🚫 {name} → single ingredient, no plate and no coins."
            );
        }
        else if (selectedFood == 0)
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

        // The Wave state's ONLY exit transition requires isCarrying, which an
        // invalid order never sets -- entering it would leave the customer
        // stuck in the front-facing wave pose for the whole exit walk.
        // Skipping it lets isIdle=false take Idle -> Walking, so an unserved
        // customer walks out in the side-facing walk pose.
        if (validOrder)
        {
            animator.SetTrigger(
                "isHappy"
            );
        }

        // =====================================================
        // COINS
        // =====================================================

        float timeUsed =
            Time.time -
            serviceStartTime;

        if (validOrder && CoinManager.Instance != null)
        {
            CoinManager.Instance.AwardCoins(
                timeUsed,
                maxServiceTime
            );
        }

        // =====================================================
        // START CARRYING ANIMATION
        // =====================================================

        if (validOrder)
        {
            StartCoroutine(
                SetWalkWithFoodAfterDelay()
            );
        }

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

            // Leaving: face the exit, not a hard-coded flag.
            Face(exitX - transform.position.x);

            // Show the dish that was actually served.
            if (carry != null)
                carry.Show(servedDish);

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

        if (carry != null)
            carry.Hide();

        // =====================================================
        // ABANDONED ORDER — CLEAR THE KITCHEN
        //
        // Only when this customer leaves WITHOUT being served. A served
        // customer already reset cooking inside ServeCustomer(), so this
        // cannot touch a valid order.
        //
        // Reuses the existing reset methods rather than adding a second
        // reset system: ResetCookingState() clears the pot, the ready food
        // and both plates; ResetCooking() clears the whole pan pipeline.
        // Runs before the next customer is activated, and before the final
        // customer triggers Level Complete.
        // =====================================================

        if (!wasServed)
        {
            Debug.Log($"🧹 {name} left unserved — clearing the abandoned order.");

            cookingManager?.ResetCookingState();
            vegCookingManager?.ResetCooking();
        }

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
            
            // Unlocks the next country in the Food Menu.
            LevelProgress.MarkCompleted(1);

            SceneManager.LoadScene(
                "LevelComplete"
            );
        }
    }
}