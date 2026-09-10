using UnityEngine;

public class EgusiReadyClick2 : MonoBehaviour
{
    public EgusiCookingManager egusiManager;

    private void Awake()
    {
        if (egusiManager == null)
        {
            egusiManager =
                Object.FindFirstObjectByType<EgusiCookingManager>();
        }
    }

    private void OnMouseDown()
    {
        ServeEgusi();
    }

    public void OnClick()
    {
        ServeEgusi();
    }

    private void ServeEgusi()
    {
        if (egusiManager == null)
        {
            egusiManager =
                Object.FindFirstObjectByType<EgusiCookingManager>();
        }

        if (egusiManager == null)
        {
            Debug.LogError(
                "EGUSI COOKING MANAGER NOT FOUND!"
            );

            return;
        }

        Debug.Log(
            "🍲 READY EGUSI CLICKED!"
        );

        egusiManager.ClickReadyEgusi();
    }
}