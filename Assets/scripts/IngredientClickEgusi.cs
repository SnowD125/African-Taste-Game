using UnityEngine;

public class IngredientClickEgusi : MonoBehaviour
{
    public enum IngredientType
    {
        PalmOil,
        Onion,
        EgusiSeeds,
        Meat,
        Vegetable,
        Salt
    }


    [Header("Ingredient Type")]
    public IngredientType ingredientType;


    [Header("Egusi Manager")]
    public EgusiCookingManager egusiManager;


    void Awake()
    {
        if (egusiManager == null)
        {
            egusiManager =
                Object.FindFirstObjectByType<EgusiCookingManager>();
        }
    }


    // =========================================================
    // NORMAL MOUSE CLICK
    // =========================================================

    void OnMouseDown()
    {
        OnClick();
    }


    // =========================================================
    // CLICK
    // =========================================================

    public void OnClick()
    {
        if (egusiManager == null)
        {
            egusiManager =
                Object.FindFirstObjectByType<EgusiCookingManager>();
        }


        if (egusiManager == null)
        {
            Debug.LogError(
                "EgusiCookingManager NOT FOUND!"
            );

            return;
        }


        Debug.Log(
            "CLICKED INGREDIENT: "
            + gameObject.name
            + " | TYPE: "
            + ingredientType
        );


        switch (ingredientType)
        {
            case IngredientType.PalmOil:

                egusiManager.ClickPalmOil();

                break;


            case IngredientType.Onion:

                egusiManager.ClickOnion();

                break;


            case IngredientType.EgusiSeeds:

                egusiManager.ClickEgusiSeeds();

                break;


            case IngredientType.Meat:

                egusiManager.ClickMeat();

                break;


            case IngredientType.Vegetable:

                egusiManager.ClickVegetable();

                break;


            case IngredientType.Salt:

                egusiManager.ClickSalt();

                break;
        }
    }
}