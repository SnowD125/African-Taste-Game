using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

[System.Serializable]
public class FoodMenuLevelData
{
    [Header("Dishes")]
    public Sprite[] dishSprites;
    public string[] dishNames;
    public Vector4[] dishIconCrop;

    [Header("Ingredients")]
    public Sprite[] ingredientSprites;
    public string[] ingredientNames;
    public Vector4[] ingredientIconCrop;
}

/// <summary>
/// Drives the Chef's Collection menu.
///
/// IMPORTANT:
/// This script only controls Food Menu New.
/// It does NOT modify gameplay, cooking, customers, coins, or level scenes.
/// Each level has its own dish and ingredient data.
/// </summary>
public class FoodMenuController : MonoBehaviour
{
    // ================================================================
    // LEVEL-SPECIFIC DATA
    // ================================================================

    [Header("LEVEL MENU DATA")]
    [Tooltip("Index 0 = Tanzania, Index 1 = Nigeria, Index 2 = Ghana")]
    public FoodMenuLevelData[] levels = new FoodMenuLevelData[3];


    // ================================================================
    // ACTIVE RUNTIME DATA
    // These are filled automatically from the selected level.
    // ================================================================

    [HideInInspector] public Sprite[] dishSprites;
    [HideInInspector] public string[] dishNames;
    [HideInInspector] public Vector4[] dishIconCrop;

    [HideInInspector] public Sprite[] ingredientSprites;
    [HideInInspector] public string[] ingredientNames;
    [HideInInspector] public Vector4[] ingredientIconCrop;


    // ================================================================
    // DISH SHELF
    // ================================================================

    [Header("Dish shelf - serialized scene objects")]
    public Image[] dishSlots;
    public TMP_Text[] dishLabels;
    public Button dishPrevButton;
    public Button dishNextButton;


    // ================================================================
    // INGREDIENT SHELF
    // ================================================================

    [Header("Ingredient shelf - serialized scene objects")]
    public Image[] ingredientSlots;
    public TMP_Text[] ingredientLabels;
    public Button ingredientPrevButton;
    public Button ingredientNextButton;


    // ================================================================
    // LEVEL SELECTION
    // ================================================================

    [Header("Level selection (index 0 = Level 1)")]
    public Button[] levelButtons;
    public Image[] levelBands;
    public Image[] levelThumbnails;

    /// <summary>
    /// Closed padlock shown while a level is locked.
    /// </summary>
    public GameObject[] lockIcons;

    /// <summary>
    /// Open padlock shown once a level is unlocked.
    /// </summary>
    public GameObject[] completedIcons;

    public GameObject[] selectionHighlights;


    // ================================================================
    // PLAY
    // ================================================================

    [Header("Play")]
    public Button playButton;


    // ================================================================
    // CHEF
    // ================================================================

    [Header("Chef (optional - shows the chef who cooks the selected level)")]
    public LevelChefBinder chefBinder;


    // ================================================================
    // TUNING
    // ================================================================

    [Header("Tuning")]
    public float pageFadeSeconds = 0.16f;


    static readonly Color[] LevelTint =
    {
        new Color(0.18f, 0.62f, 0.31f, 1f),
        new Color(0.16f, 0.39f, 0.78f, 1f),
        new Color(0.48f, 0.23f, 0.66f, 1f)
    };


    int dishPage;
    int ingPage;
    int selectedLevel = 1;
    bool busy;


    // Original authored icon positions/sizes.
    Vector2[] dishIconHome;
    Vector2[] dishIconBox;
    Vector2[] ingIconHome;
    Vector2[] ingIconBox;


    // ================================================================
    // AWAKE
    // ================================================================

    void Awake()
    {
        CaptureIconRects(
            dishSlots,
            out dishIconHome,
            out dishIconBox
        );

        CaptureIconRects(
            ingredientSlots,
            out ingIconHome,
            out ingIconBox
        );


        // Dish buttons
        Hook(
            dishPrevButton,
            PrevDishPage,
            "dishPrevButton"
        );

        Hook(
            dishNextButton,
            NextDishPage,
            "dishNextButton"
        );


        // Ingredient buttons
        Hook(
            ingredientPrevButton,
            PrevIngredientPage,
            "ingredientPrevButton"
        );

        Hook(
            ingredientNextButton,
            NextIngredientPage,
            "ingredientNextButton"
        );


        // Play
        Hook(
            playButton,
            PlaySelected,
            "playButton"
        );


        // Level buttons
        for (int i = 0; i < Len(levelButtons); i++)
        {
            int level = i + 1;

            if (levelButtons[i] == null)
            {
                Debug.LogWarning(
                    "FoodMenu: levelButtons[" + i + "] is not assigned."
                );

                continue;
            }

            levelButtons[i].onClick.AddListener(
                delegate
                {
                    SelectLevel(level);
                }
            );
        }
    }


    // ================================================================
    // START
    // ================================================================

    void Start()
    {
        selectedLevel = LevelProgress.SelectedLevel;

        if (selectedLevel < 1 ||
            selectedLevel > LevelProgress.LevelCount)
        {
            selectedLevel = 1;
        }


        ApplyLevelData(selectedLevel);

        RefreshDishes();
        RefreshIngredients();
        RefreshLevels();
    }


    // ================================================================
    // LEVEL DATA
    // ================================================================

    void ApplyLevelData(int level)
    {
        int index = level - 1;

        if (levels == null ||
            index < 0 ||
            index >= levels.Length ||
            levels[index] == null)
        {
            Debug.LogWarning(
                "FoodMenu: No menu data assigned for Level " + level
            );

            dishSprites = new Sprite[0];
            dishNames = new string[0];
            dishIconCrop = new Vector4[0];

            ingredientSprites = new Sprite[0];
            ingredientNames = new string[0];
            ingredientIconCrop = new Vector4[0];

            return;
        }


        FoodMenuLevelData data = levels[index];


        // Copy only the selected level's data.
        dishSprites = data.dishSprites ?? new Sprite[0];
        dishNames = data.dishNames ?? new string[0];
        dishIconCrop = data.dishIconCrop ?? new Vector4[0];

        ingredientSprites =
            data.ingredientSprites ?? new Sprite[0];

        ingredientNames =
            data.ingredientNames ?? new string[0];

        ingredientIconCrop =
            data.ingredientIconCrop ?? new Vector4[0];


        Debug.Log(
            "FoodMenu: Loaded Level " +
            level +
            " menu data. Dishes=" +
            dishSprites.Length +
            ", Ingredients=" +
            ingredientSprites.Length
        );
    }


    // ================================================================
    // ARRAY HELPERS
    // ================================================================

    static int Len(System.Array a)
    {
        return a == null ? 0 : a.Length;
    }


    static int Pages(int items, int per)
    {
        if (items <= 0 || per <= 0)
            return 0;

        return (items + per - 1) / per;
    }


    int DishPages
    {
        get
        {
            return Pages(
                Len(dishSprites),
                Len(dishSlots)
            );
        }
    }


    int IngPages
    {
        get
        {
            return Pages(
                Len(ingredientSprites),
                Len(ingredientSlots)
            );
        }
    }


    // ================================================================
    // ICON RECT CAPTURE
    // ================================================================

    static void CaptureIconRects(
        Image[] slots,
        out Vector2[] home,
        out Vector2[] box
    )
    {
        int n = Len(slots);

        home = new Vector2[n];
        box = new Vector2[n];


        for (int i = 0; i < n; i++)
        {
            if (slots[i] == null)
                continue;

            RectTransform rt =
                slots[i].rectTransform;

            home[i] =
                rt.anchoredPosition;

            box[i] =
                rt.sizeDelta;
        }
    }


    // ================================================================
    // BUTTON HOOK
    // ================================================================

    static void Hook(
        Button b,
        UnityEngine.Events.UnityAction a,
        string name
    )
    {
        if (b == null)
        {
            Debug.LogWarning(
                "FoodMenu: " +
                name +
                " is not assigned."
            );

            return;
        }


        b.onClick.RemoveListener(a);
        b.onClick.AddListener(a);
    }


    // ================================================================
    // DISH PAGINATION
    // ================================================================

    public void NextDishPage()
    {
        Step(true, 1);
    }


    public void PrevDishPage()
    {
        Step(true, -1);
    }


    // ================================================================
    // INGREDIENT PAGINATION
    // ================================================================

    public void NextIngredientPage()
    {
        Step(false, 1);
    }


    public void PrevIngredientPage()
    {
        Step(false, -1);
    }


    void Step(bool dishes, int dir)
    {
        int count =
            dishes
            ? DishPages
            : IngPages;


        if (busy || count <= 1)
            return;


        if (dishes)
        {
            dishPage =
                Mathf.Clamp(
                    dishPage + dir,
                    0,
                    count - 1
                );
        }
        else
        {
            ingPage =
                Mathf.Clamp(
                    ingPage + dir,
                    0,
                    count - 1
                );
        }


        StartCoroutine(
            FadeSwap(dishes)
        );
    }


    // ================================================================
    // PAGE FADE
    // ================================================================

    IEnumerator FadeSwap(bool dishes)
    {
        busy = true;


        Image[] slots =
            dishes
            ? dishSlots
            : ingredientSlots;


        TMP_Text[] labels =
            dishes
            ? dishLabels
            : ingredientLabels;


        yield return Fade(
            slots,
            labels,
            1f,
            0f
        );


        if (dishes)
            RefreshDishes();
        else
            RefreshIngredients();


        yield return Fade(
            slots,
            labels,
            0f,
            1f
        );


        busy = false;
    }


    IEnumerator Fade(
        Image[] slots,
        TMP_Text[] labels,
        float from,
        float to
    )
    {
        float t = 0f;


        while (t < pageFadeSeconds)
        {
            t += Time.unscaledDeltaTime;


            float normalized =
                pageFadeSeconds <= 0f
                ? 1f
                : t / pageFadeSeconds;


            SetAlpha(
                slots,
                labels,
                Mathf.Lerp(
                    from,
                    to,
                    normalized
                )
            );


            yield return null;
        }


        SetAlpha(
            slots,
            labels,
            to
        );
    }


    static void SetAlpha(
        Image[] slots,
        TMP_Text[] labels,
        float a
    )
    {
        for (int i = 0; i < Len(slots); i++)
        {
            if (slots[i] == null)
                continue;


            Color c =
                slots[i].color;

            c.a = a;

            slots[i].color = c;
        }


        for (int i = 0; i < Len(labels); i++)
        {
            if (labels[i] == null)
                continue;


            Color c =
                labels[i].color;

            c.a = a;

            labels[i].color = c;
        }
    }


    // ================================================================
    // REFRESH DISHES
    // ================================================================

    public void RefreshDishes()
    {
        Fill(
            dishSlots,
            dishLabels,
            dishSprites,
            dishNames,
            dishIconCrop,
            dishIconHome,
            dishIconBox,
            dishPage
        );


        if (dishPrevButton != null)
        {
            dishPrevButton.interactable =
                dishPage > 0;
        }


        if (dishNextButton != null)
        {
            dishNextButton.interactable =
                dishPage < DishPages - 1;
        }
    }


    // ================================================================
    // REFRESH INGREDIENTS
    // ================================================================

    public void RefreshIngredients()
    {
        Fill(
            ingredientSlots,
            ingredientLabels,
            ingredientSprites,
            ingredientNames,
            ingredientIconCrop,
            ingIconHome,
            ingIconBox,
            ingPage
        );


        if (ingredientPrevButton != null)
        {
            ingredientPrevButton.interactable =
                ingPage > 0;
        }


        if (ingredientNextButton != null)
        {
            ingredientNextButton.interactable =
                ingPage < IngPages - 1;
        }
    }


    // ================================================================
    // FILL SHELF
    // ================================================================

    static void Fill(
        Image[] slots,
        TMP_Text[] labels,
        Sprite[] sprites,
        string[] names,
        Vector4[] crops,
        Vector2[] home,
        Vector2[] box,
        int page
    )
    {
        int per =
            Len(slots);


        for (int i = 0; i < per; i++)
        {
            int index =
                page * per + i;


            bool has =
                index < Len(sprites) &&
                sprites[index] != null;


            // --------------------------------------------------------
            // CARD
            // Hide the whole card (background + icon + label) when the
            // slot is empty, so no blank box is left on the shelf.
            // --------------------------------------------------------

            GameObject card =
                CardRoot(
                    slots[i],
                    i < Len(labels) ? labels[i] : null
                );

            if (card != null &&
                card.activeSelf != has)
            {
                card.SetActive(has);
            }


            // --------------------------------------------------------
            // IMAGE
            // --------------------------------------------------------

            if (slots[i] != null)
            {
                slots[i].sprite =
                    has
                    ? sprites[index]
                    : null;


                slots[i].enabled =
                    has;


                if (has &&
                    i < Len(home) &&
                    i < Len(box))
                {
                    Vector4 crop =
                        index < Len(crops)
                        ? crops[index]
                        : Vector4.zero;


                    FitIcon(
                        slots[i],
                        box[i],
                        home[i],
                        crop
                    );
                }
            }


            // --------------------------------------------------------
            // LABEL
            // --------------------------------------------------------

            if (i < Len(labels) &&
                labels[i] != null)
            {
                labels[i].text =
                    (
                        has &&
                        index < Len(names)
                    )
                    ? names[index]
                    : "";
            }
        }
    }


    /// <summary>
    /// The card container for a slot: the Icon's parent (ItemN), which
    /// holds the card background, the Icon and the Label. Only used when
    /// the label shares that parent, so a whole section is never hidden.
    /// </summary>
    static GameObject CardRoot(
        Image slot,
        TMP_Text label
    )
    {
        if (slot == null)
            return null;


        Transform parent =
            slot.transform.parent;


        if (parent != null &&
            label != null &&
            label.transform.parent == parent)
        {
            return parent.gameObject;
        }


        return slot.gameObject;
    }


    // ================================================================
    // ICON FITTING
    // ================================================================

    static void FitIcon(
        Image img,
        Vector2 box,
        Vector2 home,
        Vector4 crop
    )
    {
        RectTransform rt =
            img.rectTransform;


        Sprite sp =
            img.sprite;


        float vw =
            sp == null
            ? 0f
            : sp.rect.width * crop.z;


        float vh =
            sp == null
            ? 0f
            : sp.rect.height * crop.w;


        // No crop data
        if (vw <= 0f || vh <= 0f)
        {
            img.preserveAspect = true;

            rt.sizeDelta = box;

            rt.anchoredPosition = home;

            return;
        }


        float s =
            Mathf.Min(
                box.x / vw,
                box.y / vh
            );


        img.preserveAspect = false;


        rt.sizeDelta =
            new Vector2(
                sp.rect.width * s,
                sp.rect.height * s
            );


        rt.anchoredPosition =
            home -
            new Vector2(
                crop.x *
                sp.rect.width *
                s,

                crop.y *
                sp.rect.height *
                s
            );
    }


    // ================================================================
    // LEVEL SELECTION
    // ================================================================

    public void SelectLevel(int level)
    {
        if (!LevelProgress.IsUnlocked(level))
        {
            Debug.Log(
                "FoodMenu: Level " +
                level +
                " is locked."
            );

            return;
        }


        selectedLevel =
            level;


        LevelProgress.SelectedLevel =
            level;


        // IMPORTANT:
        // Every level starts its menu from page 1.
        dishPage = 0;
        ingPage = 0;


        // Load ONLY this level's dishes and ingredients.
        ApplyLevelData(level);


        RefreshDishes();
        RefreshIngredients();
        RefreshLevels();


        if (
            level - 1 <
            Len(levelButtons)
            &&
            levelButtons[level - 1] != null
        )
        {
            StartCoroutine(
                Pulse(
                    levelButtons[level - 1].transform
                )
            );
        }
    }


    // ================================================================
    // REFRESH LEVEL BUTTONS
    // ================================================================

    public void RefreshLevels()
    {
        if (!LevelProgress.IsUnlocked(selectedLevel))
        {
            selectedLevel =
                LevelProgress.HighestUnlocked();

            ApplyLevelData(
                selectedLevel
            );

            dishPage = 0;
            ingPage = 0;

            RefreshDishes();
            RefreshIngredients();
        }


        for (
            int i = 0;
            i < LevelProgress.LevelCount;
            i++
        )
        {
            int level =
                i + 1;


            bool unlocked =
                LevelProgress.IsUnlocked(level);


            // --------------------------------------------------------
            // BUTTON
            // --------------------------------------------------------

            if (
                i < Len(levelButtons)
                &&
                levelButtons[i] != null
            )
            {
                levelButtons[i].interactable =
                    unlocked;
            }


            // --------------------------------------------------------
            // LOCK
            // --------------------------------------------------------

            if (
                i < Len(lockIcons)
                &&
                lockIcons[i] != null
            )
            {
                lockIcons[i].SetActive(
                    !unlocked
                );
            }


            // --------------------------------------------------------
            // UNLOCKED ICON
            // --------------------------------------------------------

            if (
                i < Len(completedIcons)
                &&
                completedIcons[i] != null
            )
            {
                completedIcons[i].SetActive(
                    unlocked
                );
            }


            // --------------------------------------------------------
            // SELECTION HIGHLIGHT
            // --------------------------------------------------------

            if (
                i < Len(selectionHighlights)
                &&
                selectionHighlights[i] != null
            )
            {
                selectionHighlights[i].SetActive(
                    unlocked &&
                    level == selectedLevel
                );
            }


            // --------------------------------------------------------
            // LEVEL BAND
            // --------------------------------------------------------

            if (
                i < Len(levelBands)
                &&
                levelBands[i] != null
            )
            {
                Color c =
                    LevelTint[
                        Mathf.Clamp(
                            i,
                            0,
                            LevelTint.Length - 1
                        )
                    ];


                if (!unlocked)
                {
                    c =
                        new Color(
                            c.r * 0.5f,
                            c.g * 0.5f,
                            c.b * 0.5f,
                            1f
                        );
                }


                levelBands[i].color =
                    c;
            }


            // --------------------------------------------------------
            // THUMBNAIL
            // --------------------------------------------------------

            if (
                i < Len(levelThumbnails)
                &&
                levelThumbnails[i] != null
            )
            {
                levelThumbnails[i].color =
                    unlocked
                    ? Color.white
                    : new Color(
                        0.45f,
                        0.45f,
                        0.48f,
                        1f
                    );
            }
        }


        // ------------------------------------------------------------
        // PLAY BUTTON
        // ------------------------------------------------------------

        if (playButton != null)
        {
            playButton.interactable =
                LevelProgress.IsUnlocked(
                    selectedLevel
                );
        }


        // ------------------------------------------------------------
        // CHEF
        // ------------------------------------------------------------

        if (chefBinder != null)
        {
            chefBinder.Apply(
                selectedLevel
            );
        }
    }


    // ================================================================
    // PULSE
    // ================================================================

    IEnumerator Pulse(Transform t)
    {
        float d = 0.14f;
        float e = 0f;


        while (e < d)
        {
            e +=
                Time.unscaledDeltaTime;


            t.localScale =
                Vector3.one *
                (
                    1f +
                    0.06f *
                    Mathf.Sin(
                        Mathf.PI *
                        e /
                        d
                    )
                );


            yield return null;
        }


        t.localScale =
            Vector3.one;
    }


    // ================================================================
    // PLAY SELECTED LEVEL
    // ================================================================

    public void PlaySelected()
    {
        int level =
            LevelProgress.IsUnlocked(
                selectedLevel
            )
            ? selectedLevel
            : 1;


        string scene =
            LevelProgress.StorySceneFor(
                level
            );


        Debug.Log(
            "FoodMenu: PLAY level " +
            level +
            " -> " +
            scene
        );


        SceneManager.LoadScene(
            scene
        );
    }
}