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

    [Header("Order Canvas")]
    public GameObject orderCanvas;
    public Image foodImage;
    public Sprite ugaliDagaaSprite;
    public Sprite potatoLeavesSprite;
    public Sprite ugaliTembeleSprite;

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

    // ---------------------------------------------------------------------
    // SERVED-DISH IDENTITY  (additive - nothing else reads these)
    //
    // selectedFood cannot answer "what did we serve?": SelectUgali() and
    // SelectAnchoves() both set it to 0, and the delayed Invoke(HideMenu, 5f)
    // can rewrite it after the player has chosen. These two fields give the
    // carried plate a stable identity without changing selectedFood,
    // isComboOrder, the ticks, the buttons or any cooking path.
    // ---------------------------------------------------------------------

    /// <summary>The dish the player last picked, updated by the Select* methods.</summary>
    [HideInInspector] public LevelOneDish pickedDish = LevelOneDish.None;

    /// <summary>Latched by the plate at the moment it serves. This is what the customer carries.</summary>
    [HideInInspector] public LevelOneDish servedDish = LevelOneDish.None;

    bool hasUgali = false;
    bool hasPotato = false;
    bool hasAnchoves = false;

    /// <summary>
    /// True only once a VALID Level One combo has been assembled:
    /// Ugali + Anchovies, or Ugali + Potato Leaves.
    ///
    /// A single ingredient on its own is not a valid order, and must produce no
    /// carried plate and no coins. Deliberately NOT cleared by HideMenu(), which
    /// wipes hasUgali/hasPotato five seconds after the last tap -- this flag has
    /// to survive until the customer is actually served.
    /// </summary>
    [HideInInspector] public bool isValidOrder = false;

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
        if (foodImage != null) foodImage.gameObject.SetActive(false);

        hasUgali = false;
        hasPotato = false;
        selectedFood = -1;
        isComboOrder = false;
        pickedDish = LevelOneDish.None;
        servedDish = LevelOneDish.None;
        hasAnchoves = false;
        isValidOrder = false;
    }

    public void ShowMenu()
    {
        CancelInvoke(nameof(HideMenu));
        ugaliTick?.SetActive(false);
        anchovesTick?.SetActive(false);
        potatoTick?.SetActive(false);
        orderCanvas?.SetActive(false);
        hasUgali = false;
        hasPotato = false;
        selectedFood = -1;
        isComboOrder = false;
        pickedDish = LevelOneDish.None;
        servedDish = LevelOneDish.None;
        hasAnchoves = false;
        isValidOrder = false;

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
        pickedDish = LevelOneDish.Ugali;
        ugaliTick?.SetActive(true);
        anchovesTick?.SetActive(false);
        potatoTick?.SetActive(false);
        ingredientManager?.ShowUgaliIngredients();
        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);

        if (hasAnchoves || hasPotato)
        {
            isValidOrder = true;
        }

        if (hasPotato)
        {
            isComboOrder = true;
            selectedFood = 4;
            pickedDish = LevelOneDish.UgaliTembele;
        }
    }

    public void SelectAnchoves()
    {
        selectedFood = 0;
        pickedDish = LevelOneDish.Anchoves;
        hasAnchoves = true;
        anchovesTick?.SetActive(true);
        ugaliTick?.SetActive(false);
        potatoTick?.SetActive(false);
        isComboOrder = false;

        if (hasUgali)
        {
            isValidOrder = true;
        }

        ingredientManager?.ShowAnchovesIngredients();
        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);
    }

    public void SelectPotatoLeaves()
    {
        selectedFood = 1;
        hasPotato = true;
        pickedDish = LevelOneDish.PotatoLeaves;
        potatoTick?.SetActive(true);
        ugaliTick?.SetActive(false);
        anchovesTick?.SetActive(false);
        ingredientManager?.ShowPotatoLeavesIngredients();
        CancelInvoke(nameof(HideMenu));
        Invoke(nameof(HideMenu), 5f);

        if (hasUgali)
        {
            isValidOrder = true;
            isComboOrder = true;
            selectedFood = 4;
            pickedDish = LevelOneDish.UgaliTembele;
        }
    }


    void HideMenu()
    {
        menuPanel?.SetActive(false);

        // The order/result box belongs to a real order. A single ingredient is
        // not one, so the WHOLE OrderCanvas stays off -- including its child
        // OrderPanel, which is a cream-coloured Image with no sprite and would
        // otherwise show as an empty beige box.
        //
        // isValidOrder is set the moment the second item completes the combo,
        // so this is not a timing race: the 5s Invoke is restarted by every tap.
        if (!isValidOrder)
        {
            servedTick?.SetActive(false);
            servedText?.SetActive(false);

            if (foodImage != null)
                foodImage.gameObject.SetActive(false);

            orderCanvas?.SetActive(false);

            hasUgali = false;
            hasPotato = false;

            Debug.Log("Single-item selection — order/result box stays hidden.");

            return;
        }

        orderCanvas?.SetActive(true);

        if (foodImage != null)
        {
            foodImage.gameObject.SetActive(true);
            if (hasUgali && hasPotato)
            {
                selectedFood = 4;
                isComboOrder = true;
                pickedDish = LevelOneDish.UgaliTembele;
                foodImage.sprite = ugaliTembeleSprite;
            }
            else if (selectedFood == 4)
            {
                isComboOrder = true;
                pickedDish = LevelOneDish.UgaliTembele;
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
        }

        hasUgali = false;
        hasPotato = false;
    }

    // =========================================================
    // SERVED-DISH INTERFACE
    //
    // The plates call LatchServedDish() at the instant they commit a serve.
    // Latching means a later Invoke(HideMenu, 5f) can no longer change what the
    // customer walks away holding: the plate identity is frozen at serve time.
    // =========================================================

    /// <summary>Resolves the current order into a dish identity.</summary>
    public LevelOneDish CurrentDish()
    {
        if (isComboOrder || selectedFood == 4)
            return LevelOneDish.UgaliTembele;

        if (pickedDish != LevelOneDish.None)
            return pickedDish;

        if (selectedFood == 1)
            return LevelOneDish.PotatoLeaves;

        if (selectedFood == 0)
            return LevelOneDish.Ugali;

        return LevelOneDish.None;
    }

    /// <summary>Freezes what was served, from the current order. Called by plate 1.</summary>
    public LevelOneDish LatchServedDish()
    {
        return LatchServedDish(CurrentDish());
    }

    /// <summary>Freezes what was served. Plate 2 passes the combo explicitly.</summary>
    public LevelOneDish LatchServedDish(LevelOneDish dish)
    {
        servedDish = dish;

        Debug.Log("SERVED DISH LATCHED = " + servedDish);

        return servedDish;
    }

    /// <summary>
    /// An invalid single-item serve: take the ENTIRE order/result box off screen.
    ///
    /// Hiding only the contents is not enough. orderCanvas' child OrderPanel is a
    /// plain cream-coloured Image with no sprite, so leaving the canvas active
    /// leaves an empty beige box on screen until the serve coroutine switches it
    /// off two seconds later.
    ///
    /// Also cancels the pending Invoke(HideMenu, 5f): HideMenu is the only thing
    /// in Level One that switches orderCanvas back on, and it would otherwise
    /// re-show the box after the serve. menuPanel is closed here because that is
    /// the other thing HideMenu would have done.
    /// </summary>
    public void HideOrderBoxForInvalidOrder()
    {
        CancelInvoke(nameof(HideMenu));

        menuPanel?.SetActive(false);
        servedTick?.SetActive(false);
        servedText?.SetActive(false);

        if (foodImage != null)
            foodImage.gameObject.SetActive(false);

        orderCanvas?.SetActive(false);

        Debug.Log("Invalid single-item order — order/result box hidden.");
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