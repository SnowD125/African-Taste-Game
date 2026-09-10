using UnityEngine;

public class TomatoClick : MonoBehaviour
{
    public VegCookingManager manager;

    void OnMouseDown()
    {
        manager.ClickTomato();
    }
}