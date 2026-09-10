using UnityEngine;

public class SharedPanClick : MonoBehaviour
{
    private JollofCookingManager jollofManager;
    private EgusiCookingManager egusiManager;


    private void Awake()
    {
        jollofManager =
            Object.FindFirstObjectByType<JollofCookingManager>();

        egusiManager =
            Object.FindFirstObjectByType<EgusiCookingManager>();
    }


    private void OnMouseDown()
    {
        HandlePanClick();
    }


    public void OnClick()
    {
        HandlePanClick();
    }


    private void HandlePanClick()
    {
        // =====================================================
        // EGUSI
        // =====================================================

        if (egusiManager != null)
        {
            if (
                egusiManager.egusiState ==
                EgusiCookingManager.CookState.ReadyToServe
            )
            {
                Debug.Log(
                    "🍲 SHARED PAN CLICK → EGUSI"
                );

                egusiManager.MoveEgusiToReady();

                return;
            }

            if (
                egusiManager.egusiState ==
                EgusiCookingManager.CookState.Burned
            )
            {
                Debug.Log(
                    "🔥 SHARED PAN CLICK → EGUSI BURNED"
                );

                return;
            }
        }


        // =====================================================
        // JOLLOF
        // =====================================================

        if (jollofManager != null)
        {
            if (
                jollofManager.jollofState ==
                JollofCookingManager.CookState.ReadyToServe
            )
            {
                Debug.Log(
                    "🍚 SHARED PAN CLICK → JOLLOF"
                );

                // IMPORTANT:
                // Tell Jollof Manager that this is a REAL
                // user pan click.
                jollofManager.AllowPanTransfer();

                return;
            }


            if (
                jollofManager.jollofState ==
                JollofCookingManager.CookState.Burned
            )
            {
                Debug.Log(
                    "🔥 SHARED PAN CLICK → JOLLOF BURNED"
                );

                return;
            }
        }


        Debug.Log(
            "SHARED PAN CLICK IGNORED — FOOD NOT READY"
        );
    }
}