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
        HideAll();
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

    public void ShowUgaliIngredients()
    {
        if (unga != null) unga.SetActive(true);
    }

    public void ShowAnchovesIngredients()
    {
        if (oil != null) oil.SetActive(true);
        if (dagaa != null) dagaa.SetActive(false);
        if (tomato != null) tomato.SetActive(false);
        if (onion != null) onion.SetActive(false);
    }

    public void ShowPotatoLeavesIngredients()
    {
        if (oil != null) oil.SetActive(true);
        if (sweetPotatoLeaves != null) sweetPotatoLeaves.SetActive(false);
        if (tomato != null) tomato.SetActive(false);
        if (onion != null) onion.SetActive(false);
    }

    public void ShowCoconut()
    {
        if (coconutFruit != null) coconutFruit.SetActive(true);
    }
}