using UnityEngine;

public class EgusiSeedButton : MonoBehaviour
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

    public void ClickSeed()
    {
        if (egusiManager == null)
        {
            Debug.LogError(
                "EgusiCookingManager NOT FOUND!"
            );

            return;
        }

        egusiManager.ClickEgusiSeeds();
    }

    public void OnClick()
    {
        ClickSeed();
    }
}