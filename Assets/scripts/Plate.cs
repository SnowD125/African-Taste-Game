using UnityEngine;
using System.Collections;

public class Plate : MonoBehaviour
{
    [Header("Plate Foods")]
    public GameObject[] foodsOnPlate;

    [Header("References")]
    public CustomerMenuManager customerMenuManager;
    public CookingManager cookingManager;
    public VegCookingManager vegCookingManager;

    private bool isClickable = true;
    private Collider2D plateCollider;


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        plateCollider = GetComponent<Collider2D>();

        if (customerMenuManager == null)
        {
            customerMenuManager =
                Object.FindFirstObjectByType<CustomerMenuManager>();
        }

        if (cookingManager == null)
        {
            cookingManager =
                Object.FindFirstObjectByType<CookingManager>();
        }

        if (vegCookingManager == null)
        {
            vegCookingManager =
                Object.FindFirstObjectByType<VegCookingManager>();
        }

        isClickable = true;

        Debug.Log(
            "🍽 PLATE 1 READY — Collider = "
            + (plateCollider != null)
        );
    }


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        if (!isClickable)
            return;

        if (plateCollider == null)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError(
                "❌ MAIN CAMERA NOT FOUND!"
            );

            return;
        }


        Vector3 mousePosition =
            Input.mousePosition;

        mousePosition.z =
            Mathf.Abs(
                cam.transform.position.z
            );

        Vector3 worldPosition =
            cam.ScreenToWorldPoint(
                mousePosition
            );


        // =====================================================
        // CHECK IF MOUSE IS INSIDE PLATE COLLIDER
        // =====================================================

        if (!plateCollider.OverlapPoint(
            new Vector2(
                worldPosition.x,
                worldPosition.y
            )
        ))
        {
            return;
        }


        Debug.Log(
            "🍽 PLATE 1 CLICK DETECTED!"
        );


        ServePlate();
    }


    // =========================================================
    // ALSO KEEP ONMOUSEDOWN
    // =========================================================

    void OnMouseDown()
    {
        Debug.Log(
            "🍽 PLATE 1 OnMouseDown DETECTED!"
        );

        ServePlate();
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetPlate()
    {
        isClickable = true;

        foreach (GameObject food in foodsOnPlate)
        {
            if (food != null)
                food.SetActive(false);
        }
    }


    // =========================================================
    // SERVE PLATE
    // =========================================================

    private void ServePlate()
    {
        if (!isClickable)
            return;

        if (customerMenuManager == null)
        {
            Debug.LogError(
                "❌ CUSTOMER MENU MANAGER NOT FOUND!"
            );

            return;
        }


        Debug.Log(
            "🍽 PLATE 1 SERVE START"
            + " | selectedFood="
            + customerMenuManager.selectedFood
            + " | combo="
            + customerMenuManager.isComboOrder
        );


        // =====================================================
        // PLATE 1 DOES NOT SERVE COMBO
        // =====================================================

        if (customerMenuManager.isComboOrder)
        {
            Debug.Log(
                "PLATE 1 — COMBO ORDER, USE PLATE 2"
            );

            return;
        }


        // =====================================================
        // VALID FOOD
        // =====================================================

        if (
            customerMenuManager.selectedFood != 0 &&
            customerMenuManager.selectedFood != 1 &&
            customerMenuManager.selectedFood != 2
        )
        {
            Debug.Log(
                "PLATE 1 — NO VALID ORDER"
            );

            return;
        }


        isClickable = false;


        // =====================================================
        // HIDE FOOD ON PLATE
        // =====================================================

        foreach (GameObject food in foodsOnPlate)
        {
            if (food != null)
                food.SetActive(false);
        }


        // =====================================================
        // HIDE VEG FOOD
        // =====================================================

        if (vegCookingManager != null)
        {
            vegCookingManager.HidePlateFood();
        }


        // =====================================================
        // HIDE ORDER IMAGE
        // =====================================================

        if (customerMenuManager.foodImage != null)
        {
            customerMenuManager.foodImage
                .gameObject
                .SetActive(false);
        }


        // =====================================================
        // SERVED TICK
        // =====================================================

        if (customerMenuManager.servedTick != null)
        {
            customerMenuManager.servedTick
                .SetActive(true);
        }


        // =====================================================
        // SERVED TEXT
        // =====================================================

        if (customerMenuManager.servedText != null)
        {
            customerMenuManager.servedText
                .SetActive(true);
        }


        StartCoroutine(
            ShowServedThenNotifyCustomer()
        );
    }


    // =========================================================
    // SERVED ROUTINE
    // =========================================================

    private IEnumerator ShowServedThenNotifyCustomer()
    {
        yield return new WaitForSeconds(2f);


        // =====================================================
        // HIDE SERVED UI
        // =====================================================

        if (customerMenuManager.servedTick != null)
        {
            customerMenuManager.servedTick
                .SetActive(false);
        }

        if (customerMenuManager.servedText != null)
        {
            customerMenuManager.servedText
                .SetActive(false);
        }


        // =====================================================
        // HIDE ORDER CANVAS
        // =====================================================

        if (customerMenuManager.orderCanvas != null)
        {
            customerMenuManager.orderCanvas
                .SetActive(false);
        }


        // =====================================================
        // RESET COOKING
        // =====================================================

        if (cookingManager != null)
        {
            cookingManager.OnFoodServed();
        }


        // =====================================================
        // FIND ACTIVE CUSTOMER
        // =====================================================

        FirstCustomer[] allCustomers =
            Object.FindObjectsByType<FirstCustomer>(
                FindObjectsSortMode.None
            );


        FirstCustomer activeCustomer = null;


        foreach (FirstCustomer c in allCustomers)
        {
            if (c == null)
                continue;

            if (!c.gameObject.activeInHierarchy)
                continue;

            if (c.IsLeaving)
                continue;


            activeCustomer = c;

            break;
        }


        // =====================================================
        // SERVE CUSTOMER
        // =====================================================

        if (activeCustomer != null)
        {
            Debug.Log(
                "🍽 PLATE 1 → SERVING CUSTOMER: "
                + activeCustomer.gameObject.name
            );

            activeCustomer.ServeCustomer();
        }
        else
        {
            Debug.LogWarning(
                "⚠️ PLATE 1 → NO ACTIVE CUSTOMER FOUND!"
            );
        }


        // =====================================================
        // RESET ORDER
        // =====================================================

        customerMenuManager.selectedFood = -1;
        customerMenuManager.isComboOrder = false;


        // =====================================================
        // READY FOR NEXT CUSTOMER
        // =====================================================

        isClickable = true;
    }
}