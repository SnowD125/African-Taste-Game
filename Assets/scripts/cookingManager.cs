using UnityEngine;

public class CookingManager : MonoBehaviour
{
    [Header("Boiling Water (Idle State)")]
    public GameObject boilingWater;

    [Header("Ugali Cooking")]
    public GameObject ugaliCooking;
    public GameObject readyUgali;
    public GameObject readyUgali2;

    [Header("Tembele Cooking")]
    public GameObject tembeleCooking;
    public GameObject readyTembele;

    [Header("Settings")]
    public float cookingTime = 2f;
    public CustomerMenuManager menuManager;

    [Header("Plates")]
    public Plate plate1Script;
    public Plate2 plate2Script;

    bool isUgaliCooking = false;
    bool isUgaliReady = false;
    // True once the player has taken the ugali out of the pot onto the plate.
    // Equivalent to VegCookingManager's CookState.Served: stops a second pot
    // click re-serving the same ugali.
    bool isUgaliOnPlate = false;
    bool isTembeleCooking = false;
    bool isTembeleReady = false;

    void Start()
    {
        Debug.Log("CookingManager Start — calling ResetCookingState");
        ResetCookingState();
    }

    public void ResetCookingState()
    {
        Debug.Log("ResetCookingState called");
        isUgaliCooking = false;
        isUgaliReady = false;
        isUgaliOnPlate = false;
        isTembeleCooking = false;
        isTembeleReady = false;

        ugaliCooking?.SetActive(false);
        tembeleCooking?.SetActive(false);
        readyUgali?.SetActive(false);
        readyUgali2?.SetActive(false);
        readyTembele?.SetActive(false);

        boilingWater?.SetActive(true);

        CancelInvoke();

        plate1Script?.ResetPlate();
        plate2Script?.ResetPlate();

        Debug.Log("ResetCookingState done — isUgaliReady=" + isUgaliReady);
    }

    public void StartUgaliCooking()
    {
        Debug.Log($"StartUgaliCooking called — isUgaliCooking={isUgaliCooking}, isUgaliReady={isUgaliReady}");

        if (isUgaliCooking || isUgaliReady)
        {
            Debug.Log("BLOCKED — returning early");
            return;
        }

        isUgaliCooking = true;
        boilingWater?.SetActive(false);
        ugaliCooking?.SetActive(true);
        Debug.Log("Ugali cooking started successfully");

        Invoke(nameof(UgaliCooked), cookingTime);
    }

    /// <summary>
    /// The cook timer finishing only marks the ugali READY -- it stays visible in
    /// the pot. This mirrors VegCookingManager, where the progress bar filling
    /// sets CookState.ReadyToServe and the food only leaves the pan when the
    /// player clicks it (TryServePan).
    /// </summary>
    void UgaliCooked()
    {
        isUgaliCooking = false;
        isUgaliReady = true;

        Debug.Log("Ugali is cooked — staying in the pot until the player clicks it.");
    }

    /// <summary>
    /// Player-driven: clicking the pot takes the cooked ugali out onto the plate.
    /// The ugali equivalent of VegCookingManager.TryServePan().
    /// </summary>
    public void TakeUgaliFromPot()
    {
        if (!isUgaliReady)
        {
            Debug.Log("Pot clicked — ugali is not ready yet.");
            return;
        }

        if (isUgaliOnPlate)
        {
            Debug.Log("Pot clicked — the ugali is already on the plate.");
            return;
        }

        isUgaliOnPlate = true;

        Debug.Log($"TakeUgaliFromPot — isComboOrder={menuManager?.isComboOrder}");

        ugaliCooking?.SetActive(false);
        boilingWater?.SetActive(true);

        if (menuManager == null)
        {
            Debug.LogError("menuManager IS NULL in TakeUgaliFromPot");
            return;
        }

        if (menuManager.isComboOrder)
        {
            Debug.Log("Combo order — sending ugali to plate2");
            menuManager.plate2?.SetActive(true);
            readyUgali2?.SetActive(true);
            readyUgali?.SetActive(false);
        }
        else
        {
            Debug.Log("Normal order — sending ugali to plate1");
            menuManager.plate1?.SetActive(true);
            readyUgali?.SetActive(true);
            readyUgali2?.SetActive(false);

            // 🔥 Force hide dagaaCook in case it is a child of plate1
            VegCookingManager vegManager = Object.FindFirstObjectByType<VegCookingManager>();
            vegManager?.ForceHideDagaa();
        }
    }

    /// <summary>Every pot click: start cooking, or take the cooked ugali out.</summary>
    public void ClickPot()
    {
        if (isUgaliReady || isUgaliOnPlate)
        {
            TakeUgaliFromPot();
            return;
        }

        StartUgaliCooking();
    }

    public void StartTembeleCooking()
    {
        Debug.Log($"StartTembeleCooking called — isTembeleCooking={isTembeleCooking}, isTembeleReady={isTembeleReady}");

        if (isTembeleCooking || isTembeleReady) return;

        isTembeleCooking = true;
        tembeleCooking?.SetActive(true);

        Invoke(nameof(FinishTembeleCooking), cookingTime);
    }

    void FinishTembeleCooking()
    {
        Debug.Log($"FinishTembeleCooking — isComboOrder={menuManager?.isComboOrder}");
        isTembeleCooking = false;
        isTembeleReady = true;

        tembeleCooking?.SetActive(false);

        if (menuManager != null && menuManager.isComboOrder)
        {
            Debug.Log("Tembele done — sending to plate2");
            menuManager.plate2?.SetActive(true);
            readyTembele?.SetActive(true);
        }
    }

    public void OnFoodServed()
    {
        Debug.Log("OnFoodServed called — resetting cooking state");
        ResetCookingState();
    }
}