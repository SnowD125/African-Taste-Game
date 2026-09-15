using UnityEngine;

public class PotClick : MonoBehaviour
{
    public CookingManager cookingManager;

    void OnMouseDown()
    {
        if (cookingManager != null)
        {
            // Starts the cook, or takes the cooked ugali out of the pot.
            cookingManager.ClickPot();
        }
    }
}