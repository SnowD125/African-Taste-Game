using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomerMenuManager4 : MonoBehaviour
{
    [Header("Menu Panel")]
    public GameObject menuPanel;

    public GameObject tagineTick;
    public GameObject couscousTick;
    public GameObject pastillaTick;
    public GameObject hariraSoupTick;


    [Header("Order Canvas")]
    public GameObject orderCanvas;

    public Image foodImage;

    public Sprite tagineSprite;
    public Sprite couscousSprite;
    public Sprite pastillaSprite;
    public Sprite hariraSoupSprite;


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

    public enum MoroccoOrder
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

        if (tagineTick != null)
            tagineTick.SetActive(false);

        if (couscousTick != null)
            couscousTick.SetActive(false);

        if (pastillaTick != null)
            pastillaTick.SetActive(false);

        if (hariraSoupTick != null)
            hariraSoupTick.SetActive(false);

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

        if (tagineTick != null)
            tagineTick.SetActive(false);

        if (couscousTick != null)
            couscousTick.SetActive(false);

        if (pastillaTick != null)
            pastillaTick.SetActive(false);

        if (hariraSoupTick != null)
            hariraSoupTick.SetActive(false);

        if (orderCanvas != null)
            orderCanvas.SetActive(false);

        selectedFood = -1;

        if (menuPanel != null)
            menuPanel.SetActive(true);

        Debug.Log("LEVEL 4 MOROCCO CUSTOMER MENU SHOWN");
    }


    // =========================================================
    // SELECT TAGINE
    // =========================================================

    public void SelectTagine()
    {
        selectedFood = 0;

        if (tagineTick != null)
            tagineTick.SetActive(true);

        if (couscousTick != null)
            couscousTick.SetActive(false);

        if (pastillaTick != null)
            pastillaTick.SetActive(false);

        if (hariraSoupTick != null)
            hariraSoupTick.SetActive(false);

        if (hariraSoupTick != null)
            hariraSoupTick.SetActive(false);

        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);

        Debug.Log("CUSTOMER ORDERED TAGINE");
    }


    // =========================================================
    // SELECT COUSCOUS
    // =========================================================

    public void SelectCouscous()
    {
        selectedFood = 1;

        if (couscousTick != null)
            couscousTick.SetActive(true);

        if (tagineTick != null)
            tagineTick.SetActive(false);

        if (pastillaTick != null)
            pastillaTick.SetActive(false);

        if (hariraSoupTick != null)
            hariraSoupTick.SetActive(false);

        if (hariraSoupTick != null)
            hariraSoupTick.SetActive(false);

        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);

        Debug.Log("CUSTOMER ORDERED COUSCOUS");
    }


    // =========================================================
    // SELECT PASTILLA
    // =========================================================

    public void SelectPastilla()
    {
        selectedFood = 2;

        if (pastillaTick != null)
            pastillaTick.SetActive(true);

        if (tagineTick != null)
            tagineTick.SetActive(false);

        if (couscousTick != null)
            couscousTick.SetActive(false);

        if (hariraSoupTick != null)
            hariraSoupTick.SetActive(false);

        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);

        Debug.Log("CUSTOMER ORDERED PASTILLA");
    }


    // =========================================================
    // SELECT HARIRA SOUP
    // =========================================================

    public void SelectHariraSoup()
    {
        selectedFood = 3;

        if (hariraSoupTick != null)
            hariraSoupTick.SetActive(true);

        if (tagineTick != null)
            tagineTick.SetActive(false);

        if (couscousTick != null)
            couscousTick.SetActive(false);

        if (pastillaTick != null)
            pastillaTick.SetActive(false);

        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);

        Debug.Log("CUSTOMER ORDERED HARIRA SOUP");
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
                foodImage.sprite = tagineSprite;
            }
            else if (selectedFood == 1)
            {
                foodImage.sprite = couscousSprite;
            }
            else if (selectedFood == 2)
            {
                foodImage.sprite = pastillaSprite;
            }
            else if (selectedFood == 3)
            {
                foodImage.sprite = hariraSoupSprite;
            }
        }

        Debug.Log(
            $"LEVEL 4 ORDER DISPLAYED = {selectedFood}"
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
            $"🍽️ LEVEL 4 FOOD SERVED = {selectedFood}"
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
        // FIND ACTIVE LEVEL 4 CUSTOMER
        // -----------------------------------------------------

        FirstCustomer4[] allCustomers =
            Object.FindObjectsByType<FirstCustomer4>(
                FindObjectsSortMode.None
            );

        FirstCustomer4 activeCustomer = null;


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
                "NO ACTIVE LEVEL 4 CUSTOMER FOUND"
            );
        }


        // -----------------------------------------------------
        // DO NOT RESET selectedFood HERE
        // -----------------------------------------------------
        //
        // FirstCustomer4 reads selectedFood inside
        // ServeCustomer() before the customer leaves.
        //
        // Therefore we intentionally keep selectedFood.
        //
    }
}