using UnityEngine;

public class EgusiClickDetector : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;


        if (Camera.main == null)
            return;


        Ray ray =
            Camera.main.ScreenPointToRay(
                Input.mousePosition
            );


        RaycastHit2D[] hits =
            Physics2D.GetRayIntersectionAll(ray);


        if (hits == null || hits.Length == 0)
            return;


        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
                continue;


            GameObject clickedObject =
                hit.collider.gameObject;


            // =================================================
            // INGREDIENT
            // =================================================

            IngredientClickEgusi ingredient =
                clickedObject
                .GetComponent<IngredientClickEgusi>();


            if (ingredient == null)
            {
                ingredient =
                    clickedObject
                    .GetComponentInParent<IngredientClickEgusi>();
            }


            if (ingredient != null)
            {
                Debug.Log(
                    "EGUSI CLICK DETECTOR → "
                    + ingredient.gameObject.name
                );


                ingredient.OnClick();

                return;
            }


            // =================================================
            // EGUSI MANAGER DIRECT
            // =================================================

            EgusiCookingManager manager =
                clickedObject
                .GetComponent<EgusiCookingManager>();


            if (manager == null)
            {
                manager =
                    clickedObject
                    .GetComponentInParent<EgusiCookingManager>();
            }


            if (manager != null)
            {
                if (manager.currentStep == 7)
                {
                    manager.ClickReadyEgusi();

                    return;
                }
            }
        }
    }
}