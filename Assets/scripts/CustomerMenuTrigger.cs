using UnityEngine;

public class CustomerMenuTrigger : MonoBehaviour
{
    public CustomerMenuManager menuManager;
    public GameObject orderCanvas;

    void Awake()
    {
        if (menuManager == null)
            menuManager = Object.FindFirstObjectByType<CustomerMenuManager>();
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

        if (menuManager.selectedFood == 4)
            menuManager.ServePlate2();
        else
            menuManager.ServePlate1();
    }
}