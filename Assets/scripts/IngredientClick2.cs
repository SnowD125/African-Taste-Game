using UnityEngine;

public class IngredientClick2 : MonoBehaviour
{
    public enum IngredientType
    {
        Tomato,
        Onion,
        Pepper,
        BlackPepper,
        Garlic,
        BayLeaves,
        OilBottle,
        Blender,
        Meat,
        Rice,
        Pan
    }

    [Header("Ingredient Type")]
    public IngredientType ingredientType;

    [Header("Jollof Manager")]
    public JollofCookingManager jollofManager;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (jollofManager == null)
        {
            jollofManager =
                Object.FindFirstObjectByType<JollofCookingManager>();
        }
    }


    // =========================================================
    // DIRECT MOUSE CLICK
    // =========================================================

    private void OnMouseDown()
    {
        TriggerClick();
    }


    // =========================================================
    // CLICK DETECTOR SUPPORT
    // =========================================================

    public void TriggerClick()
    {
        Debug.Log(
            "🍅 INGREDIENT CLICKED: "
            + gameObject.name
            + " | TYPE: "
            + ingredientType
        );


        // -----------------------------------------------------
        // FIND MANAGER IF MISSING
        // -----------------------------------------------------

        if (jollofManager == null)
        {
            jollofManager =
                Object.FindFirstObjectByType<JollofCookingManager>();
        }


        if (jollofManager == null)
        {
            Debug.LogError(
                "❌ JollofCookingManager is NULL!"
            );

            return;
        }


        // =====================================================
        // INGREDIENT ACTION
        // =====================================================

        switch (ingredientType)
        {
            case IngredientType.Tomato:

                jollofManager.ClickTomato();

                break;


            case IngredientType.Onion:

                jollofManager.ClickOnion();

                break;


            case IngredientType.Pepper:

                jollofManager.ClickPepper();

                break;


            case IngredientType.BlackPepper:

                jollofManager.ClickBlackPepper();

                break;


            case IngredientType.Garlic:

                jollofManager.ClickGarlic();

                break;


            case IngredientType.BayLeaves:

                jollofManager.ClickBayLeaves();

                break;


            case IngredientType.OilBottle:

                jollofManager.ClickOilBottle();

                break;


            case IngredientType.Blender:

                jollofManager.ClickBlender();

                break;


            case IngredientType.Meat:

                jollofManager.ClickMeat();

                break;


            case IngredientType.Rice:

                jollofManager.ClickRice();

                break;


            case IngredientType.Pan:

                jollofManager.ClickPan();

                break;


            default:

                Debug.LogWarning(
                    "Unknown ingredient type!"
                );

                break;
        }
    }
}