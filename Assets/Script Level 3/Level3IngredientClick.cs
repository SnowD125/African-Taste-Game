using UnityEngine;

/// <summary>
/// Level Three (Ghana) only. Forwards a click on an ingredient, cooking
/// station or plate to Level3CookingManager, which decides whether it is the
/// step the recipe is waiting for. A click that is not is ignored there.
/// </summary>
public class Level3IngredientClick : MonoBehaviour
{
    public Level3CookingManager.Target target;
    public Level3CookingManager manager;

    void OnMouseDown()
    {
        if (manager == null)
            manager = Object.FindFirstObjectByType<Level3CookingManager>();

        if (manager != null)
            manager.Click(target);
    }
}
