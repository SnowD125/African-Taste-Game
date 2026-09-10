using UnityEngine;

public class TembeleClick : MonoBehaviour
{
    public VegCookingManager manager;

    void OnMouseDown()
    {
        manager.ClickTembele();
    }
}