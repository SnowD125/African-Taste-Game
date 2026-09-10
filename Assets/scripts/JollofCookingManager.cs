using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JollofCookingManager : MonoBehaviour
{
    [Header("Blender Ingredients")]
    public GameObject tomatoRaw;
    public GameObject onionRaw;
    public GameObject pepperRaw;
    public GameObject blackPepperRaw;
    public GameObject garlicRaw;
    public GameObject bayLeavesRaw;

    [Header("Blender")]
    public GameObject blenderEmpty;
    public GameObject blenderBlending;
    public GameObject blenderReady;

    [Header("Blender Sound")]          // ADDED
    public AudioSource blenderVoice;   // ADDED

    [Header("Pan Ingredients")]
    public GameObject oilBottle;
    public GameObject oilInPan;
    public GameObject meatRaw;
    public GameObject riceRaw;
    public GameObject mixtureInPan;

    [Header("Jollof Cooking")]
    public GameObject jollofCooking;
    public GameObject jollofReady;
    public GameObject jollofBurned;

    [Header("Plate")]
    public GameObject plate1;
    public GameObject jollofOnPlate;

    [Header("Served UI")]
    public GameObject servedTick;
    public GameObject servedText;

    [Header("Burn")]
    public GameObject flameIcon;

    [Header("Cooking Progress Bar")]
    public Image cookingProgressBar;

    [Header("Hand Cursor")]
    public GameObject handCursor;

    [Header("Settings")]
    public float blendingTime = 3f;
    public float oilDelay = 1f;
    public float cookingTime = 1f;
    public float burnTime = 10f;

    [Header("Oil Animation")]
    public OilClick oilClickScript;

    [Header("Menu")]
    public CustomerMenuManager2 menuManager;

    [Header("Current Step")]
    public int currentStep = 0;

    public enum CookState
    {
        Idle,
        Cooking,
        ReadyToServe,
        Served,
        Burned
    }

    public CookState jollofState = CookState.Idle;

    private Coroutine blendCoroutine;
    private Coroutine fillBarCoroutine;
    private Coroutine burnCoroutine;
    private Coroutine flameBlinkCoroutine;

    private SpriteRenderer jollofCookingRenderer;

    // =========================================================
    // PAN TRANSFER PROTECTION
    // =========================================================

    private bool panTransferAllowed = false;


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        if (jollofCooking != null)
        {
            jollofCookingRenderer =
                jollofCooking.GetComponent<SpriteRenderer>();
        }

        ResetCooking();
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetCooking()
    {
        currentStep = 0;
        jollofState = CookState.Idle;

        panTransferAllowed = false;

        StopCookingCoroutines();

        // ADDED — hakikisha sauti imezimwa wakati wa reset
        if (blenderVoice != null && blenderVoice.isPlaying)
        {
            blenderVoice.Stop();
        }

        SetActive(tomatoRaw, true);
        SetActive(onionRaw, true);
        SetActive(pepperRaw, true);
        SetActive(blackPepperRaw, true);
        SetActive(garlicRaw, true);
        SetActive(bayLeavesRaw, true);

        SetActive(oilBottle, true);
        SetActive(meatRaw, true);
        SetActive(riceRaw, true);

        SetActive(blenderEmpty, true);
        SetActive(blenderBlending, false);
        SetActive(blenderReady, false);

        SetActive(oilInPan, false);
        SetActive(mixtureInPan, false);

        SetActive(jollofCooking, false);
        SetActive(jollofReady, false);
        SetActive(jollofBurned, false);

        // PLATE 1 MUST BE VISIBLE FROM START
        SetActive(plate1, true);

        // JOLLOF FOOD IS HIDDEN UNTIL PAN IS CLICKED
        SetActive(jollofOnPlate, false);

        SetActive(servedTick, false);
        SetActive(servedText, false);

        SetActive(flameIcon, false);

        if (cookingProgressBar != null)
        {
            cookingProgressBar.fillAmount = 0f;
            cookingProgressBar.gameObject.SetActive(false);
        }

        SetActive(handCursor, false);

        if (jollofCookingRenderer != null)
        {
            jollofCookingRenderer.color = Color.white;
        }

        if (oilClickScript != null)
        {
            oilClickScript.ResetBottle();
        }

        ShowHandForNextStep();

        Debug.Log("JOLLOF COOKING RESET");
    }


    // =========================================================
    // HELPER
    // =========================================================

    void SetActive(GameObject obj, bool state)
    {
        if (obj != null)
        {
            obj.SetActive(state);
        }
    }


    // =========================================================
    // STOP COROUTINES
    // =========================================================

    void StopCookingCoroutines()
    {
        if (blendCoroutine != null)
        {
            StopCoroutine(blendCoroutine);
            blendCoroutine = null;
        }

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

        CancelInvoke();
    }


    // =========================================================
    // TOMATO
    // =========================================================

    public void ClickTomato()
    {
        if (currentStep != 0)
            return;

        SetActive(tomatoRaw, false);

        currentStep = 1;

        ShowHandForNextStep();
    }


    // =========================================================
    // ONION
    // =========================================================

    public void ClickOnion()
    {
        if (currentStep != 1)
            return;

        SetActive(onionRaw, false);

        currentStep = 2;

        ShowHandForNextStep();
    }


    // =========================================================
    // PEPPER
    // =========================================================

    public void ClickPepper()
    {
        if (currentStep != 2)
            return;

        SetActive(pepperRaw, false);

        currentStep = 3;

        ShowHandForNextStep();
    }


    // =========================================================
    // BLACK PEPPER
    // =========================================================

    public void ClickBlackPepper()
    {
        if (currentStep != 3)
            return;

        SetActive(blackPepperRaw, false);

        currentStep = 4;

        ShowHandForNextStep();
    }


    // =========================================================
    // GARLIC
    // =========================================================

    public void ClickGarlic()
    {
        if (currentStep != 4)
            return;

        SetActive(garlicRaw, false);

        currentStep = 5;

        ShowHandForNextStep();
    }


    // =========================================================
    // BAY LEAVES
    // =========================================================

    public void ClickBayLeaves()
    {
        if (currentStep != 5)
            return;

        SetActive(bayLeavesRaw, false);

        currentStep = 6;

        blendCoroutine = StartCoroutine(BlendRoutine());
    }


    // =========================================================
    // BLENDING
    // =========================================================

    IEnumerator BlendRoutine()
    {
        SetActive(handCursor, false);

        SetActive(blenderEmpty, false);
        SetActive(blenderBlending, true);
        SetActive(blenderReady, false);

        // ADDED — anza kucheza sauti ya blender inaposaga
        if (blenderVoice != null)
        {
            blenderVoice.Play();
        }

        yield return new WaitForSeconds(blendingTime);

        // ADDED — simamisha sauti kusaga kukiisha
        if (blenderVoice != null && blenderVoice.isPlaying)
        {
            blenderVoice.Stop();
        }

        SetActive(blenderBlending, false);
        SetActive(blenderReady, true);

        blendCoroutine = null;

        ShowHandForNextStep();
    }


    // =========================================================
    // OIL
    // =========================================================

    public void ClickOilBottle()
    {
        if (currentStep != 6)
            return;

        if (oilClickScript != null)
        {
            oilClickScript.SimulateClick();
        }

        CancelInvoke(nameof(ShowOilInPan));

        Invoke(
            nameof(ShowOilInPan),
            oilDelay
        );
    }


    // =========================================================
    // SHOW OIL IN PAN
    // =========================================================

    public void ShowOilInPan()
    {
        SetActive(oilBottle, false);
        SetActive(oilInPan, true);

        currentStep = 7;

        if (blenderReady != null)
        {
            blenderReady.SetActive(true);
        }

        ShowCursorOnTarget(blenderReady);
    }


    // =========================================================
    // BLENDER
    // =========================================================

    public void ClickBlender()
    {
        Debug.Log(
            "ClickBlender — currentStep="
            + currentStep
        );

        if (currentStep != 7)
            return;

        SetActive(handCursor, false);

        SetActive(blenderReady, false);
        SetActive(blenderEmpty, true);

        SetActive(oilInPan, false);
        SetActive(mixtureInPan, true);

        currentStep = 8;

        ShowCursorOnTarget(meatRaw);
    }


    // =========================================================
    // MEAT
    // =========================================================

    public void ClickMeat()
    {
        if (currentStep != 8)
            return;

        SetActive(meatRaw, false);

        currentStep = 9;

        ShowCursorOnTarget(riceRaw);
    }


    // =========================================================
    // RICE
    // =========================================================

    public void ClickRice()
    {
        if (currentStep != 9)
            return;

        SetActive(riceRaw, false);
        SetActive(mixtureInPan, false);

        currentStep = 10;

        SetActive(handCursor, false);

        StartJollofCooking();
    }


    // =========================================================
    // START COOKING
    // =========================================================

    void StartJollofCooking()
    {
        Debug.Log("START JOLLOF COOKING");

        jollofState = CookState.Cooking;

        panTransferAllowed = false;

        SetActive(jollofCooking, true);
        SetActive(jollofReady, false);
        SetActive(jollofBurned, false);
        SetActive(flameIcon, false);

        if (cookingProgressBar == null)
        {
            Debug.LogError(
                "COOKING PROGRESS BAR IS NULL!"
            );

            return;
        }

        cookingProgressBar.gameObject.SetActive(true);
        cookingProgressBar.fillAmount = 0f;

        if (fillBarCoroutine != null)
        {
            StopCoroutine(fillBarCoroutine);
        }

        fillBarCoroutine =
            StartCoroutine(
                FillBarRoutine()
            );
    }


    // =========================================================
    // COOKING BAR
    // =========================================================

    IEnumerator FillBarRoutine()
    {
        Debug.Log("COOKING BAR STARTED");

        float elapsed = 0f;

        cookingProgressBar.gameObject.SetActive(true);
        cookingProgressBar.fillAmount = 0f;

        while (elapsed < cookingTime)
        {
            if (jollofState != CookState.Cooking)
                yield break;

            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / cookingTime
                );

            cookingProgressBar.fillAmount = progress;

            yield return null;
        }

        cookingProgressBar.fillAmount = 1f;

        Debug.Log("JOLLOF 100% COOKED!");

        jollofState =
            CookState.ReadyToServe;

        panTransferAllowed = false;

        SetActive(jollofCooking, false);
        SetActive(jollofReady, true);

        cookingProgressBar.gameObject.SetActive(false);

        fillBarCoroutine = null;

        burnCoroutine =
            StartCoroutine(
                BurnWarning()
            );
    }


    // =========================================================
    // BURN WARNING
    // =========================================================

    IEnumerator BurnWarning()
    {
        Debug.Log("BURN TIMER STARTED");

        if (flameIcon != null)
        {
            flameIcon.SetActive(true);

            flameBlinkCoroutine =
                StartCoroutine(
                    BlinkFlame()
                );
        }

        float elapsed = 0f;

        while (elapsed < burnTime)
        {
            if (jollofState != CookState.ReadyToServe)
                yield break;

            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / burnTime
                );

            if (jollofCookingRenderer != null)
            {
                jollofCookingRenderer.color =
                    Color.Lerp(
                        Color.white,
                        Color.red,
                        progress
                    );
            }

            yield return null;
        }

        burnCoroutine = null;

        BurnJollof();
    }


    // =========================================================
    // FLAME BLINK
    // =========================================================

    IEnumerator BlinkFlame()
    {
        while (true)
        {
            if (flameIcon != null)
            {
                flameIcon.SetActive(
                    !flameIcon.activeSelf
                );
            }

            yield return new WaitForSeconds(0.3f);
        }
    }


    // =========================================================
    // BURN
    // =========================================================

    void BurnJollof()
    {
        Debug.Log("🔥 JOLLOF BURNED!");

        jollofState =
            CookState.Burned;

        panTransferAllowed = false;

        if (cookingProgressBar != null)
        {
            cookingProgressBar.gameObject.SetActive(false);
        }

        SetActive(jollofCooking, false);
        SetActive(jollofReady, false);
        SetActive(jollofBurned, true);

        StopBurnEffect();

        SetActive(flameIcon, true);
    }


    // =========================================================
    // STOP BURN
    // =========================================================

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

        if (jollofCookingRenderer != null)
        {
            jollofCookingRenderer.color =
                Color.white;
        }

        SetActive(flameIcon, false);
    }


    // =========================================================
    // ALLOW PAN TRANSFER
    //
    // SharedPanClick calls this.
    // =========================================================

    public void AllowPanTransfer()
    {
        if (
            jollofState !=
            CookState.ReadyToServe
        )
        {
            Debug.Log(
                "❌ PAN TRANSFER DENIED - JOLLOF NOT READY"
            );

            return;
        }

        Debug.Log(
            "✅ REAL SHARED PAN CLICK CONFIRMED"
        );

        panTransferAllowed = true;

        ClickPan();
    }


    // =========================================================
    // PAN CLICK
    // =========================================================

    public void ClickPan()
    {
        Debug.Log(
            "PAN CLICK — state="
            + jollofState
            + " | transferAllowed="
            + panTransferAllowed
        );


        // =====================================================
        // JOLLOF READY
        // =====================================================

        if (
            jollofState ==
            CookState.ReadyToServe
        )
        {
            // Block automatic calls
            if (!panTransferAllowed)
            {
                Debug.Log(
                    "❌ CLICKPAN BLOCKED — NO REAL PAN CLICK"
                );

                return;
            }


            // Consume permission
            panTransferAllowed = false;


            // Stop burn timer immediately
            StopBurnEffect();


            jollofState =
                CookState.Served;


            // -------------------------------------------------
            // READY JOLLOF OFF
            // -------------------------------------------------

            SetActive(
                jollofReady,
                false
            );


            SetActive(
                jollofCooking,
                false
            );


            // -------------------------------------------------
            // PLATE 1 STAYS VISIBLE
            // -------------------------------------------------

            SetActive(
                plate1,
                true
            );


            // -------------------------------------------------
            // JOLLOF APPEARS ON PLATE
            // -------------------------------------------------

            SetActive(
                jollofOnPlate,
                true
            );


            // -------------------------------------------------
            // FLAME OFF
            // -------------------------------------------------

            SetActive(
                flameIcon,
                false
            );


            // -------------------------------------------------
            // PROGRESS BAR OFF
            // -------------------------------------------------

            if (
                cookingProgressBar != null
            )
            {
                cookingProgressBar
                    .gameObject
                    .SetActive(false);
            }


            // -------------------------------------------------
            // CURSOR OFF
            // -------------------------------------------------

            SetActive(
                handCursor,
                false
            );


            // -------------------------------------------------
            // WAIT FOR PLATE CLICK
            // -------------------------------------------------

            currentStep = 11;


            Debug.Log(
                "🍚 JOLLOF MOVED TO PLATE 1"
            );

            return;
        }


        // =====================================================
        // BURNED
        // =====================================================

        if (
            jollofState ==
            CookState.Burned
        )
        {
            Debug.Log(
                "🔥 BURNED JOLLOF CLICKED"
            );


            jollofState =
                CookState.Idle;


            panTransferAllowed = false;


            SetActive(
                jollofBurned,
                false
            );


            StopBurnEffect();


            currentStep = 6;


            SetActive(
                oilBottle,
                true
            );

            SetActive(
                meatRaw,
                true
            );

            SetActive(
                riceRaw,
                true
            );


            SetActive(
                blenderEmpty,
                true
            );

            SetActive(
                blenderBlending,
                false
            );

            SetActive(
                blenderReady,
                false
            );


            SetActive(
                oilInPan,
                false
            );

            SetActive(
                mixtureInPan,
                false
            );


            SetActive(
                jollofCooking,
                false
            );

            SetActive(
                jollofReady,
                false
            );


            SetActive(
                jollofOnPlate,
                false
            );


            if (
                cookingProgressBar != null
            )
            {
                cookingProgressBar.fillAmount = 0f;

                cookingProgressBar
                    .gameObject
                    .SetActive(false);
            }


            ShowHandForNextStep();

            return;
        }


        Debug.Log(
            "Pan clicked but food is not ready."
        );
    }


    // =========================================================
    // SERVE PLATE
    // =========================================================

    public void ServePlate()
    {
        Debug.Log(
            "PLATE CLICK — state="
            + jollofState
        );


        if (
            jollofState !=
            CookState.Served
        )
        {
            Debug.LogWarning(
                "Plate is not ready to serve."
            );

            return;
        }


        SetActive(
            plate1,
            false
        );


        SetActive(
            jollofOnPlate,
            false
        );


        SetActive(
            servedTick,
            true
        );


        SetActive(
            servedText,
            true
        );


        StartCoroutine(
            ServeRoutine()
        );
    }


    // =========================================================
    // SERVE ROUTINE
    // =========================================================

    IEnumerator ServeRoutine()
    {
        yield return new WaitForSeconds(2f);


        SetActive(
            servedTick,
            false
        );


        SetActive(
            servedText,
            false
        );


        Debug.Log(
            "JOLLOF SERVED SUCCESSFULLY!"
        );


        if (
            menuManager != null
        )
        {
            menuManager.ServePlate1();
        }
    }


    // =========================================================
    // FOOD SERVED
    // =========================================================

    public void OnFoodServed()
    {
        ResetCooking();
    }


    // =========================================================
    // CURSOR
    // =========================================================

    public void ShowHandForNextStep()
    {
        if (handCursor == null)
        {
            Debug.LogError(
                "HAND CURSOR IS NULL!"
            );

            return;
        }


        GameObject target = null;


        switch (currentStep)
        {
            case 0:
                target = tomatoRaw;
                break;

            case 1:
                target = onionRaw;
                break;

            case 2:
                target = pepperRaw;
                break;

            case 3:
                target = blackPepperRaw;
                break;

            case 4:
                target = garlicRaw;
                break;

            case 5:
                target = bayLeavesRaw;
                break;

            case 6:
                target = oilBottle;
                break;

            case 7:
                target = blenderReady;
                break;

            case 8:
                target = meatRaw;
                break;

            case 9:
                target = riceRaw;
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

            Debug.LogWarning(
                "Cursor target "
                + target.name
                + " is inactive."
            );

            return;
        }


        ShowCursorOnTarget(target);
    }


    // =========================================================
    // SHOW CURSOR
    // =========================================================

    void ShowCursorOnTarget(GameObject target)
    {
        if (handCursor == null)
            return;

        if (target == null)
            return;


        handCursor.transform.position =
            target.transform.position;


        handCursor.SetActive(true);


        SpriteRenderer cursorSprite =
            handCursor.GetComponent<SpriteRenderer>();


        if (cursorSprite != null)
        {
            cursorSprite.sortingOrder = 1000;
        }


        Canvas cursorCanvas =
            handCursor.GetComponentInParent<Canvas>();


        if (cursorCanvas != null)
        {
            cursorCanvas.sortingOrder = 1000;
        }


        Debug.Log(
            "CURSOR → "
            + target.name
            + " | STEP="
            + currentStep
        );
    }
}