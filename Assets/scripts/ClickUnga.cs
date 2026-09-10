using UnityEngine;

public class ClickUnga : MonoBehaviour
{
    public GameObject fallingFlour;
    public float visibleTime = 1f;
    public CookingManager cookingManager;

    void OnMouseDown()
    {
        Debug.Log("Maize flour clicked — cookingManager=" + cookingManager);

        if (fallingFlour != null)
        {
            fallingFlour.SetActive(true);
            CancelInvoke(nameof(HideFlour));
            Invoke(nameof(HideFlour), visibleTime);
        }

        if (cookingManager != null)
        {
            cookingManager.StartUgaliCooking();
        }
        else
        {
            Debug.LogError("cookingManager IS NULL in ClickUnga");
        }
    }

    void HideFlour()
    {
        if (fallingFlour != null)
            fallingFlour.SetActive(false);
    }
}