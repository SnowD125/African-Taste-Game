using UnityEngine;

public class PotClick : MonoBehaviour
{
    public CookingManager cookingManager;

    void OnMouseDown()
    {
        if (cookingManager != null)
        {
            cookingManager.StartUgaliCooking(); // 🔥 Changed from ServeFood to StartUgaliCooking
        }
    }
}