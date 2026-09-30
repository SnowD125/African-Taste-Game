using UnityEngine;

public class CustomerMenuTrigger4 : MonoBehaviour
{
    public CustomerMenuManager4 menuManager;
    public GameObject orderCanvas;

    void Awake()
    {
        if (menuManager == null)
            menuManager = Object.FindFirstObjectByType<CustomerMenuManager4>();

        if (orderCanvas == null && menuManager != null)
            orderCanvas = menuManager.orderCanvas;
    }

    public void ShowOrder()
    {
        if (menuManager != null)
            menuManager.ShowMenu();
    }

    public void HideOrder()
    {
        if (menuManager != null)
            menuManager.ResetAll();
    }

    public void ShowServed()
    {
        if (menuManager == null)
            return;

        menuManager.ServePlate1();
    }
}