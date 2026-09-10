using UnityEngine;

public class CoconutClick : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Collider2D col;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    // 🔥 Call this to reset for next customer
    public void ResetCoconut()
    {
        if (sprite != null) sprite.enabled = true;
        if (col != null) col.enabled = true;
    }

    void OnMouseDown()
    {
        if (sprite != null) sprite.enabled = false;
        if (col != null) col.enabled = false;

        CustomerMenuManager manager = Object.FindFirstObjectByType<CustomerMenuManager>();
        if (manager != null)
            manager.ServePlate1();
    }
}