using UnityEngine;

public class CustomerMenuTrigger2 : MonoBehaviour
{
    public CustomerMenuManager2 menuManager;
    public GameObject orderCanvas;

    void Awake()
    {
        if (menuManager == null)
            menuManager = Object.FindFirstObjectByType<CustomerMenuManager2>();
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
        if (menuManager == null) return;
        menuManager.ServePlate1();
    }
}