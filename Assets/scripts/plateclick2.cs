using UnityEngine;

public class PlateClick2 : MonoBehaviour
{
    public JollofCookingManager jollofManager;

    void Awake()
    {
        if (jollofManager == null)
        {
            jollofManager =
                Object.FindFirstObjectByType<JollofCookingManager>();
        }
    }

    public void ClickPlate()
    {
        Debug.Log("🍽 PLATE 1 CLICKED");

        if (jollofManager == null)
        {
            Debug.LogError(
                "JollofCookingManager is NULL!"
            );

            return;
        }

        // Only serve when Jollof is actually on Plate 1
        if (
            jollofManager.jollofState !=
            JollofCookingManager.CookState.Served
        )
        {
            Debug.Log(
                "❌ Jollof is not ready on Plate 1."
            );

            return;
        }

        Debug.Log(
            "🍚 SERVING JOLLOF FROM PLATE 1"
        );

        // Keep the original serving system
        jollofManager.ServePlate();
    }

    void OnMouseDown()
    {
        ClickPlate();
    }
}