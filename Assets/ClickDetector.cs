using UnityEngine;

public class ClickDetector : MonoBehaviour
{
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (Camera.main == null)
            return;

        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(
                Input.mousePosition
            );

        Vector2 mousePosition2D =
            new Vector2(
                mousePosition.x,
                mousePosition.y
            );

        RaycastHit2D hit =
            Physics2D.Raycast(
                mousePosition2D,
                Vector2.zero
            );

        if (!hit.collider)
            return;

        GameObject clickedObject =
            hit.collider.gameObject;

        // =========================================
        // SHARED PAN
        // =========================================

        SharedPanClick sharedPan =
            clickedObject.GetComponent<SharedPanClick>();

        if (sharedPan != null)
        {
            Debug.Log(
                "CLICK DETECTOR → SHARED PAN"
            );

            sharedPan.OnClick();

            return;
        }

        // =========================================
        // EGUSI READY
        // =========================================

        EgusiReadyClick2 readyClick =
            clickedObject.GetComponent<EgusiReadyClick2>();

        if (readyClick != null)
        {
            Debug.Log(
                "CLICK DETECTOR → READY EGUSI"
            );

            readyClick.OnClick();

            return;
        }

        // =========================================
        // EGUSI INGREDIENT
        // =========================================

        IngredientClickEgusi ingredient =
            clickedObject.GetComponent<IngredientClickEgusi>();

        if (ingredient != null)
        {
            Debug.Log(
                "CLICK DETECTOR → EGUSI INGREDIENT"
            );

            ingredient.OnClick();

            return;
        }

        Debug.Log(
            "CLICKED OBJECT: "
            + clickedObject.name
        );
    }
}