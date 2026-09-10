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
        isTembeleCooking = false;
        isTembeleReady = false;

        ugaliCooking?.SetActive(false);
        tembeleCooking?.SetActive(false);
        readyUgali?.SetActive(false);
        readyUgali2?.SetActive(false);
        readyTembele?.SetActive(false);

        boilingWater?.SetActive(true);

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

        Invoke(nameof(FinishUgaliCooking), cookingTime);
    }

    void FinishUgaliCooking()
    {
        Debug.Log($"FinishUgaliCooking — isComboOrder={menuManager?.isComboOrder}");
        isUgaliCooking = false;
        isUgaliReady = true;

        ugaliCooking?.SetActive(false);
        boilingWater?.SetActive(true);

        if (menuManager == null)
        {
            Debug.LogError("menuManager IS NULL in FinishUgaliCooking");
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