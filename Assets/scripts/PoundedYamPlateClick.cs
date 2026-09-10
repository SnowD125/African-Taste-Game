using UnityEngine;
using System.Collections;

public class PoundedYamPlateClick : MonoBehaviour
{
    [Header("Pounded Yam Manager")]
    public PoundedYamCookingManager poundedYamManager;

    [Header("Order Panel")]
    public CustomerMenuManager2 customerMenuManager;

    private bool isClickable = true;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (poundedYamManager == null)
        {
            poundedYamManager =
                Object.FindFirstObjectByType<PoundedYamCookingManager>();
        }

        if (customerMenuManager == null)
        {
            customerMenuManager =
                Object.FindFirstObjectByType<CustomerMenuManager2>();
        }
    }


    // =========================================================
    // CLICK
    // =========================================================

    private void OnMouseDown()
    {
        OnClick();
    }


    public void OnClick()
    {
        if (!isClickable)
            return;


        // -----------------------------------------------------
        // FIND POUNDED YAM MANAGER
        // -----------------------------------------------------

        if (poundedYamManager == null)
        {
            poundedYamManager =
                Object.FindFirstObjectByType<PoundedYamCookingManager>();
        }


        // -----------------------------------------------------
        // FIND CUSTOMER MENU MANAGER
        // -----------------------------------------------------

        if (customerMenuManager == null)
        {
            customerMenuManager =
                Object.FindFirstObjectByType<CustomerMenuManager2>();
        }


        if (poundedYamManager == null)
        {
            Debug.LogError(
                "POUNDED YAM MANAGER NOT FOUND!"
            );

            return;
        }


        if (customerMenuManager == null)
        {
            Debug.LogError(
                "CUSTOMER MENU MANAGER 2 NOT FOUND!"
            );

            return;
        }


        // =====================================================
        // CHECK PLATE IS READY
        // =====================================================

        if (
            poundedYamManager.currentStep != 5 ||
            poundedYamManager.yamState !=
            PoundedYamCookingManager.PoundedYamState.ReadyToServe
        )
        {
            Debug.Log(
                "PLATE CLICK IGNORED - POUNDED YAM NOT READY"
            );

            return;
        }


        isClickable = false;


        Debug.Log("==============================");
        Debug.Log("🍽 PLATE 2 CLICKED");
        Debug.Log("==============================");


        // =====================================================
        // FIRST:
        // ORIGINAL SERVE / PAYMENT LOGIC
        // =====================================================

        poundedYamManager.ServeFromPlate2();


        // =====================================================
        // THEN:
        // SHOW SERVED UI
        // =====================================================

        ShowServedUI();


        // =====================================================
        // WAIT 2 SECONDS
        // =====================================================

        StartCoroutine(
            HideServedUIAfterDelay()
        );
    }


    // =========================================================
    // SHOW SERVED UI
    // =========================================================

    private void ShowServedUI()
    {
        Debug.Log(
            "⭐ SHOWING SERVED UI"
        );


        // -----------------------------------------------------
        // SERVED TICK
        // -----------------------------------------------------

        if (
            customerMenuManager.servedTick != null
        )
        {
            customerMenuManager.servedTick.SetActive(true);

            Debug.Log(
                "✅ SERVED TICK = ON"
            );
        }
        else
        {
            Debug.LogError(
                "❌ SERVED TICK IS NULL!"
            );
        }


        // -----------------------------------------------------
        // SERVED TEXT
        // -----------------------------------------------------

        if (
            customerMenuManager.servedText != null
        )
        {
            customerMenuManager.servedText.SetActive(true);

            Debug.Log(
                "✅ SERVED TEXT = ON"
            );
        }
        else
        {
            Debug.LogError(
                "❌ SERVED TEXT IS NULL!"
            );
        }


        // -----------------------------------------------------
        // ORDER FOOD IMAGE OFF
        // -----------------------------------------------------

        if (
            customerMenuManager.foodImage != null
        )
        {
            customerMenuManager.foodImage
                .gameObject
                .SetActive(false);
        }
    }


    // =========================================================
    // HIDE SERVED UI
    // =========================================================

    private IEnumerator HideServedUIAfterDelay()
    {
        yield return new WaitForSeconds(2f);


        if (
            customerMenuManager == null
        )
        {
            yield break;
        }


        // -----------------------------------------------------
        // TICK OFF
        // -----------------------------------------------------

        if (
            customerMenuManager.servedTick != null
        )
        {
            customerMenuManager.servedTick
                .SetActive(false);
        }


        // -----------------------------------------------------
        // TEXT OFF
        // -----------------------------------------------------

        if (
            customerMenuManager.servedText != null
        )
        {
            customerMenuManager.servedText
                .SetActive(false);
        }


        Debug.Log(
            "✅ SERVED UI HIDDEN"
        );
    }
}