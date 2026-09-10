using UnityEngine;

public class PoundedYamPotClick : MonoBehaviour
{
    public PoundedYamCookingManager poundedYamManager;

    private void Awake()
    {
        if (poundedYamManager == null)
        {
            poundedYamManager =
                Object.FindFirstObjectByType<PoundedYamCookingManager>();
        }
    }

    private void OnMouseDown()
    {
        OnClick();
    }

    public void OnClick()
    {
        if (poundedYamManager == null)
        {
            poundedYamManager =
                Object.FindFirstObjectByType<PoundedYamCookingManager>();
        }

        if (poundedYamManager == null)
        {
            Debug.LogError(
                "POUNDED YAM MANAGER NOT FOUND!"
            );

            return;
        }

        Debug.Log(
            "🍲 POT CLICKED"
        );

        poundedYamManager.ClickPot();
    }
}