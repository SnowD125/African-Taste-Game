using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomerMenuManager2 : MonoBehaviour
{
    // =========================================================
    // MENU PANEL
    // =========================================================

    [Header("Menu Panel")]
    public GameObject menuPanel;

    public GameObject jollofTick;
    public GameObject egusiTick;
    public GameObject poundedYamTick;


    // =========================================================
    // ORDER CANVAS
    // =========================================================

    [Header("Order Canvas")]
    public GameObject orderCanvas;

    public Image foodImage;

    public Sprite jollofSprite;
    public Sprite egusiSprite;
    public Sprite poundedYamSprite;


    // =========================================================
    // SERVED UI
    // =========================================================

    [Header("Served UI")]
    public GameObject servedTick;
    public GameObject servedText;


    // =========================================================
    // PLATE
    // =========================================================

    [Header("Jollof Plate")]
    public GameObject plate1;


    // =========================================================
    // COOKING MANAGERS
    // =========================================================

    [Header("Cooking Managers")]

    public JollofCookingManager jollofCookingManager;

    public EgusiCookingManager egusiCookingManager;


    // =========================================================
    // OPTIONAL MANAGERS
    // =========================================================

    [Header("Other Cooking System")]
    public MonoBehaviour poundedYamCookingManager;


    // =========================================================
    // SELECTED FOOD
    // =========================================================

    [HideInInspector]
    public int selectedFood = -1;


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
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetAll()
    {
        CancelInvoke(nameof(HideMenu));

        menuPanel?.SetActive(false);

        orderCanvas?.SetActive(false);

        servedTick?.SetActive(false);

        servedText?.SetActive(false);

        jollofTick?.SetActive(false);

        egusiTick?.SetActive(false);

        poundedYamTick?.SetActive(false);

        if (foodImage != null)
        {
            foodImage.gameObject.SetActive(false);
        }

        selectedFood = -1;
    }


    // =========================================================
    // SHOW MENU
    // =========================================================

    public void ShowMenu()
    {
        CancelInvoke(nameof(HideMenu));

        jollofTick?.SetActive(false);

        egusiTick?.SetActive(false);

        poundedYamTick?.SetActive(false);

        orderCanvas?.SetActive(false);

        selectedFood = -1;

        menuPanel?.SetActive(true);

        Debug.Log("CUSTOMER MENU SHOWN");
    }


    // =========================================================
    // SELECT JOLLOF
    // =========================================================

    public void SelectJollof()
    {
        selectedFood = 0;

        Debug.Log("================================");
        Debug.Log("CUSTOMER SELECTED: JOLLOF RICE");
        Debug.Log("================================");


        jollofTick?.SetActive(true);

        egusiTick?.SetActive(false);

        poundedYamTick?.SetActive(false);


        CancelInvoke(nameof(HideMenu));

        Invoke(nameof(HideMenu), 5f);
    }


    // =========================================================
    // SELECT EGUSI
    // =========================================================

    public void SelectEgusi()
    {
        selectedFood = 1;

        Debug.Log("================================");
        Debug.Log("CUSTOMER SELECTED: EGUSI SOUP");
        Debug.Log("================================");


        egusiTick?.SetActive(true);

        jollofTick?.SetActive(false);

        poundedYamTick?.SetActive(false);


        // -----------------------------------------------------
        // MAKE SURE EGUSI MANAGER EXISTS
        // -----------------------------------------------------

        if (egusiCookingManager == null)
        {
            egusiCookingManager =
                Object.FindFirstObjectByType<EgusiCookingManager>();
        }


        if (egusiCookingManager != null)
        {
            Debug.Log(
                "EGUSI COOKING MANAGER FOUND"
            );

            // RESET EGUSI FLOW
            egusiCookingManager.ResetEgusiCooking();

            Debug.Log(
                "EGUSI COOKING SYSTEM READY"
            );
        }
        else
        {
            Debug.LogError(
                "EGUSI COOKING MANAGER NOT FOUND!"
            );
        }


        CancelInvoke(nameof(HideMenu));

        Invoke(nameof(HideMenu), 5f);
    }


    // =========================================================
    // SELECT POUNDED YAM
    // =========================================================

    public void SelectPoundedYam()
    {
        selectedFood = 2;

        Debug.Log(
            "CUSTOMER SELECTED: POUNDED YAM"
        );


        poundedYamTick?.SetActive(true);

        jollofTick?.SetActive(false);

        egusiTick?.SetActive(false);


        CancelInvoke(nameof(HideMenu));

        Invoke(nameof(HideMenu), 5f);
    }


    // =========================================================
    // HIDE MENU / SHOW ORDER
    // =========================================================

    void HideMenu()
    {
        menuPanel?.SetActive(false);

        orderCanvas?.SetActive(true);


        if (foodImage != null)
        {
            foodImage.gameObject.SetActive(true);


            if (selectedFood == 0)
            {
                foodImage.sprite =
                    jollofSprite;

                Debug.Log(
                    "ORDER IMAGE: JOLLOF RICE"
                );
            }
            else if (selectedFood == 1)
            {
                foodImage.sprite =
                    egusiSprite;

                Debug.Log(
                    "ORDER IMAGE: EGUSI SOUP"
                );
            }
            else if (selectedFood == 2)
            {
                foodImage.sprite =
                    poundedYamSprite;

                Debug.Log(
                    "ORDER IMAGE: POUNDED YAM"
                );
            }
        }


        // -----------------------------------------------------
        // IMPORTANT
        // -----------------------------------------------------
        // EGUSI MANAGER IS ALREADY RESET WHEN
        // SelectEgusi() WAS CLICKED.
        //
        // HAPA HATUITI JOLLOF MANAGER.
        // -----------------------------------------------------


        Debug.Log(
            "MENU HIDDEN — ORDER DISPLAYED"
        );
    }


    // =========================================================
    // SERVE JOLLOF PLATE
    // =========================================================

    public void ServePlate1()
    {
        if (selectedFood != 0)
        {
            Debug.Log(
                "ServePlate1 ignored — customer did not order Jollof."
            );

            return;
        }


        plate1?.SetActive(false);

        foodImage?.gameObject.SetActive(false);

        servedTick?.SetActive(true);

        servedText?.SetActive(true);

        StartCoroutine(ServeRoutine());
    }


    // =========================================================
    // SERVE ROUTINE
    // =========================================================

    private IEnumerator ServeRoutine()
    {
        yield return new WaitForSeconds(2f);


        servedTick?.SetActive(false);

        servedText?.SetActive(false);

        orderCanvas?.SetActive(false);


        FirstCustomer2[] allCustomers =
            Object.FindObjectsByType<FirstCustomer2>(
                FindObjectsSortMode.None
            );


        FirstCustomer2 activeCustomer = null;


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


        if (activeCustomer != null)
        {
            activeCustomer.ServeCustomer();

            Debug.Log(
                "CUSTOMER SERVED"
            );
        }
        else
        {
            Debug.LogWarning(
                "NO ACTIVE CUSTOMER FOUND"
            );
        }


        selectedFood = -1;
    }
}