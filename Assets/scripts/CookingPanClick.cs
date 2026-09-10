using UnityEngine;

public class CookingPanClick : MonoBehaviour
{
    public VegCookingManager manager;

    void OnMouseDown()
    {
        manager.TryServePan();
    }
}