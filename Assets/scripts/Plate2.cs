using UnityEngine;
using System.Collections;

public class Plate2 : MonoBehaviour
{
    [Header("Plate Foods")]
    public GameObject[] foodsOnPlate;

    [Header("References")]
    public CustomerMenuManager customerMenuManager;
    public CookingManager cookingManager;
    public VegCookingManager vegCookingManager;

    private bool isClickable = true;
    private bool isUgaliTembelePlate = false;

    void Start()
    {
        if (customerMenuManager == null)
            customerMenuManager =
                Object.FindFirstObjectByType<CustomerMenuManager>();

        if (cookingManager == null)
            cookingManager =
                Object.FindFirstObjectByType<CookingManager>();

        if (vegCookingManager == null)
            vegCookingManager =
                Object.FindFirstObjectByType<VegCookingManager>();
    }

    public void PrepareForUgaliTembele()
    {
        isClickable = true;
        isUgaliTembelePlate = true;

        Debug.Log(
            "🍛 Plate 2 prepared for Ugali + Tembele ✅"
        );
    }

    public void PrepareForTembeleOnly()
    {
        isClickable = true;
        isUgaliTembelePlate = false;

        Debug.Log(
            "🥬 Plate 2 prepared for Tembele only"
        );
    }

    public void ResetPlate()
    {
        isClickable = true;
        isUgaliTembelePlate = false;

        foreach (GameObject food in foodsOnPlate)
        {
            if (food != null)
                food.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        Debug.Log("🔥 PLATE 2 CLICKED!");

        // =====================================================
        // CLICK CHECK
        // =====================================================

        if (!isClickable)
        {
            Debug.Log(
                "Plate 2 ignored — isClickable = false"
            );

            return;
        }

        // =====================================================
        // CUSTOMER MENU MANAGER
        // =====================================================

        if (customerMenuManager == null)
        {
            customerMenuManager =
                Object.FindFirstObjectByType<CustomerMenuManager>();
        }

        if (customerMenuManager == null)
        {
            Debug.LogWarning(
                "❌ Plate 2 — CustomerMenuManager is NULL!"
            );

            return;
        }

        Debug.Log(
            $"Plate2 → " +
            $"isUgaliTembelePlate={isUgaliTembelePlate}, " +
            $"isComboOrder={customerMenuManager.isComboOrder}, " +
            $"selectedFood={customerMenuManager.selectedFood}"
        );

        // =====================================================
        // CHECK WHETHER THIS PLATE IS VALID
        // =====================================================

        if (isUgaliTembelePlate)
        {
            Debug.Log(
                "✅ Plate 2 accepted → Ugali + Tembele"
            );
        }
        else if (
            customerMenuManager.selectedFood == 1 ||
            customerMenuManager.isComboOrder
        )
        {
            Debug.Log(
                "✅ Plate 2 accepted → Tembele order"
            );
        }
        else
        {
            Debug.Log(
                "❌ Plate 2 rejected — wrong food/order"
            );

            return;
        }

        isClickable = false;

        // =====================================================
        // LATCH WHAT WAS ACTUALLY SERVED
        //
        // isUgaliTembelePlate is set by the cooking system itself
        // (PrepareForUgaliTembele / PrepareForTembeleOnly), so it is the most
        // truthful record in the project of what is physically on this plate.
        // =====================================================

        customerMenuManager.LatchServedDish(
            isUgaliTembelePlate
                ? LevelOneDish.UgaliTembele
                : LevelOneDish.PotatoLeaves
        );

        // =====================================================
        // HIDE FOOD ON PLATE
        // =====================================================

        foreach (GameObject food in foodsOnPlate)
        {
            if (food != null)
                food.SetActive(false);
        }

        // =====================================================
        // HIDE COOKED TEMBELE
        // =====================================================

        if (vegCookingManager != null)
            vegCookingManager.HidePlateFood();

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
        // SHOW SERVED UI -- VALID COMBO ONLY
        //
        // A single ingredient is not a successful order, so it gets no
        // confirmation UI, exactly as it gets no plate and no coins.
        // =====================================================

        if (customerMenuManager.isValidOrder)
        {
            if (customerMenuManager.servedTick != null)
            {
                customerMenuManager.servedTick
                    .SetActive(true);
            }

            if (customerMenuManager.servedText != null)
            {
                customerMenuManager.servedText
                    .SetActive(true);
            }
        }
        else
        {
            // No box at all for an invalid order -- not even an empty one.
            customerMenuManager.HideOrderBoxForInvalidOrder();
        }

        // =====================================================
        // GET CURRENT CUSTOMER DIRECTLY
        // =====================================================

        FirstCustomer currentCustomer =
            FirstCustomer.CurrentCustomer;

        if (currentCustomer != null)
        {
            Debug.Log(
                $"🍛 SERVING CURRENT CUSTOMER → " +
                $"{currentCustomer.name} ✅"
            );

            // IMPORTANT:
            // Serve the exact customer currently waiting.
            currentCustomer.ServeCustomer();
        }
        else
        {
            Debug.LogError(
                "❌ Plate 2: CurrentCustomer is NULL!"
            );
        }

        // =====================================================
        // COOKING RESET
        // =====================================================

        if (cookingManager != null)
            cookingManager.OnFoodServed();

        // =====================================================
        // HIDE SERVED UI AFTER 2 SECONDS
        // =====================================================

        StartCoroutine(
            HideServedUI()
        );
    }

    private IEnumerator HideServedUI()
    {
        yield return new WaitForSeconds(2f);

        if (customerMenuManager != null)
        {
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

            if (customerMenuManager.orderCanvas != null)
            {
                customerMenuManager.orderCanvas
                    .SetActive(false);
            }
        }

        isUgaliTembelePlate = false;
    }
}