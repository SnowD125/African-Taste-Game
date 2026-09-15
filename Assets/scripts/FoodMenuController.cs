using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// Drives the Chef's Collection menu. It does NOT create any UI - every object
/// referenced below is a real serialized scene object, visible and editable in
/// the Editor. This script only updates page contents, selection, lock and
/// completed state, and button interactability.
/// </summary>
public class FoodMenuController : MonoBehaviour
{
    [Header("Dish data (sprites, names and crops run in parallel)")]
    public Sprite[] dishSprites;
    public string[] dishNames;
    /// <summary>Opaque bounds of each dish sprite, normalised and Y-up:
    /// (x, y) = centre of the visible pixels relative to the sprite centre,
    /// (z, w) = width/height of the visible pixels as a fraction of the sprite.
    /// Lets the menu centre and size each item by what you can actually SEE
    /// rather than by its transparent canvas. Leave empty to fall back to
    /// plain preserveAspect.</summary>
    public Vector4[] dishIconCrop;

    [Header("Dish shelf - serialized scene objects")]
    public Image[] dishSlots;
    public TMP_Text[] dishLabels;
    public Button dishPrevButton;
    public Button dishNextButton;

    [Header("Ingredient data")]
    public Sprite[] ingredientSprites;
    public string[] ingredientNames;
    public Vector4[] ingredientIconCrop;

    [Header("Ingredient shelf - serialized scene objects")]
    public Image[] ingredientSlots;
    public TMP_Text[] ingredientLabels;
    public Button ingredientPrevButton;
    public Button ingredientNextButton;

    [Header("Level selection (index 0 = Level 1)")]
    public Button[] levelButtons;
    public Image[] levelBands;
    public Image[] levelThumbnails;
    /// <summary>Closed padlock - shown while a level is still locked.</summary>
    public GameObject[] lockIcons;
    /// <summary>Open padlock - shown once a level is unlocked (and stays on when
    /// it is completed and replayable). Locked = closed lock, unlocked = open lock.</summary>
    public GameObject[] completedIcons;
    public GameObject[] selectionHighlights;

    [Header("Play")]
    public Button playButton;

    [Header("Chef (optional - shows the chef who cooks the selected level)")]
    public LevelChefBinder chefBinder;

    [Header("Tuning")]
    public float pageFadeSeconds = 0.16f;

    static readonly Color[] LevelTint = {
        new Color(0.18f, 0.62f, 0.31f, 1f),
        new Color(0.16f, 0.39f, 0.78f, 1f),
        new Color(0.48f, 0.23f, 0.66f, 1f)
    };

    int dishPage, ingPage, selectedLevel = 1;
    bool busy;

    // Icon slots are resized and nudged per sprite, so their authored rect is
    // captured once and every later fit is measured from that.
    Vector2[] dishIconHome, dishIconBox, ingIconHome, ingIconBox;

    void Awake()
    {
        CaptureIconRects(dishSlots, out dishIconHome, out dishIconBox);
        CaptureIconRects(ingredientSlots, out ingIconHome, out ingIconBox);

        // Listeners are registered in Awake so that a later failure in Start
        // cannot leave the buttons dead.
        Hook(dishPrevButton, PrevDishPage, "dishPrevButton");
        Hook(dishNextButton, NextDishPage, "dishNextButton");
        Hook(ingredientPrevButton, PrevIngredientPage, "ingredientPrevButton");
        Hook(ingredientNextButton, NextIngredientPage, "ingredientNextButton");
        Hook(playButton, PlaySelected, "playButton");

        for (int i = 0; i < Len(levelButtons); i++)
        {
            int level = i + 1;
            if (levelButtons[i] == null)
            {
                Debug.LogWarning("FoodMenu: levelButtons[" + i + "] is not assigned.");
                continue;
            }
            levelButtons[i].onClick.AddListener(delegate { SelectLevel(level); });
        }
    }

    static void CaptureIconRects(Image[] slots, out Vector2[] home, out Vector2[] box)
    {
        int n = Len(slots);
        home = new Vector2[n];
        box = new Vector2[n];
        for (int i = 0; i < n; i++)
        {
            if (slots[i] == null) continue;
            RectTransform rt = slots[i].rectTransform;
            home[i] = rt.anchoredPosition;
            box[i] = rt.sizeDelta;
        }
    }

    static void Hook(Button b, UnityEngine.Events.UnityAction a, string name)
    {
        if (b == null) { Debug.LogWarning("FoodMenu: " + name + " is not assigned."); return; }
        b.onClick.RemoveListener(a);   // guard against double registration
        b.onClick.AddListener(a);
    }

    void Start()
    {
        selectedLevel = LevelProgress.SelectedLevel;
        RefreshDishes();
        RefreshIngredients();
        RefreshLevels();
    }

    static int Len(System.Array a) { return a == null ? 0 : a.Length; }
    static int Pages(int items, int per) { return (items <= 0 || per <= 0) ? 0 : (items + per - 1) / per; }

    int DishPages { get { return Pages(Len(dishSprites), Len(dishSlots)); } }
    int IngPages { get { return Pages(Len(ingredientSprites), Len(ingredientSlots)); } }

    // ------------------------------------------------------------------ pages

    public void NextDishPage() { Step(true, 1); }
    public void PrevDishPage() { Step(true, -1); }
    public void NextIngredientPage() { Step(false, 1); }
    public void PrevIngredientPage() { Step(false, -1); }

    void Step(bool dishes, int dir)
    {
        int count = dishes ? DishPages : IngPages;
        if (busy || count <= 1) return;
        if (dishes) dishPage = Mathf.Clamp(dishPage + dir, 0, count - 1);
        else ingPage = Mathf.Clamp(ingPage + dir, 0, count - 1);
        StartCoroutine(FadeSwap(dishes));
    }

    IEnumerator FadeSwap(bool dishes)
    {
        busy = true;
        Image[] slots = dishes ? dishSlots : ingredientSlots;
        TMP_Text[] labels = dishes ? dishLabels : ingredientLabels;
        yield return Fade(slots, labels, 1f, 0f);
        if (dishes) RefreshDishes(); else RefreshIngredients();
        yield return Fade(slots, labels, 0f, 1f);
        busy = false;
    }

    IEnumerator Fade(Image[] slots, TMP_Text[] labels, float from, float to)
    {
        float t = 0f;
        while (t < pageFadeSeconds)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(slots, labels, Mathf.Lerp(from, to, pageFadeSeconds <= 0f ? 1f : t / pageFadeSeconds));
            yield return null;
        }
        SetAlpha(slots, labels, to);
    }

    static void SetAlpha(Image[] slots, TMP_Text[] labels, float a)
    {
        for (int i = 0; i < Len(slots); i++)
        {
            if (slots[i] == null) continue;
            Color c = slots[i].color; c.a = a; slots[i].color = c;
        }
        for (int i = 0; i < Len(labels); i++)
        {
            if (labels[i] == null) continue;
            Color c = labels[i].color; c.a = a; labels[i].color = c;
        }
    }

    public void RefreshDishes()
    {
        Fill(dishSlots, dishLabels, dishSprites, dishNames, dishIconCrop,
             dishIconHome, dishIconBox, dishPage);
        if (dishPrevButton != null) dishPrevButton.interactable = dishPage > 0;
        if (dishNextButton != null) dishNextButton.interactable = dishPage < DishPages - 1;
    }

    public void RefreshIngredients()
    {
        Fill(ingredientSlots, ingredientLabels, ingredientSprites, ingredientNames,
             ingredientIconCrop, ingIconHome, ingIconBox, ingPage);
        if (ingredientPrevButton != null) ingredientPrevButton.interactable = ingPage > 0;
        if (ingredientNextButton != null) ingredientNextButton.interactable = ingPage < IngPages - 1;
    }

    static void Fill(Image[] slots, TMP_Text[] labels, Sprite[] sprites, string[] names,
                     Vector4[] crops, Vector2[] home, Vector2[] box, int page)
    {
        int per = Len(slots);
        for (int i = 0; i < per; i++)
        {
            int index = page * per + i;
            bool has = index < Len(sprites) && sprites[index] != null;
            if (slots[i] != null)
            {
                slots[i].sprite = has ? sprites[index] : null;
                slots[i].enabled = has;
                if (has && i < Len(home))
                {
                    Vector4 crop = index < Len(crops) ? crops[index] : Vector4.zero;
                    FitIcon(slots[i], box[i], home[i], crop);
                }
            }
            if (i < Len(labels) && labels[i] != null)
                labels[i].text = (has && index < Len(names)) ? names[index] : "";
        }
    }

    /// <summary>
    /// Sizes and positions an icon so its VISIBLE pixels are centred in, and fill,
    /// the authored icon box. Several source PNGs have their food off-centre on a
    /// large transparent canvas; plain preserveAspect would centre the canvas and
    /// leave the food looking shifted and undersized. Falls back to preserveAspect
    /// when no crop data is supplied.
    /// </summary>
    static void FitIcon(Image img, Vector2 box, Vector2 home, Vector4 crop)
    {
        RectTransform rt = img.rectTransform;
        Sprite sp = img.sprite;
        float vw = sp == null ? 0f : sp.rect.width * crop.z;
        float vh = sp == null ? 0f : sp.rect.height * crop.w;

        if (vw <= 0f || vh <= 0f)          // no crop data - behave as before
        {
            img.preserveAspect = true;
            rt.sizeDelta = box;
            rt.anchoredPosition = home;
            return;
        }

        float s = Mathf.Min(box.x / vw, box.y / vh);
        img.preserveAspect = false;
        rt.sizeDelta = new Vector2(sp.rect.width * s, sp.rect.height * s);
        rt.anchoredPosition = home - new Vector2(crop.x * sp.rect.width * s,
                                                 crop.y * sp.rect.height * s);
    }

    // ----------------------------------------------------------------- levels

    public void SelectLevel(int level)
    {
        if (!LevelProgress.IsUnlocked(level))
        {
            Debug.Log("FoodMenu: level " + level + " is locked.");
            return;
        }
        selectedLevel = level;
        LevelProgress.SelectedLevel = level;
        RefreshLevels();
        if (level - 1 < Len(levelButtons) && levelButtons[level - 1] != null)
            StartCoroutine(Pulse(levelButtons[level - 1].transform));
    }

    public void RefreshLevels()
    {
        if (!LevelProgress.IsUnlocked(selectedLevel)) selectedLevel = LevelProgress.HighestUnlocked();

        for (int i = 0; i < LevelProgress.LevelCount; i++)
        {
            int level = i + 1;
            bool unlocked = LevelProgress.IsUnlocked(level);

            if (i < Len(levelButtons) && levelButtons[i] != null)
                levelButtons[i].interactable = unlocked;

            // Padlock state, not a tick: locked levels show a closed padlock,
            // unlocked (including completed, replayable) levels show an open one.
            if (i < Len(lockIcons) && lockIcons[i] != null)
                lockIcons[i].SetActive(!unlocked);

            if (i < Len(completedIcons) && completedIcons[i] != null)
                completedIcons[i].SetActive(unlocked);

            if (i < Len(selectionHighlights) && selectionHighlights[i] != null)
                selectionHighlights[i].SetActive(unlocked && level == selectedLevel);

            if (i < Len(levelBands) && levelBands[i] != null)
            {
                Color c = LevelTint[i];
                if (!unlocked) c = new Color(c.r * 0.5f, c.g * 0.5f, c.b * 0.5f, 1f);
                levelBands[i].color = c;
            }

            if (i < Len(levelThumbnails) && levelThumbnails[i] != null)
                levelThumbnails[i].color = unlocked ? Color.white : new Color(0.45f, 0.45f, 0.48f, 1f);
        }

        if (playButton != null) playButton.interactable = LevelProgress.IsUnlocked(selectedLevel);
        if (chefBinder != null) chefBinder.Apply(selectedLevel);
    }

    IEnumerator Pulse(Transform t)
    {
        float d = 0.14f, e = 0f;
        while (e < d)
        {
            e += Time.unscaledDeltaTime;
            t.localScale = Vector3.one * (1f + 0.06f * Mathf.Sin(Mathf.PI * e / d));
            yield return null;
        }
        t.localScale = Vector3.one;
    }

    /// <summary>Loads the story scene for the selected level. Never skips the story.</summary>
    public void PlaySelected()
    {
        int level = LevelProgress.IsUnlocked(selectedLevel) ? selectedLevel : 1;
        string scene = LevelProgress.StorySceneFor(level);
        Debug.Log("FoodMenu: PLAY level " + level + " -> " + scene);
        SceneManager.LoadScene(scene);
    }
}
