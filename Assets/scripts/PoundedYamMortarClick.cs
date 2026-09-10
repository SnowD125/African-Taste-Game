using UnityEngine;

public class PoundedYamMortarClick : MonoBehaviour
{
    public PoundedYamCookingManager poundedYamManager;

    private void Awake()
    {
        if (poundedYamManager == null)
        {
            poundedYamManager =
                Object.FindFirstObjectByType<PoundedYamCookingManager>();
        }
    }

    private void OnMouseDown()
    {
        OnClick();
    }

    public void OnClick()
    {
        if (poundedYamManager == null)
        {
            poundedYamManager =
                Object.FindFirstObjectByType<PoundedYamCookingManager>();
        }

        if (poundedYamManager == null)
        {
            Debug.LogError(
                "POUNDED YAM COOKING MANAGER NOT FOUND!"
            );

            return;
        }

        // ONLY CLICK AFTER POUNDING FINISHES
        if (
            poundedYamManager.currentStep != 4 ||
            poundedYamManager.yamState !=
            PoundedYamCookingManager.PoundedYamState.Pounded
        )
        {
            Debug.Log(
                "READY MORTAR CLICK IGNORED"
            );

            return;
        }

        Debug.Log("==============================");
        Debug.Log("🔨 READY MORTAR CLICKED");
        Debug.Log("🥔 MOVING POUNDED YAM TO PLATE");
        Debug.Log("==============================");

        // =====================================================
        // CHANGE STATE
        // =====================================================

        poundedYamManager.yamState =
            PoundedYamCookingManager.PoundedYamState.ReadyToServe;

        poundedYamManager.currentStep = 5;


        // =====================================================
        // READY MORTAR OFF
        // =====================================================

        if (
            poundedYamManager.readyPoundedYam != null
        )
        {
            poundedYamManager.readyPoundedYam.SetActive(false);
        }


        // =====================================================
        // EMPTY MORTAR ON
        // =====================================================

        if (
            poundedYamManager.mortarEmpty != null
        )
        {
            poundedYamManager.mortarEmpty.SetActive(true);
        }


        // =====================================================
        // PLATE 2 REMAINS VISIBLE
        // =====================================================

        if (
            poundedYamManager.plate2 != null
        )
        {
            poundedYamManager.plate2.SetActive(true);
        }


        // =====================================================
        // POUNDED YAM ONLY → PLATE 2
        // =====================================================

        if (
            poundedYamManager.poundedYamOnPlate != null
        )
        {
            poundedYamManager.poundedYamOnPlate.SetActive(true);

            // Position on Plate 2
            poundedYamManager.poundedYamOnPlate.transform.position =
                poundedYamManager.plate2.transform.position;

            // Slightly above plate
            poundedYamManager.poundedYamOnPlate.transform.position +=
                new Vector3(0f, 0.15f, -0.1f);
        }
        else
        {
            Debug.LogError(
                "POUNDED YAM ON PLATE IS NOT ASSIGNED!"
            );
        }


        // =====================================================
        // CURSOR OFF
        // =====================================================

        if (
            poundedYamManager.handCursor != null
        )
        {
            poundedYamManager.handCursor.SetActive(false);
        }


        Debug.Log("==============================");
        Debug.Log("🔨 READY MORTAR = OFF");
        Debug.Log("🔨 EMPTY MORTAR = ON");
        Debug.Log("🥔 POUNDED YAM ONLY = ON PLATE 2");
        Debug.Log("==============================");
    }
}