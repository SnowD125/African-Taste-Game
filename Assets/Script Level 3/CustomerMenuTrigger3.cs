using UnityEngine;

public class CustomerMenuTrigger3 : MonoBehaviour
{
    public CustomerMenuManager3 menuManager;
    public GameObject orderCanvas;

    void Awake()
    {
        if (menuManager == null)
            menuManager = Object.FindFirstObjectByType<CustomerMenuManager3>();

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