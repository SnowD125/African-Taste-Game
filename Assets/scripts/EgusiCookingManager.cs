using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EgusiCookingManager : MonoBehaviour
{
    // =========================================================
    // INGREDIENTS
    // =========================================================

    [Header("Ingredients")]
    public GameObject palmOil;
    public GameObject onion;
    public GameObject egusiSeeds;
    public GameObject rawMeat;
    public GameObject vegetable;
    public GameObject salt;


    // =========================================================
    // PAN
    // =========================================================

    [Header("Pan")]
    public GameObject egusiPan;
    public GameObject oilInPan;


    // =========================================================
    // EGUSI FOOD
    // =========================================================

    [Header("Egusi Food")]
    public GameObject egusiCooking;
    public GameObject egusiReady;
    public GameObject egusiBurned;


    // =========================================================
    // FLAME
    // =========================================================

    [Header("Flame")]
    public GameObject flameIcon;


    // =========================================================
    // COOKING BAR
    // =========================================================

    [Header("Cooking Progress Bar")]
    public Image cookingProgressBar;


    // =========================================================
    // HAND CURSOR
    // =========================================================

    [Header("Hand Cursor")]
    public GameObject handCursor;


    // =========================================================
    // SETTINGS
    // =========================================================

    [Header("Cooking Settings")]
    public float cookingTime = 5f;
    public float burnTime = 10f;


    // =========================================================
    // CURRENT STEP
    // =========================================================

    [Header("Current Step")]
    public int currentStep = 0;


    // =========================================================
    // STATE
    // =========================================================

    public enum CookState
    {
        Idle,
        Cooking,
        ReadyToServe,
        Served,
        Burned
    }

    public CookState egusiState = CookState.Idle;


    // =========================================================
    // COROUTINES
    // =========================================================

    private Coroutine cookingCoroutine;
    private Coroutine burnCoroutine;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        ResetEgusiCooking();
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetEgusiCooking()
    {
        currentStep = 0;

        egusiState = CookState.Idle;

        StopAllEgusiCoroutines();


        // Ingredients visible
        SetActive(palmOil, true);
        SetActive(onion, true);
        SetActive(egusiSeeds, true);
        SetActive(rawMeat, true);
        SetActive(vegetable, true);
        SetActive(salt, true);


        // Pan
        SetActive(egusiPan, true);
        SetActive(oilInPan, false);


        // Food
        SetActive(egusiCooking, false);
        SetActive(egusiReady, false);
        SetActive(egusiBurned, false);


        // Flame
        SetActive(flameIcon, false);


        // Progress bar
        if (cookingProgressBar != null)
        {
            cookingProgressBar.fillAmount = 0f;
            cookingProgressBar.gameObject.SetActive(false);
        }


        // Cursor
        SetActive(handCursor, false);

        ShowCursorForCurrentStep();


        Debug.Log("==============================");
        Debug.Log("EGUSI COOKING STARTED");
        Debug.Log("STEP 0 → CLICK PALM OIL");
        Debug.Log("==============================");
    }


    // =========================================================
    // HELPER
    // =========================================================

    private void SetActive(GameObject obj, bool value)
    {
        if (obj != null)
            obj.SetActive(value);
    }


    // =========================================================
    // STOP COROUTINES
    // =========================================================

    private void StopAllEgusiCoroutines()
    {
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
            cookingCoroutine = null;
        }

        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;
        }
    }


    // =========================================================
    // STEP 0 — PALM OIL
    // =========================================================

    public void ClickPalmOil()
    {
        if (currentStep != 0)
            return;


        Debug.Log("PALM OIL CLICKED");


        SetActive(palmOil, false);

        SetActive(oilInPan, true);


        currentStep = 1;


        Debug.Log("PALM OIL ADDED TO PAN");
        Debug.Log("STEP 1 → CLICK ONION");


        ShowCursorForCurrentStep();
    }


    // =========================================================
    // STEP 1 — ONION
    // =========================================================

    public void ClickOnion()
    {
        if (currentStep != 1)
            return;


        Debug.Log("ONION CLICKED");


        SetActive(onion, false);


        currentStep = 2;


        Debug.Log("ONION ADDED");
        Debug.Log("STEP 2 → CLICK EGUSI SEEDS");


        ShowCursorForCurrentStep();
    }


    // =========================================================
    // STEP 2 — EGUSI SEEDS
    // =========================================================

    public void ClickEgusiSeeds()
    {
        if (currentStep != 2)
            return;


        Debug.Log("==============================");
        Debug.Log("EGUSI SEEDS CLICKED");
        Debug.Log("==============================");


        SetActive(egusiSeeds, false);


        currentStep = 3;


        Debug.Log("EGUSI SEEDS ADDED");
        Debug.Log("STEP 3 → CLICK MEAT");


        ShowCursorForCurrentStep();
    }


    // Compatibility
    public void ClickIngredient()
    {
        ClickEgusiSeeds();
    }


    // =========================================================
    // STEP 3 — MEAT
    // =========================================================

    public void ClickMeat()
    {
        if (currentStep != 3)
            return;


        Debug.Log("MEAT CLICKED");


        SetActive(rawMeat, false);


        currentStep = 4;


        Debug.Log("MEAT ADDED");
        Debug.Log("STEP 4 → CLICK VEGETABLE");


        ShowCursorForCurrentStep();
    }


    // =========================================================
    // STEP 4 — VEGETABLE
    // =========================================================

    public void ClickVegetable()
    {
        if (currentStep != 4)
            return;


        Debug.Log("VEGETABLE CLICKED");


        SetActive(vegetable, false);


        currentStep = 5;


        Debug.Log("VEGETABLE ADDED");
        Debug.Log("STEP 5 → CLICK SALT");


        ShowCursorForCurrentStep();
    }


    // =========================================================
    // STEP 5 — SALT
    // =========================================================

    public void ClickSalt()
    {
        if (currentStep != 5)
            return;


        Debug.Log("SALT CLICKED");


        SetActive(salt, false);


        currentStep = 6;


        Debug.Log("SALT ADDED");
        Debug.Log("STARTING EGUSI COOKING");


        StartEgusiCooking();
    }


    // =========================================================
    // START COOKING
    // =========================================================

    private void StartEgusiCooking()
    {
        egusiState = CookState.Cooking;


        SetActive(egusiCooking, true);

        SetActive(egusiReady, false);

        SetActive(egusiBurned, false);

        SetActive(flameIcon, false);

        SetActive(handCursor, false);


        if (cookingProgressBar != null)
        {
            cookingProgressBar.gameObject.SetActive(true);

            cookingProgressBar.fillAmount = 0f;
        }


        Debug.Log("EGUSI COOKING...");


        cookingCoroutine =
            StartCoroutine(CookingRoutine());
    }


    // =========================================================
    // COOKING ROUTINE
    // =========================================================

    private IEnumerator CookingRoutine()
    {
        float elapsed = 0f;


        while (elapsed < cookingTime)
        {
            if (egusiState != CookState.Cooking)
                yield break;


            elapsed += Time.deltaTime;


            float progress =
                Mathf.Clamp01(
                    elapsed / cookingTime
                );


            if (cookingProgressBar != null)
            {
                cookingProgressBar.fillAmount =
                    progress;
            }


            yield return null;
        }


        cookingCoroutine = null;


        if (cookingProgressBar != null)
        {
            cookingProgressBar.fillAmount = 1f;

            cookingProgressBar.gameObject.SetActive(false);
        }


        EgusiReadyInPan();
    }


    // =========================================================
    // EGUSI COOKING FINISHED
    //
    // MUHIMU:
    // READY EGUSI HAIONEKANI BADO.
    //
    // EGUSI INABAKI KWENYE PAN.
    // PLAYER LAZIMA ABONYEZE PAN.
    // =========================================================

    private void EgusiReadyInPan()
    {
        Debug.Log("==============================");
        Debug.Log("🍲 EGUSI SOUP IS READY!");
        Debug.Log("👆 CLICK THE PAN");
        Debug.Log("==============================");


        egusiState =
            CookState.ReadyToServe;


        // Egusi bado iko pan
        SetActive(
            egusiCooking,
            true
        );


        // Ready image haionekani mpaka pan iclickwe
        SetActive(
            egusiReady,
            false
        );


        SetActive(
            egusiBurned,
            false
        );


        // Flame inaanza baada ya kuiva
        SetActive(
            flameIcon,
            true
        );


        // Step 6 = CLICK PAN
        currentStep = 6;


        ShowCursorForCurrentStep();


        // Burn countdown
        burnCoroutine =
            StartCoroutine(
                BurnRoutine()
            );
    }


    // =========================================================
    // PAN CLICK
    //
    // EGUSI:
    // PAN → READY EGUSI
    // =========================================================

    public void MoveEgusiToReady()
    {
        if (
            egusiState !=
            CookState.ReadyToServe
        )
        {
            Debug.Log(
                "EGUSI PAN CLICK IGNORED — NOT READY"
            );

            return;
        }


        Debug.Log("==============================");
        Debug.Log("🍲 EGUSI PAN CLICKED");
        Debug.Log("🍲 MOVING EGUSI TO READY");
        Debug.Log("==============================");


        // Stop burn timer
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);

            burnCoroutine = null;
        }


        // Pan food OFF
        SetActive(
            egusiCooking,
            false
        );


        // Ready food ON
        SetActive(
            egusiReady,
            true
        );


        // Burned OFF
        SetActive(
            egusiBurned,
            false
        );


        // Flame OFF
        SetActive(
            flameIcon,
            false
        );


        // Ready step
        currentStep = 7;


        ShowCursorForCurrentStep();


        Debug.Log(
            "🍲 READY EGUSI IS NOW VISIBLE"
        );

        Debug.Log(
            "👆 CLICK READY EGUSI TO SERVE"
        );
    }


    // =========================================================
    // BURN ROUTINE
    // =========================================================

    private IEnumerator BurnRoutine()
    {
        float elapsed = 0f;


        while (elapsed < burnTime)
        {
            if (
                egusiState !=
                CookState.ReadyToServe
            )
            {
                yield break;
            }


            elapsed += Time.deltaTime;


            yield return null;
        }


        burnCoroutine = null;


        BurnEgusi();
    }


    // =========================================================
    // BURNED
    // =========================================================

    private void BurnEgusi()
    {
        if (
            egusiState !=
            CookState.ReadyToServe
        )
        {
            return;
        }


        Debug.Log(
            "🔥 EGUSI SOUP BURNED!"
        );


        egusiState =
            CookState.Burned;


        SetActive(
            egusiCooking,
            false
        );


        SetActive(
            egusiReady,
            false
        );


        SetActive(
            egusiBurned,
            true
        );


        SetActive(
            flameIcon,
            true
        );


        SetActive(
            handCursor,
            false
        );
    }


    // =========================================================
    // CLICK READY EGUSI
    // =========================================================

    public void ClickReadyEgusi()
    {
        if (
            egusiState !=
            CookState.ReadyToServe
        )
        {
            Debug.Log(
                "READY EGUSI CLICK IGNORED"
            );

            return;
        }


        Debug.Log("==============================");
        Debug.Log("🍲 READY EGUSI CLICKED");
        Debug.Log("🍲 EGUSI SOUP SERVED!");
        Debug.Log("==============================");


        if (burnCoroutine != null)
        {
            StopCoroutine(
                burnCoroutine
            );

            burnCoroutine = null;
        }


        egusiState =
            CookState.Served;


        SetActive(
            egusiReady,
            false
        );


        SetActive(
            flameIcon,
            false
        );


        SetActive(
            handCursor,
            false
        );


        // Serve customer
        FirstCustomer2 customer =
            Object.FindFirstObjectByType<FirstCustomer2>();


        if (
            customer != null &&
            !customer.IsLeaving
        )
        {
            customer.ServeCustomer();


            Debug.Log(
                "CUSTOMER SERVED SUCCESSFULLY"
            );
        }
        else
        {
            Debug.LogWarning(
                "FirstCustomer2 NOT FOUND"
            );
        }
    }


    // =========================================================
    // OLD COMPATIBILITY METHOD
    // =========================================================

    public void ClickBowl()
    {
        ClickReadyEgusi();
    }


    // =========================================================
    // CURSOR
    // =========================================================

    private void ShowCursorForCurrentStep()
    {
        if (handCursor == null)
            return;


        GameObject target = null;


        switch (currentStep)
        {
            case 0:

                target = palmOil;

                break;


            case 1:

                target = onion;

                break;


            case 2:

                target = egusiSeeds;

                break;


            case 3:

                target = rawMeat;

                break;


            case 4:

                target = vegetable;

                break;


            case 5:

                target = salt;

                break;


            case 6:

                // IMPORTANT:
                // baada ya kupika cursor inaenda PAN
                target = egusiPan;

                break;


            case 7:

                // baada ya pan click cursor inaenda READY EGUSI
                target = egusiReady;

                break;


            default:

                handCursor.SetActive(false);

                return;
        }


        if (target == null)
        {
            handCursor.SetActive(false);

            return;
        }


        if (!target.activeInHierarchy)
        {
            handCursor.SetActive(false);

            return;
        }


        handCursor.transform.position =
            target.transform.position;


        handCursor.SetActive(true);


        SpriteRenderer sr =
            handCursor.GetComponent<SpriteRenderer>();


        if (sr != null)
        {
            sr.sortingOrder = 1000;
        }


        Debug.Log(
            "CURSOR → "
            + target.name
            + " | STEP="
            + currentStep
        );
    }


    // =========================================================
    // PUBLIC CURSOR METHOD
    // =========================================================

    public void ShowReadyCursor()
    {
        currentStep = 7;

        ShowCursorForCurrentStep();
    }
}