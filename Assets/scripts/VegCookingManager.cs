using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VegCookingManager : MonoBehaviour
{
    [Header("Raw Ingredients")]
    public GameObject oilRaw;
    public GameObject onionRaw;
    public GameObject tomatoRaw;
    public GameObject dagaaRaw;
    public GameObject tembeleRaw;

    [Header("Pot Ingredients")]
    public GameObject oilCook;
    public GameObject onionCook;
    public GameObject tomatoCook;

    [Header("Pan Cooking Sprites")]
    public GameObject dagaaCooking;
    public GameObject tembeleCooking;

    [Header("Ready Food (Plate)")]
    public GameObject dagaaCook;
    public GameObject tembeleCook;

    [Header("Burned Foods")]
    public GameObject dagaaBurned;
    public GameObject tembeleBurned;

    [Header("Burn Effect")]
    public GameObject flameIcon;

    [Header("Progress Bar (Shared)")]
    public Image cookingProgressBar;

    [Header("Hand Guide")]
    public GameObject handCursor;

    [Header("Reference")]
    public CustomerMenuManager menuManager;

    [Header("Bottle Animation")]
    public OilClick oilClickScript;

    [Header("Cooking Settings")]
    public float cookingTime = 8f;
    public float burnTime = 5f;

    public int currentStep = 0;

    public enum CookState
    {
        Idle,
        Cooking,
        ReadyToServe,
        Served,
        Burned
    }

    public CookState dagaaState = CookState.Idle;
    public CookState tembeleState = CookState.Idle;

    [HideInInspector] public bool isDagaaReady = false;
    [HideInInspector] public bool isTembeleReady = false;
    [HideInInspector] public bool isDagaaBurned = false;
    [HideInInspector] public bool isTembeleBurned = false;
    [HideInInspector] public bool isReadyToServe = false;
    [HideInInspector] public bool isFoodOnPlate = false;

    Coroutine fillBarCoroutine;
    Coroutine burnCoroutine;
    Coroutine flameBlinkCoroutine;

    SpriteRenderer dagaaCookingRenderer;
    SpriteRenderer tembeleCookingRenderer;

    void Start()
    {
        if (dagaaCooking != null)
            dagaaCookingRenderer =
                dagaaCooking.GetComponent<SpriteRenderer>();

        if (tembeleCooking != null)
            tembeleCookingRenderer =
                tembeleCooking.GetComponent<SpriteRenderer>();

        ResetCooking();
    }

    public void ResetCooking()
    {
        currentStep = 0;

        dagaaState = CookState.Idle;
        tembeleState = CookState.Idle;

        isDagaaReady = false;
        isTembeleReady = false;

        isDagaaBurned = false;
        isTembeleBurned = false;

        isReadyToServe = false;
        isFoodOnPlate = false;

        if (fillBarCoroutine != null)
        {
            StopCoroutine(fillBarCoroutine);
            fillBarCoroutine = null;
        }

        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;
        }

        if (flameBlinkCoroutine != null)
        {
            StopCoroutine(flameBlinkCoroutine);
            flameBlinkCoroutine = null;
        }

        if (dagaaCookingRenderer != null)
            dagaaCookingRenderer.color = Color.white;

        if (tembeleCookingRenderer != null)
            tembeleCookingRenderer.color = Color.white;

        flameIcon?.SetActive(false);

        oilRaw?.SetActive(false);
        onionRaw?.SetActive(false);
        tomatoRaw?.SetActive(false);
        dagaaRaw?.SetActive(false);
        tembeleRaw?.SetActive(false);

        oilCook?.SetActive(false);
        onionCook?.SetActive(false);
        tomatoCook?.SetActive(false);

        dagaaCooking?.SetActive(false);
        tembeleCooking?.SetActive(false);

        dagaaCook?.SetActive(false);
        tembeleCook?.SetActive(false);

        dagaaBurned?.SetActive(false);
        tembeleBurned?.SetActive(false);

        if (cookingProgressBar != null)
        {
            cookingProgressBar.fillAmount = 0f;
            cookingProgressBar.gameObject.SetActive(false);
        }

        if (handCursor != null)
            handCursor.SetActive(false);

        CancelInvoke();

        oilClickScript?.ResetBottle();

        ShowHandForNextStep();
    }

    public void ForceHideDagaa()
    {
        if (dagaaState != CookState.Served)
            dagaaCook?.SetActive(false);

        if (tembeleState != CookState.Served)
            tembeleCook?.SetActive(false);
    }

    public void HidePlateFood()
    {
        dagaaCook?.SetActive(false);
        tembeleCook?.SetActive(false);

        isFoodOnPlate = false;
    }

    void ResetPanIngredients()
    {
        oilRaw?.SetActive(false);
        onionRaw?.SetActive(false);
        tomatoRaw?.SetActive(false);
        dagaaRaw?.SetActive(false);
        tembeleRaw?.SetActive(false);

        oilCook?.SetActive(false);
        onionCook?.SetActive(false);
        tomatoCook?.SetActive(false);
    }

    void StopBurnEffect()
    {
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;
        }

        if (flameBlinkCoroutine != null)
        {
            StopCoroutine(flameBlinkCoroutine);
            flameBlinkCoroutine = null;
        }

        if (dagaaCookingRenderer != null)
            dagaaCookingRenderer.color = Color.white;

        if (tembeleCookingRenderer != null)
            tembeleCookingRenderer.color = Color.white;

        flameIcon?.SetActive(false);
    }

    public void ClickOil()
    {
        if (currentStep != 0)
        {
            ShowHandForNextStep();
            return;
        }

        oilRaw.SetActive(false);
        oilCook.SetActive(true);

        onionRaw.SetActive(true);

        currentStep++;

        if (fillBarCoroutine != null)
            StopCoroutine(fillBarCoroutine);

        fillBarCoroutine =
            StartCoroutine(FillBarOnly());

        ShowHandForNextStep();
    }

    IEnumerator FillBarOnly()
    {
        if (cookingProgressBar != null)
        {
            cookingProgressBar.gameObject.SetActive(true);
            cookingProgressBar.fillAmount = 0f;
        }

        float t = 0f;

        while (t < cookingTime)
        {
            t += Time.deltaTime;

            if (cookingProgressBar != null)
            {
                cookingProgressBar.fillAmount =
                    Mathf.Clamp01(
                        t / cookingTime
                    );
            }

            yield return null;
        }

        if (cookingProgressBar != null)
            cookingProgressBar.fillAmount = 1f;

        fillBarCoroutine = null;

        if (dagaaState == CookState.Cooking)
        {
            dagaaState = CookState.ReadyToServe;

            isDagaaReady = true;
            isReadyToServe = true;

            Debug.Log(
                "Bar imejaa — Dagaa Ready! ✅"
            );

            burnCoroutine =
                StartCoroutine(
                    BurnWarning(true)
                );
        }
        else if (tembeleState == CookState.Cooking)
        {
            tembeleState = CookState.ReadyToServe;

            isTembeleReady = true;
            isReadyToServe = true;

            Debug.Log(
                "Bar imejaa — Tembele Ready! ✅"
            );

            burnCoroutine =
                StartCoroutine(
                    BurnWarning(false)
                );
        }
        else
        {
            Debug.Log(
                "Bar imejaa — subiri dagaa/tembele ibonyezwe"
            );
        }
    }

    IEnumerator BurnWarning(bool isDagaa)
    {
        SpriteRenderer sr =
            isDagaa
                ? dagaaCookingRenderer
                : tembeleCookingRenderer;

        flameIcon?.SetActive(true);

        flameBlinkCoroutine =
            StartCoroutine(
                BlinkFlame()
            );

        float t = 0f;

        while (t < burnTime)
        {
            t += Time.deltaTime;

            float progress =
                t / burnTime;

            if (sr != null)
            {
                if (progress < 0.5f)
                {
                    sr.color =
                        Color.Lerp(
                            Color.white,
                            Color.yellow,
                            progress * 2f
                        );
                }
                else
                {
                    sr.color =
                        Color.Lerp(
                            Color.yellow,
                            Color.red,
                            (progress - 0.5f) * 2f
                        );
                }
            }

            yield return null;
        }

        burnCoroutine = null;

        if (isDagaa &&
            dagaaState == CookState.ReadyToServe)
        {
            BurnDagaa();
        }
        else if (!isDagaa &&
                 tembeleState == CookState.ReadyToServe)
        {
            BurnTembele();
        }
    }

    IEnumerator BlinkFlame()
    {
        while (true)
        {
            if (flameIcon != null)
                flameIcon.SetActive(
                    !flameIcon.activeSelf
                );

            yield return new WaitForSeconds(0.3f);
        }
    }

    public void ClickOnion()
    {
        if (currentStep != 1)
        {
            ShowHandForNextStep();
            return;
        }

        onionRaw.SetActive(false);
        onionCook.SetActive(true);

        tomatoRaw.SetActive(true);

        currentStep++;

        ShowHandForNextStep();
    }

    public void ClickTomato()
    {
        if (currentStep != 2)
        {
            ShowHandForNextStep();
            return;
        }

        tomatoRaw.SetActive(false);
        tomatoCook.SetActive(true);

        if (menuManager != null)
        {
            if (menuManager.selectedFood == 0)
            {
                dagaaRaw.SetActive(true);
                tembeleRaw.SetActive(false);
            }
            else if (
                menuManager.selectedFood == 1 ||
                menuManager.isComboOrder
            )
            {
                tembeleRaw.SetActive(true);
                dagaaRaw.SetActive(false);
            }
        }

        currentStep++;

        ShowHandForNextStep();
    }

    public void ClickDagaa()
    {
        if (currentStep != 3)
            return;

        if (dagaaState != CookState.Idle)
            return;

        if (menuManager == null ||
            menuManager.selectedFood != 0)
            return;

        dagaaRaw.SetActive(false);

        if (handCursor != null)
            handCursor.SetActive(false);

        dagaaCooking?.SetActive(true);

        dagaaCook?.SetActive(false);

        isFoodOnPlate = false;

        dagaaState = CookState.Cooking;
        isDagaaReady = false;

        if (cookingProgressBar != null &&
            cookingProgressBar.fillAmount >= 1f)
        {
            dagaaState = CookState.ReadyToServe;

            isDagaaReady = true;
            isReadyToServe = true;

            burnCoroutine =
                StartCoroutine(
                    BurnWarning(true)
                );
        }

        Debug.Log(
            "Dagaa wanaiva — dagaaState="
            + dagaaState
        );
    }

    public void ClickTembele()
    {
        if (currentStep != 3)
            return;

        if (tembeleState != CookState.Idle)
            return;

        if (menuManager == null)
            return;

        if (!(
            menuManager.selectedFood == 1 ||
            menuManager.isComboOrder
        ))
            return;

        tembeleRaw.SetActive(false);

        if (handCursor != null)
            handCursor.SetActive(false);

        tembeleCooking?.SetActive(true);

        tembeleCook?.SetActive(false);

        isFoodOnPlate = false;

        tembeleState = CookState.Cooking;
        isTembeleReady = false;

        if (cookingProgressBar != null &&
            cookingProgressBar.fillAmount >= 1f)
        {
            tembeleState =
                CookState.ReadyToServe;

            isTembeleReady = true;
            isReadyToServe = true;

            burnCoroutine =
                StartCoroutine(
                    BurnWarning(false)
                );
        }

        Debug.Log(
            "Tembele wanaiva — tembeleState="
            + tembeleState
        );
    }

    // =========================================================
    // PAN → PLATE
    // =========================================================

    public void TryServePan()
    {
        Debug.Log(
            $"TryServePan — dagaaState={dagaaState} " +
            $"tembeleState={tembeleState}"
        );

        // =====================================================
        // DAGAA
        // =====================================================

        if (dagaaState == CookState.ReadyToServe)
        {
            dagaaState = CookState.Served;

            isDagaaReady = false;
            isReadyToServe = false;
            isFoodOnPlate = true;

            if (fillBarCoroutine != null)
            {
                StopCoroutine(
                    fillBarCoroutine
                );

                fillBarCoroutine = null;
            }

            StopBurnEffect();

            ResetPanIngredients();

            dagaaCooking?.SetActive(false);

            dagaaCook?.SetActive(true);

            // =================================================
            // DAGAA → PLATE 1
            // =================================================

            if (menuManager != null)
            {
                menuManager.plate1?.SetActive(true);
            }

            if (cookingProgressBar != null)
            {
                cookingProgressBar
                    .gameObject
                    .SetActive(false);
            }

            currentStep++;

            Debug.Log(
                "Dagaa served → Plate 1 activated ✅"
            );

            return;
        }

        // =====================================================
        // TEMBELE
        // =====================================================

        if (tembeleState == CookState.ReadyToServe)
        {
            tembeleState = CookState.Served;

            isTembeleReady = false;
            isReadyToServe = false;
            isFoodOnPlate = true;

            if (fillBarCoroutine != null)
            {
                StopCoroutine(
                    fillBarCoroutine
                );

                fillBarCoroutine = null;
            }

            StopBurnEffect();

            ResetPanIngredients();

            tembeleCooking?.SetActive(false);

            tembeleCook?.SetActive(true);

            // =================================================
            // TEMBELE → CORRECT PLATE
            // =================================================

            if (menuManager != null)
            {
                if (menuManager.isComboOrder &&
                    menuManager.selectedFood == 4)
                {
                    // -------------------------------------------------
                    // UGALI + TEMBELE
                    // -------------------------------------------------

                    menuManager.plate2?.SetActive(true);

                    // Tell Plate2 that this plate belongs
                    // to the Ugali + Tembele order.
                    if (menuManager.plate2 != null)
                    {
                        Plate2 plate2Script =
                            menuManager.plate2.GetComponent<Plate2>();

                        if (plate2Script != null)
                        {
                            plate2Script.PrepareForUgaliTembele();
                        }
                        else
                        {
                            Debug.LogWarning(
                                "Plate2 GameObject does not have Plate2 script!"
                            );
                        }
                    }

                    Debug.Log(
                        "Ugali + Tembele served → Plate 2 activated and prepared ✅"
                    );
                }
                else
                {
                    // -------------------------------------------------
                    // TEMBELE PEKE YAKE
                    // -------------------------------------------------

                    menuManager.plate1?.SetActive(true);

                    Debug.Log(
                        "Tembele served → Plate 1 activated ✅"
                    );
                }
            }

            if (cookingProgressBar != null)
            {
                cookingProgressBar
                    .gameObject
                    .SetActive(false);
            }

            currentStep++;

            return;
        }

        // =====================================================
        // BURNED DAGAA
        // =====================================================

        if (dagaaState == CookState.Burned)
        {
            dagaaState = CookState.Idle;

            isDagaaBurned = false;

            dagaaBurned?.SetActive(false);

            StopBurnEffect();

            ResetPanIngredients();

            currentStep = 3;

            dagaaRaw.SetActive(true);

            ShowHandForNextStep();

            return;
        }

        // =====================================================
        // BURNED TEMBELE
        // =====================================================

        if (tembeleState == CookState.Burned)
        {
            tembeleState = CookState.Idle;

            isTembeleBurned = false;

            tembeleBurned?.SetActive(false);

            StopBurnEffect();

            ResetPanIngredients();

            currentStep = 3;

            tembeleRaw.SetActive(true);

            ShowHandForNextStep();

            return;
        }

        Debug.Log(
            "Pan clicked — nothing ready yet"
        );
    }

    void BurnDagaa()
    {
        if (dagaaState == CookState.Burned)
            return;

        dagaaState = CookState.Burned;

        isDagaaReady = false;
        isDagaaBurned = true;

        isReadyToServe = false;
        isFoodOnPlate = false;

        if (cookingProgressBar != null)
        {
            cookingProgressBar
                .gameObject
                .SetActive(false);
        }

        ResetPanIngredients();

        dagaaCooking?.SetActive(false);

        dagaaCook?.SetActive(false);

        StopBurnEffect();

        dagaaBurned?.SetActive(true);

        flameIcon?.SetActive(true);

        Debug.Log(
            "Dagaa Burned 🔥"
        );
    }

    void BurnTembele()
    {
        if (tembeleState == CookState.Burned)
            return;

        tembeleState = CookState.Burned;

        isTembeleReady = false;
        isTembeleBurned = true;

        isReadyToServe = false;
        isFoodOnPlate = false;

        if (cookingProgressBar != null)
        {
            cookingProgressBar
                .gameObject
                .SetActive(false);
        }

        ResetPanIngredients();

        tembeleCooking?.SetActive(false);

        tembeleCook?.SetActive(false);

        StopBurnEffect();

        tembeleBurned?.SetActive(true);

        flameIcon?.SetActive(true);

        Debug.Log(
            "Tembele Burned 🔥"
        );
    }

    public void ClickBurnedDagaa()
    {
        if (dagaaState != CookState.Burned)
            return;

        dagaaState = CookState.Idle;

        isDagaaBurned = false;

        dagaaBurned?.SetActive(false);

        StopBurnEffect();

        ResetPanIngredients();

        currentStep = 3;

        dagaaRaw.SetActive(true);

        ShowHandForNextStep();
    }

    public void ClickBurnedTembele()
    {
        if (tembeleState != CookState.Burned)
            return;

        tembeleState = CookState.Idle;

        isTembeleBurned = false;

        tembeleBurned?.SetActive(false);

        StopBurnEffect();

        ResetPanIngredients();

        currentStep = 3;

        tembeleRaw.SetActive(true);

        ShowHandForNextStep();
    }

    public void ShowHandForNextStep()
    {
        if (handCursor == null)
            return;

        GameObject target = null;

        switch (currentStep)
        {
            case 0:
                target = oilRaw;
                break;

            case 1:
                target = onionRaw;
                break;

            case 2:
                target = tomatoRaw;
                break;

            case 3:

                if (
                    menuManager != null &&
                    menuManager.selectedFood == 0
                )
                {
                    target = dagaaRaw;
                }
                else
                {
                    target = tembeleRaw;
                }

                break;

            default:

                handCursor.SetActive(false);

                return;
        }

        if (
            target != null &&
            target.activeInHierarchy
        )
        {
            handCursor.SetActive(true);

            handCursor.transform.position =
                target.transform.position;
        }
    }
}