using UnityEngine;

public class BlenderClick : MonoBehaviour
{
    public JollofCookingManager jollofManager;

    void Awake()
    {
        if (jollofManager == null)
        {
            jollofManager = Object.FindFirstObjectByType<JollofCookingManager>();
        }
    }

    void OnMouseDown()
    {
        Debug.Log("BlenderClick OnMouseDown");

        if (jollofManager != null)
        {
            jollofManager.ClickBlender();
        }
    }

    public void OnClick()
    {
        Debug.Log("BlenderClick OnClick");

        if (jollofManager != null)
        {
            jollofManager.ClickBlender();
        }
    }
}