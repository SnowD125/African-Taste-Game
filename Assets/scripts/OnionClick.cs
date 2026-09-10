using UnityEngine;

public class OnionClick : MonoBehaviour
{
    public VegCookingManager manager;

    void OnMouseDown()
    {
        manager.ClickOnion();
    }
}