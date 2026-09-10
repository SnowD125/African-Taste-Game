using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomerMenuManager3 : MonoBehaviour
{
    [Header("Menu Panel")]
    public GameObject menuPanel;

    public GameObject waakyeTick;
    public GameObject bankuTilapiaTick;
    public GameObject fufuSoupTick;


    [Header("Order Canvas")]
    public GameObject orderCanvas;

    public Image foodImage;

    public Sprite waakyeSprite;
    public Sprite bankuTilapiaSprite;
    public Sprite fufuSoupSprite;


    [Header("Served UI")]
    public GameObject servedTick;
    public GameObject servedText;


    [Header("Plate")]
    public GameObject plate1;


    [HideInInspector]
    public int selectedFood = -1;


    // =========================================================
    // FOOD TYPES
    // =========================================================

    public enum GhanaOrder
    {
        None = -1,
        Waakye = 0,
        BankuTilapia = 1,
        FufuLightSoup = 2
    }


    // =========================================================
    // AWAKE
    // =========================================================

    void Awake()
    {
        SetupReferences();
        ResetAll();
    }


    // =========================================================
    // SETUP REFERENCES
    // =========================================================

    private void SetupReferences()
    {
        // References are assigned from the Unity Inspector.
    }


    // =========================================================
    // RESET EVERYTHING
    // =========================================================

    public void ResetAll()
    {
        CancelInvoke(nameof(HideMenu));

        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (orderCanvas != null)
            orderCanvas.SetActive(false);

        if (servedTick != null)
            servedTick.SetActive(false);

        if (servedText != null)
            servedText.SetActive(false);

        if (waakyeTick != null)
            waakyeTick.SetActive(false);

        if (bankuTilapiaTick != null)
            bankuTilapiaTick.SetActive(false);

        if (fufuSoupTick != null)
            fufuSoupTick.SetActive(false);

        if (foodImage != null)
            foodImage.gameObject.SetActive(false);

        selectedFood = -1;
    }


    // =========================================================
    // SHOW MENU
    // =========================================================

    public void ShowMenu()
    {
        CancelInvoke(nameof(HideMenu));

        if (waakyeTick != null)
            waakyeTick.SetActive(false);

        if (bankuTilapiaTick != null)
            bankuTilapiaTick.SetActive(false);

        if (fufuSoupTick != null)
            fufuSoupTick.SetActive(false);

        if (orderCanvas != null)
            orderCanvas.SetActive(false);

        selectedFood = -1;

        if (menuPanel != null)
            menuPanel.SetActive(true);

        Debug.Log("LEVEL 3 GHANA CUSTOMER MENU SHOWN");
    }


    // =========================================================
    // SELECT WAAKYE
    // =========================================================

    public void SelectWaakye()
    {
        selectedFood = 0;

        if (waakyeTick != null)
            waakyeTick.SetActive(true);

        if (bankuTilapiaTick != null)
            bankuTilapiaTick.SetActive(false);

        if (fufuSoupTick != null)
            fufuSoupTick.SetActive(false);

        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);

        Debug.Log("CUSTOMER ORDERED WAAKYE");
    }


    // =========================================================
    // SELECT BANKU & TILAPIA
    // =========================================================

    public void SelectBankuTilapia()
    {
        selectedFood = 1;

        if (bankuTilapiaTick != null)
            bankuTilapiaTick.SetActive(true);

        if (waakyeTick != null)
            waakyeTick.SetActive(false);

        if (fufuSoupTick != null)
            fufuSoupTick.SetActive(false);

        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);

        Debug.Log("CUSTOMER ORDERED BANKU & TILAPIA");
    }


    // =========================================================
    // SELECT FUFU & LIGHT SOUP
    // =========================================================

    public void SelectFufuLightSoup()
    {
        selectedFood = 2;

        if (fufuSoupTick != null)
            fufuSoupTick.SetActive(true);

        if (waakyeTick != null)
            waakyeTick.SetActive(false);

        if (bankuTilapiaTick != null)
            bankuTilapiaTick.SetActive(false);

        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);

        Debug.Log("CUSTOMER ORDERED FUFU & LIGHT SOUP");
    }


    // =========================================================
    // HIDE MENU / SHOW ORDER
    // =========================================================

    void HideMenu()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (orderCanvas != null)
            orderCanvas.SetActive(true);

        if (foodImage != null)
        {
            foodImage.gameObject.SetActive(true);

            if (selectedFood == 0)
            {
                foodImage.sprite = waakyeSprite;
            }
            else if (selectedFood == 1)
            {
                foodImage.sprite = bankuTilapiaSprite;
            }
            else if (selectedFood == 2)
            {
                foodImage.sprite = fufuSoupSprite;
            }
        }

        Debug.Log(
            $"LEVEL 3 ORDER DISPLAYED = {selectedFood}"
        );
    }


    // =========================================================
    // SERVE PLATE
    // =========================================================

    public void ServePlate1()
    {
        // -----------------------------------------------------
        // CHECK THAT A FOOD WAS ORDERED
        // -----------------------------------------------------

        if (selectedFood < 0)
        {
            Debug.LogWarning(
                "ServePlate1 ignored — customer has no valid order."
            );

            return;
        }


        // -----------------------------------------------------
        // HIDE PLATE
        // -----------------------------------------------------

        if (plate1 != null)
            plate1.SetActive(false);


        // -----------------------------------------------------
        // HIDE ORDER IMAGE
        // -----------------------------------------------------

        if (foodImage != null)
            foodImage.gameObject.SetActive(false);


        // -----------------------------------------------------
        // SHOW SERVED UI
        // -----------------------------------------------------

        if (servedTick != null)
            servedTick.SetActive(true);

        if (servedText != null)
            servedText.SetActive(true);


        Debug.Log(
            $"🍽️ LEVEL 3 FOOD SERVED = {selectedFood}"
        );


        // -----------------------------------------------------
        // START SERVE ROUTINE
        // -----------------------------------------------------

        StartCoroutine(ServeRoutine());
    }


    // =========================================================
    // SERVE ROUTINE
    // =========================================================

    private IEnumerator ServeRoutine()
    {
        yield return new WaitForSeconds(2f);


        // -----------------------------------------------------
        // HIDE SERVED UI
        // -----------------------------------------------------

        if (servedTick != null)
            servedTick.SetActive(false);

        if (servedText != null)
            servedText.SetActive(false);

        if (orderCanvas != null)
            orderCanvas.SetActive(false);


        // -----------------------------------------------------
        // FIND ACTIVE LEVEL 3 CUSTOMER
        // -----------------------------------------------------

        FirstCustomer3[] allCustomers =
            Object.FindObjectsByType<FirstCustomer3>(
                FindObjectsSortMode.None
            );

        FirstCustomer3 activeCustomer = null;


        foreach (var c in allCustomers)
        {
            if (
                c.gameObject.activeInHierarchy &&
                !c.IsLeaving
            )
            {
                activeCustomer = c;
                break;
            }
        }


        // -----------------------------------------------------
        // SERVE CUSTOMER
        // -----------------------------------------------------

        if (activeCustomer != null)
        {
            activeCustomer.ServeCustomer();
        }
        else
        {
            Debug.LogWarning(
                "NO ACTIVE LEVEL 3 CUSTOMER FOUND"
            );
        }


        // -----------------------------------------------------
        // DO NOT RESET selectedFood HERE
        // -----------------------------------------------------
        //
        // FirstCustomer3 reads selectedFood inside
        // ServeCustomer() before the customer leaves.
        //
        // Therefore we intentionally keep selectedFood.
        //
    }
}