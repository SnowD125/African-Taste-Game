using UnityEngine;

public class IngredientManager : MonoBehaviour
{
    [Header("Ingredients")]
    public GameObject unga;
    public GameObject oil;
    public GameObject tomato;
    public GameObject onion;
    public GameObject dagaa;
    public GameObject sweetPotatoLeaves;
    public GameObject coconutFruit;

    void Start()
    {
        // All Level One ingredients are on the table from the start, like
        // Level Two / Three. Maize flour and oil have no step check of their
        // own -- before, being hidden was what kept them unusable -- so they
        // stay LOCKED (no clicks) until the order is chosen.
        ShowOnTable(unga, false);
        ShowOnTable(oil, false);
        ShowOnTable(tomato, true);
        ShowOnTable(onion, true);
        ShowOnTable(dagaa, true);
        ShowOnTable(sweetPotatoLeaves, true);
        if (coconutFruit != null) coconutFruit.SetActive(false);
    }

    /// <summary>Visible on the table; clickable only when usable.</summary>
    void ShowOnTable(GameObject ingredient, bool usable)
    {
        if (ingredient == null) return;

        ingredient.SetActive(true);

        Collider2D col = ingredient.GetComponent<Collider2D>();
        if (col != null) col.enabled = usable;
    }

    public void HideAll()
    {
        if (unga != null) unga.SetActive(false);
        if (oil != null) oil.SetActive(false);
        if (tomato != null) tomato.SetActive(false);
        if (onion != null) onion.SetActive(false);
        if (dagaa != null) dagaa.SetActive(false);
        if (sweetPotatoLeaves != null) sweetPotatoLeaves.SetActive(false);
        if (coconutFruit != null) coconutFruit.SetActive(false);
    }

    // Choosing the order now UNLOCKS the ingredient instead of revealing it.
    // Nothing is hidden: the other ingredients stay on the table, and the
    // VegCookingManager step checks still decide which click is accepted.

    public void ShowUgaliIngredients()
    {
        ShowOnTable(unga, true);
    }

    public void ShowAnchovesIngredients()
    {
        ShowOnTable(oil, true);
    }

    public void ShowPotatoLeavesIngredients()
    {
        ShowOnTable(oil, true);
    }

    public void ShowCoconut()
    {
        if (coconutFruit != null) coconutFruit.SetActive(true);
    }
}