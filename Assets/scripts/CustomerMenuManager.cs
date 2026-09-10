using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomerMenuManager : MonoBehaviour
{
    [Header("Menu Panel")]
    public GameObject menuPanel;
    public GameObject ugaliTick;
    public GameObject anchovesTick;
    public GameObject potatoTick;
    public GameObject coconutTick;

    [Header("Order Canvas")]
    public GameObject orderCanvas;
    public Image foodImage;
    public Sprite ugaliDagaaSprite;
    public Sprite potatoLeavesSprite;
    public Sprite ugaliTembeleSprite;
    public Sprite coconutSprite;

    [Header("Served UI")]
    public GameObject servedTick;
    public GameObject servedText;

    [Header("Ingredient Manager")]
    public IngredientManager ingredientManager;

    [Header("Plates")]
    public GameObject plate1;
    public GameObject plate2;

    [Header("Plate2 Foods (Combo)")]
    public GameObject readyUgali2;
    public GameObject readyTembele;

    [HideInInspector] public int selectedFood = -1;
    [HideInInspector] public bool isComboOrder = false;

    bool hasUgali = false;
    bool hasPotato = false;

    void Awake()
    {
        ResetAll();
    }

    public void ResetAll()
    {
        CancelInvoke(nameof(HideMenu));

        menuPanel?.SetActive(false);
        orderCanvas?.SetActive(false);
        servedTick?.SetActive(false);
        servedText?.SetActive(false);
        ugaliTick?.SetActive(false);
        anchovesTick?.SetActive(false);
        potatoTick?.SetActive(false);
        coconutTick?.SetActive(false);
        if (foodImage != null) foodImage.gameObject.SetActive(false);

        hasUgali = false;
        hasPotato = false;
        selectedFood = -1;
        isComboOrder = false;
    }

    public void ShowMenu()
    {
        CancelInvoke(nameof(HideMenu));
        ugaliTick?.SetActive(false);
        anchovesTick?.SetActive(false);
        potatoTick?.SetActive(false);
        coconutTick?.SetActive(false);
        orderCanvas?.SetActive(false);
        hasUgali = false;
        hasPotato = false;
        selectedFood = -1;
        isComboOrder = false;

        if (menuPanel == null)
            Debug.LogWarning("ShowMenu — menuPanel is NULL! Jaza field kwenye Inspector.");
        else
        {
            menuPanel.SetActive(true);
            Debug.Log($"ShowMenu — menuPanel activeSelf={menuPanel.activeSelf}, activeInHierarchy={menuPanel.activeInHierarchy}");
        }
    }

    public void SelectUgali()
    {
        selectedFood = 0;
        hasUgali = true;
        ugaliTick?.SetActive(true);
        anchovesTick?.SetActive(false);
        potatoTick?.SetActive(false);
        coconutTick?.SetActive(false);
        ingredientManager?.ShowUgaliIngredients();
        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);

        if (hasPotato)
        {
            isComboOrder = true;
            selectedFood = 4;
        }
    }

    public void SelectAnchoves()
    {
        selectedFood = 0;
        anchovesTick?.SetActive(true);
        ugaliTick?.SetActive(false);
        potatoTick?.SetActive(false);
        coconutTick?.SetActive(false);
        isComboOrder = false;
        ingredientManager?.ShowAnchovesIngredients();
        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);
    }

    public void SelectPotatoLeaves()
    {
        selectedFood = 1;
        hasPotato = true;
        potatoTick?.SetActive(true);
        ugaliTick?.SetActive(false);
        anchovesTick?.SetActive(false);
        coconutTick?.SetActive(false);
        ingredientManager?.ShowPotatoLeavesIngredients();
        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);

        if (hasUgali)
        {
            isComboOrder = true;
            selectedFood = 4;
        }
    }

    public void SelectCoconut()
    {
        selectedFood = 2;
        coconutTick?.SetActive(true);
        ugaliTick?.SetActive(false);
        anchovesTick?.SetActive(false);
        potatoTick?.SetActive(false);
        ingredientManager?.ShowCoconut();
        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);
    }

    void HideMenu()
    {
        menuPanel?.SetActive(false);
        orderCanvas?.SetActive(true);

        if (foodImage != null)
        {
            foodImage.gameObject.SetActive(true);
            if (hasUgali && hasPotato)
            {
                selectedFood = 4;
                isComboOrder = true;
                foodImage.sprite = ugaliTembeleSprite;
            }
            else if (selectedFood == 4)
            {
                isComboOrder = true;
                foodImage.sprite = ugaliTembeleSprite;
            }
            else if (selectedFood == 0)
            {
                isComboOrder = false;
                foodImage.sprite = ugaliDagaaSprite;
            }
            else if (selectedFood == 1)
            {
                isComboOrder = false;
                foodImage.sprite = potatoLeavesSprite;
            }
            else if (selectedFood == 2)
            {
                isComboOrder = false;
                foodImage.sprite = coconutSprite;
            }
        }

        hasUgali = false;
        hasPotato = false;
    }

    public void ServePlate1()
    {
        plate1?.SetActive(false);
        readyUgali2?.SetActive(false);
        readyTembele?.SetActive(false);
        foodImage?.gameObject.SetActive(false);
        servedTick?.SetActive(true);
        servedText?.SetActive(true);
        StartCoroutine(ServeRoutine());
    }

    public void ServePlate2()
    {
        plate2?.SetActive(false);
        readyUgali2?.SetActive(false);
        readyTembele?.SetActive(false);
        foodImage?.gameObject.SetActive(false);
        servedTick?.SetActive(true);
        servedText?.SetActive(true);
        StartCoroutine(ServeRoutine());
    }

    private IEnumerator ServeRoutine()
    {
        yield return new WaitForSeconds(2f);

        servedTick?.SetActive(false);
        servedText?.SetActive(false);
        orderCanvas?.SetActive(false);

        FirstCustomer[] allCustomers = Object.FindObjectsByType<FirstCustomer>(FindObjectsSortMode.None);
        FirstCustomer activeCustomer = null;
        foreach (var c in allCustomers)
        {
            if (c.gameObject.activeInHierarchy && !c.IsLeaving)
            {
                activeCustomer = c;
                break;
            }
        }

        activeCustomer?.ServeCustomer();
        selectedFood = -1;
        isComboOrder = false;
    }
}