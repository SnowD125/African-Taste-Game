using UnityEngine;
using System.Collections;

public class PoundedYamCookingManager : MonoBehaviour
{
    // =========================================================
    // RAW YAM
    // =========================================================

    [Header("Raw Yam")]
    public GameObject yamRaw;


    // =========================================================
    // BOILING WATER
    // =========================================================

    [Header("Boiling Water")]
    public GameObject boilingWater;


    // =========================================================
    // BOILING YAM
    // =========================================================

    [Header("Boiling Yam")]
    public GameObject boilingYam;


    // =========================================================
    // POT
    // =========================================================

    [Header("Pot")]
    public GameObject pot;


    // =========================================================
    // EMPTY MORTAR
    // KINU TUPU
    // =========================================================

    [Header("Mortar - Empty")]
    public GameObject mortarEmpty;


    // =========================================================
    // POUNDING YAMS
    // SPRITE YA KUTWANGA
    // =========================================================

    [Header("Pounding Yams")]
    public GameObject poundingYam;


    // =========================================================
    // READY MORTAR
    // KINU CHENYE POUNDED YAM
    // =========================================================

    [Header("Ready Mortar")]
    public GameObject readyPoundedYam;


    // =========================================================
    // POUNDED YAM ONLY ON PLATE
    // YAM PEKE YAKE
    // =========================================================

    [Header("Pounded Yam On Plate")]
    public GameObject poundedYamOnPlate;


    // =========================================================
    // PLATE 2
    // =========================================================

    [Header("Plate 2")]
    public GameObject plate2;


    // =========================================================
    // HAND CURSOR
    // =========================================================

    [Header("Hand Cursor")]
    public GameObject handCursor;


    // =========================================================
    // COOKING SETTINGS
    // =========================================================

    [Header("Cooking Settings")]
    public float boilingTime = 5f;

    public float poundingTime = 5f;


    // =========================================================
    // CURRENT STEP
    // =========================================================

    [Header("Current Step")]
    public int currentStep = 0;


    // =========================================================
    // STATE
    // =========================================================

    public enum PoundedYamState
    {
        Idle,
        Boiling,
        Boiled,
        Pounding,
        Pounded,
        ReadyToServe,
        Served
    }

    public PoundedYamState yamState =
        PoundedYamState.Idle;


    // =========================================================
    // COROUTINES
    // =========================================================

    private Coroutine boilingCoroutine;

    private Coroutine poundingCoroutine;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        ResetPoundedYam();
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetPoundedYam()
    {
        currentStep = 0;

        yamState =
            PoundedYamState.Idle;

        StopAllCooking();


        // -----------------------------------------------------
        // RAW YAM = ON
        // -----------------------------------------------------

        SetActive(
            yamRaw,
            true
        );


        // -----------------------------------------------------
        // BOILING WATER = ON
        // -----------------------------------------------------

        SetActive(
            boilingWater,
            true
        );


        // -----------------------------------------------------
        // BOILING YAM = OFF
        // -----------------------------------------------------

        SetActive(
            boilingYam,
            false
        );


        // -----------------------------------------------------
        // POT = OFF
        // -----------------------------------------------------

        SetActive(
            pot,
            false
        );


        // -----------------------------------------------------
        // EMPTY MORTAR = ON
        // -----------------------------------------------------

        SetActive(
            mortarEmpty,
            true
        );


        // -----------------------------------------------------
        // POUNDING YAMS = OFF
        // -----------------------------------------------------

        SetActive(
            poundingYam,
            false
        );


        // -----------------------------------------------------
        // READY MORTAR = OFF
        // -----------------------------------------------------

        SetActive(
            readyPoundedYam,
            false
        );


        // -----------------------------------------------------
        // POUNDED YAM ON PLATE = OFF
        // -----------------------------------------------------

        SetActive(
            poundedYamOnPlate,
            false
        );


        // -----------------------------------------------------
        // PLATE 2 IS NOT TOUCHED
        // -----------------------------------------------------


        // -----------------------------------------------------
        // CURSOR OFF
        // -----------------------------------------------------

        SetActive(
            handCursor,
            false
        );


        ShowCursorForCurrentStep();


        Debug.Log("==============================");
        Debug.Log("POUNDED YAM RESET");
        Debug.Log("RAW YAM = ON");
        Debug.Log("BOILING WATER = ON");
        Debug.Log("BOILING YAM = OFF");
        Debug.Log("POT = OFF");
        Debug.Log("EMPTY MORTAR = ON");
        Debug.Log("POUNDING YAMS = OFF");
        Debug.Log("READY MORTAR = OFF");
        Debug.Log("YAM ON PLATE = OFF");
        Debug.Log("==============================");
    }


    // =========================================================
    // HELPER
    // =========================================================

    private void SetActive(
        GameObject obj,
        bool state
    )
    {
        if (obj != null)
        {
            obj.SetActive(state);
        }
    }


    // =========================================================
    // STOP COOKING
    // =========================================================

    private void StopAllCooking()
    {
        if (boilingCoroutine != null)
        {
            StopCoroutine(
                boilingCoroutine
            );

            boilingCoroutine = null;
        }


        if (poundingCoroutine != null)
        {
            StopCoroutine(
                poundingCoroutine
            );

            poundingCoroutine = null;
        }
    }


    // =========================================================
    // STEP 0
    // CLICK RAW YAM
    // =========================================================

    public void ClickYams()
    {
        if (
            currentStep != 0 ||
            yamState !=
            PoundedYamState.Idle
        )
        {
            Debug.Log(
                "RAW YAM CLICK IGNORED"
            );

            return;
        }


        Debug.Log("==============================");
        Debug.Log("🥔 RAW YAM CLICKED");
        Debug.Log("🔥 BOILING STARTED");
        Debug.Log("==============================");


        // Raw yam OFF
        SetActive(
            yamRaw,
            false
        );


        // Boiling water OFF
        SetActive(
            boilingWater,
            false
        );


        // Boiling yam ON
        SetActive(
            boilingYam,
            true
        );


        // Pot OFF
        SetActive(
            pot,
            false
        );


        // Empty mortar stays ON
        SetActive(
            mortarEmpty,
            true
        );


        // Pounding OFF
        SetActive(
            poundingYam,
            false
        );


        // Ready mortar OFF
        SetActive(
            readyPoundedYam,
            false
        );


        // Yam on plate OFF
        SetActive(
            poundedYamOnPlate,
            false
        );


        currentStep = 1;

        yamState =
            PoundedYamState.Boiling;


        // Cursor OFF
        SetActive(
            handCursor,
            false
        );


        boilingCoroutine =
            StartCoroutine(
                BoilingRoutine()
            );
    }


    // =========================================================
    // BOILING ROUTINE
    // =========================================================

    private IEnumerator BoilingRoutine()
    {
        float elapsed = 0f;


        while (
            elapsed < boilingTime
        )
        {
            if (
                yamState !=
                PoundedYamState.Boiling
            )
            {
                yield break;
            }


            elapsed +=
                Time.deltaTime;


            yield return null;
        }


        boilingCoroutine = null;


        BoilingFinished();
    }


    // =========================================================
    // BOILING FINISHED
    //
    // BOILING YAM OFF
    // POT ON
    // =========================================================

    private void BoilingFinished()
    {
        Debug.Log("==============================");
        Debug.Log("🥔 BOILING FINISHED");
        Debug.Log("🥔 BOILING YAM = OFF");
        Debug.Log("💧 BOILING WATER = OFF");
        Debug.Log("🍲 POT = ON");
        Debug.Log("👆 CLICK POT");
        Debug.Log("==============================");


        yamState =
            PoundedYamState.Boiled;


        currentStep = 2;


        // Boiling yam OFF
        SetActive(
            boilingYam,
            false
        );


        // Boiling water OFF
        SetActive(
            boilingWater,
            false
        );


        // Pot ON
        SetActive(
            pot,
            true
        );


        // Empty mortar stays ON
        SetActive(
            mortarEmpty,
            true
        );


        // Pounding OFF
        SetActive(
            poundingYam,
            false
        );


        // Ready mortar OFF
        SetActive(
            readyPoundedYam,
            false
        );


        // Yam on plate OFF
        SetActive(
            poundedYamOnPlate,
            false
        );


        ShowCursorForCurrentStep();
    }


    // =========================================================
    // STEP 2
    // CLICK POT
    //
    // POT OFF
    // BOILING WATER ON
    // EMPTY MORTAR OFF
    // POUNDING YAMS ON
    // =========================================================

    public void ClickPot()
    {
        if (
            currentStep != 2 ||
            yamState !=
            PoundedYamState.Boiled
        )
        {
            Debug.Log(
                "POT CLICK IGNORED"
            );

            return;
        }


        Debug.Log("==============================");
        Debug.Log("🍲 POT CLICKED");
        Debug.Log("🍲 POT = OFF");
        Debug.Log("💧 BOILING WATER = ON");
        Debug.Log("🔨 EMPTY MORTAR = OFF");
        Debug.Log("🥔 POUNDING YAMS = ON");
        Debug.Log("==============================");


        // Pot OFF
        SetActive(
            pot,
            false
        );


        // Boiling water ON
        SetActive(
            boilingWater,
            true
        );


        // Empty mortar OFF
        SetActive(
            mortarEmpty,
            false
        );


        // Pounding yams ON
        SetActive(
            poundingYam,
            true
        );


        // Ready mortar OFF
        SetActive(
            readyPoundedYam,
            false
        );


        // Yam on plate OFF
        SetActive(
            poundedYamOnPlate,
            false
        );


        currentStep = 3;


        StartPounding();
    }


    // =========================================================
    // START POUNDING
    // =========================================================

    private void StartPounding()
    {
        yamState =
            PoundedYamState.Pounding;


        // Cursor OFF
        SetActive(
            handCursor,
            false
        );


        Debug.Log(
            "🔨 POUNDING STARTED"
        );


        poundingCoroutine =
            StartCoroutine(
                PoundingRoutine()
            );
    }


    // =========================================================
    // POUNDING ROUTINE
    // =========================================================

    private IEnumerator PoundingRoutine()
    {
        float elapsed = 0f;


        while (
            elapsed < poundingTime
        )
        {
            if (
                yamState !=
                PoundedYamState.Pounding
            )
            {
                yield break;
            }


            elapsed +=
                Time.deltaTime;


            yield return null;
        }


        poundingCoroutine = null;


        PoundingFinished();
    }


    // =========================================================
    // POUNDING FINISHED
    //
    // POUNDING OFF
    // EMPTY MORTAR OFF
    // READY MORTAR ON
    //
    // IMPORTANT:
    // NOTHING GOES TO PLATE YET
    // =========================================================

    private void PoundingFinished()
    {
        Debug.Log("==============================");
        Debug.Log("🥔 POUNDING FINISHED");
        Debug.Log("🔨 POUNDING YAMS = OFF");
        Debug.Log("🔨 EMPTY MORTAR = OFF");
        Debug.Log("🥔 READY MORTAR = ON");
        Debug.Log("⏸ WAITING FOR CLICK");
        Debug.Log("==============================");


        yamState =
            PoundedYamState.Pounded;


        currentStep = 4;


        // -----------------------------------------------------
        // POUNDING OFF
        // -----------------------------------------------------

        SetActive(
            poundingYam,
            false
        );


        // -----------------------------------------------------
        // EMPTY MORTAR OFF
        // -----------------------------------------------------

        SetActive(
            mortarEmpty,
            false
        );


        // -----------------------------------------------------
        // READY MORTAR ON
        //
        // Kinu + pounded yam
        // -----------------------------------------------------

        SetActive(
            readyPoundedYam,
            true
        );


        // -----------------------------------------------------
        // POUNDED YAM ON PLATE MUST STAY OFF
        //
        // NO AUTOMATIC PLATE
        // -----------------------------------------------------

        SetActive(
            poundedYamOnPlate,
            false
        );


        // -----------------------------------------------------
        // CURSOR → READY MORTAR
        // -----------------------------------------------------

        ShowCursorForCurrentStep();


        Debug.Log(
            "🥔 READY MORTAR IS WAITING FOR CLICK"
        );
    }


    // =========================================================
    // STEP 4
    // CLICK READY MORTAR
    //
    // READY MORTAR OFF
    // EMPTY MORTAR ON
    // POUNDED YAM ONLY ON PLATE
    // =========================================================

    public void ClickReadyPoundedYam()
    {
        if (
            currentStep != 4 ||
            yamState !=
            PoundedYamState.Pounded
        )
        {
            Debug.Log(
                "READY MORTAR CLICK IGNORED"
            );

            return;
        }


        Debug.Log("==============================");
        Debug.Log("🔨 READY MORTAR CLICKED");
        Debug.Log("🥔 READY MORTAR = OFF");
        Debug.Log("🔨 EMPTY MORTAR = ON");
        Debug.Log("🥔 POUNDED YAM ONLY = ON");
        Debug.Log("🍽 PLATE 2");
        Debug.Log("==============================");


        yamState =
            PoundedYamState.ReadyToServe;


        currentStep = 5;


        // -----------------------------------------------------
        // READY MORTAR OFF
        // -----------------------------------------------------

        SetActive(
            readyPoundedYam,
            false
        );


        // -----------------------------------------------------
        // EMPTY MORTAR RETURNS
        // -----------------------------------------------------

        SetActive(
            mortarEmpty,
            true
        );


        // -----------------------------------------------------
        // POUNDED YAM ONLY ON
        // -----------------------------------------------------

        SetActive(
            poundedYamOnPlate,
            true
        );


        // -----------------------------------------------------
        // PUT YAM ON PLATE 2
        // -----------------------------------------------------

        if (
            poundedYamOnPlate != null &&
            plate2 != null
        )
        {
            poundedYamOnPlate.transform.position =
                plate2.transform.position +
                new Vector3(
                    0f,
                    0.15f,
                    -0.1f
                );
        }


        // -----------------------------------------------------
        // CURSOR OFF
        // -----------------------------------------------------

        SetActive(
            handCursor,
            false
        );


        Debug.Log(
            "🥔 POUNDED YAM ONLY IS NOW ON PLATE 2"
        );
    }


    // =========================================================
    // COMPATIBILITY METHOD
    //
    // Kama script yako ya zamani inaita:
    //
    // ServePoundedYam()
    //
    // haitatoa error.
    // =========================================================

    public void ServePoundedYam()
    {
        ClickReadyPoundedYam();
    }


    // =========================================================
    // STEP 5
    // SERVE FROM PLATE 2
    // =========================================================

    public void ServeFromPlate2()
    {
        if (
            currentStep != 5 ||
            yamState !=
            PoundedYamState.ReadyToServe
        )
        {
            Debug.Log(
                "PLATE 2 SERVE IGNORED"
            );

            return;
        }


        Debug.Log("==============================");
        Debug.Log("🍽 PLATE 2 CLICKED");
        Debug.Log("🥔 POUNDED YAM SERVED");
        Debug.Log("==============================");


        yamState =
            PoundedYamState.Served;


        currentStep = 6;


        // -----------------------------------------------------
        // YAM ON PLATE OFF AFTER SERVING
        // -----------------------------------------------------

        SetActive(
            poundedYamOnPlate,
            false
        );


        // -----------------------------------------------------
        // EMPTY MORTAR STAYS ON
        // -----------------------------------------------------

        SetActive(
            mortarEmpty,
            true
        );


        // -----------------------------------------------------
        // CURSOR OFF
        // -----------------------------------------------------

        SetActive(
            handCursor,
            false
        );


        // -----------------------------------------------------
        // SERVE CUSTOMER
        // -----------------------------------------------------

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

                target = yamRaw;

                break;


            case 2:

                target = pot;

                break;


            case 4:

                target = readyPoundedYam;

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
    }
}