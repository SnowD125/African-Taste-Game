using UnityEngine;

public class IngredientManager2 : MonoBehaviour
{
    [Header("Jollof Rice Ingredients")]
    public GameObject tomato;
    public GameObject onion;
    public GameObject pepper;
    public GameObject rice;
    public GameObject meat;
    public GameObject oil;
    public GameObject chillPowder;
    public GameObject bouillonCube;
    public GameObject salt;
    public GameObject bayLeaves;
    public GameObject blackPepper;
    public GameObject garlic;

    [Header("Egusi Soup Ingredients")]
    public GameObject egusiSeeds;
    public GameObject vegetable;
    public GameObject palmOil;

    [Header("Pounded Yam Ingredients")]
    public GameObject yam;

    void Start()
    {
        // Ingredients zinaonekana tokea mwanzo
    }

    public void HideAll()
    {
        tomato?.SetActive(false);
        onion?.SetActive(false);
        pepper?.SetActive(false);
        rice?.SetActive(false);
        meat?.SetActive(false);
        oil?.SetActive(false);
        chillPowder?.SetActive(false);
        bouillonCube?.SetActive(false);
        salt?.SetActive(false);
        bayLeaves?.SetActive(false);
        blackPepper?.SetActive(false);
        garlic?.SetActive(false);
        egusiSeeds?.SetActive(false);
        vegetable?.SetActive(false);
        palmOil?.SetActive(false);
        yam?.SetActive(false);
    }

    public void ShowJollofIngredients()
    {
        HideAll();
        tomato?.SetActive(true);
        onion?.SetActive(true);
        pepper?.SetActive(true);
        rice?.SetActive(true);
        meat?.SetActive(true);
        oil?.SetActive(true);
        chillPowder?.SetActive(true);
        bouillonCube?.SetActive(true);
        salt?.SetActive(true);
        bayLeaves?.SetActive(true);
        blackPepper?.SetActive(true);
        garlic?.SetActive(true);
    }

    public void ShowEgusiIngredients()
    {
        HideAll();
        egusiSeeds?.SetActive(true);
        vegetable?.SetActive(true);
        palmOil?.SetActive(true);
        onion?.SetActive(true);
        pepper?.SetActive(true);
        meat?.SetActive(true);
    }

    public void ShowPoundedYamIngredients()
    {
        HideAll();
        yam?.SetActive(true);
    }
}