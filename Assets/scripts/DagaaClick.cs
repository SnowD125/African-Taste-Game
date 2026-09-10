using UnityEngine;

public class DagaaClick : MonoBehaviour
{
    public VegCookingManager manager;

    void OnMouseDown()
    {
        manager.ClickDagaa();
    }
}