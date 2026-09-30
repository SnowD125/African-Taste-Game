using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Level Three (Ghana) kitchen.
///
/// Every dish is ONE sequential list of steps:
///   the ingredients of each part, in the order given by Level3Recipes
///   -> the cooking stations -> the plate.
/// Only the current step's ingredient/station is accepted; any other click is
/// ignored and changes nothing. The hand always points at the current step.
/// A part starts cooking (Level3Recipes cookingTime) the moment its last
/// ingredient is added, so cooking runs while the player adds the next part.
///
/// Ingredients are never hidden. The order comes from CustomerMenuManager3
/// (selectedFood), and serving goes through CustomerMenuManager3.ServePlate1(),
/// so the customer, coin and Level Complete flow is unchanged.
/// </summary>
public class Level3CookingManager : MonoBehaviour
{
    public enum Target
    {
        None,
        Rice, Beans, Salt, CornDough, CassavaDough, RawTilapia, Ginger,
        Cassava, Plantain, Meat, Tomato, Onion, Pepper,
        WaakyePot, BoilPot, Grill, Mortar, SoupPan,
        Plate1, Plate2
    }

    [Header("Order")]
    public CustomerMenuManager3 menuManager;

    [Header("Hand")]
    public GameObject handCursor;

    [Header("Ingredients (always visible)")]
    public GameObject rice;
    public GameObject beans;
    public GameObject salt;
    public GameObject cornDough;
    public GameObject cassavaDough;
    public GameObject rawTilapia;
    public GameObject ginger;
    public GameObject cassava;
    public GameObject plantain;
    public GameObject meat;
    public GameObject tomato;
    public GameObject onion;
    public GameObject pepper;

    [Header("Waakye pot")]
    public SpriteRenderer waakyePot;
    public Sprite[] waakyeCookingFrames;
    public Sprite waakyeReadySprite;

    [Header("Boiling pot (Banku / Fufu)")]
    public SpriteRenderer boilingPot;
    public GameObject bankuCooking;
    public GameObject bankuReady;
    public GameObject fufuBoiling;
    public GameObject fufuBoiled;

    [Header("Grill (Tilapia)")]
    public SpriteRenderer grill;
    public Sprite[] tilapiaGrillFrames;
    public SpriteRenderer tilapiaCooked;
    public GameObject tilapiaBurned;

    [Header("Mortar (Fufu)")]
    public SpriteRenderer mortar;
    public SpriteRenderer fufuPounding;
    public Sprite[] fufuPoundingFrames;
    public SpriteRenderer fufuPounded;
    public Sprite fufuPoundedSprite;

    [Header("Soup pan (Light Soup)")]
    public SpriteRenderer soupPan;
    public GameObject soupCooking;
    public SpriteRenderer soupReady;
    public GameObject soupBurned;

    [Header("Plates")]
    public GameObject plate1;
    public GameObject plate2;
    public GameObject bankuOnPlate;
    public GameObject tilapiaOnPlate;
    public GameObject fufuOnPlate;
    public GameObject soupOnPlate;
    public Sprite waakyePlateSprite;
    [Tooltip("Waakye width on plate 1, as a fraction of the plate width.")]
    public float waakyePlateWidth = 0.7f;

    [Header("Timing (same values as Level Two)")]
    public float burnTime = 10f;
    public float poundingTime = 5f;
    public float frameRate = 6f;

    enum Part { Waakye, Banku, Tilapia, Fufu, LightSoup }
    enum State { Idle, Cooking, Ready, Pounding, Pounded, Plated, Burned }

    class Step
    {
        public Target target;
        public Part part;
        public bool isStation;
        public bool done;
    }

    class Original
    {
        public Sprite sprite;
        public Vector3 scale;
        public Vector3 position;
        public Bounds bounds;
    }

    readonly List<Step> steps = new List<Step>();
    readonly Dictionary<Part, State> state = new Dictionary<Part, State>();
    readonly Dictionary<SpriteRenderer, Original> originals = new Dictionary<SpriteRenderer, Original>();
    readonly Dictionary<Target, Vector3> stationPoint = new Dictionary<Target, Vector3>();

    GameObject waakyeOnPlate;
    int lastSelectedFood = -1;
    bool served;

    // =========================================================
    // SETUP
    // =========================================================

    void Awake()
    {
        if (menuManager == null)
            menuManager = Object.FindFirstObjectByType<CustomerMenuManager3>();

        Remember(waakyePot);
        Remember(grill);
        Remember(mortar);
        Remember(fufuPounding);
        Remember(fufuPounded);

        // The pounding object carries Level Two's pounded-yam animation;
        // Level Three drives it with the fufu frames instead.
        if (fufuPounding != null)
        {
            Animator a = fufuPounding.GetComponent<Animator>();
            if (a != null) a.enabled = false;
        }

        stationPoint[Target.WaakyePot] = Centre(waakyePot);
        stationPoint[Target.BoilPot] = Centre(boilingPot);
        stationPoint[Target.Grill] = Centre(grill);
        stationPoint[Target.Mortar] = Centre(mortar);
        stationPoint[Target.SoupPan] = Centre(soupPan);

        // Waakye on plate 1: a copy of the banku-on-plate object with the
        // waakye art (Level Three has no waakye plate object of its own).
        if (bankuOnPlate != null && plate1 != null && waakyePlateSprite != null)
        {
            waakyeOnPlate = Instantiate(bankuOnPlate, bankuOnPlate.transform.parent);
            waakyeOnPlate.name = "Waakye on plate";

            SpriteRenderer sr = waakyeOnPlate.GetComponent<SpriteRenderer>();
            SpriteRenderer plateSr = plate1.GetComponent<SpriteRenderer>();

            if (sr != null && plateSr != null)
            {
                Bounds b = plateSr.bounds;
                b.size = new Vector3(b.size.x * waakyePlateWidth, b.size.y, b.size.z);
                Fit(sr, waakyePlateSprite, b);
            }

            waakyeOnPlate.SetActive(false);
        }
    }

    void Start()
    {
        ResetKitchen();
    }

    void Update()
    {
        if (menuManager == null)
            return;

        int selected = menuManager.selectedFood;

        if (selected == lastSelectedFood)
            return;

        lastSelectedFood = selected;

        // New customer / abandoned order -> clean kitchen.
        // A (new) order -> its recipe starts at step 1.
        ResetKitchen();

        if (selected >= 0)
            BeginDish(selected);
    }

    // =========================================================
    // RECIPE -> STEPS
    // =========================================================

    void BeginDish(int selectedFood)
    {
        Part[] parts;
        Target plate;

        switch (selectedFood)
        {
            case 0: parts = new[] { Part.Waakye }; plate = Target.Plate1; break;
            case 1: parts = new[] { Part.Banku, Part.Tilapia }; plate = Target.Plate1; break;
            case 2: parts = new[] { Part.Fufu, Part.LightSoup }; plate = Target.Plate2; break;
            default: return;
        }

        foreach (Part p in parts)
        {
            Level3Recipe recipe = Level3Recipes.GetRecipe(FoodType(p));

            foreach (string ingredient in recipe.requiredIngredients)
            {
                Target t = IngredientTarget(ingredient);

                // e.g. "Waakye Leaves": no such ingredient on the table.
                if (t == Target.None)
                    continue;

                steps.Add(new Step { target = t, part = p });
            }
        }

        foreach (Part p in parts)
        {
            switch (p)
            {
                case Part.Waakye: AddStation(Target.WaakyePot, p); break;
                case Part.Banku: AddStation(Target.BoilPot, p); break;
                case Part.Tilapia: AddStation(Target.Grill, p); break;
                case Part.Fufu: AddStation(Target.BoilPot, p); AddStation(Target.Mortar, p); break;
                case Part.LightSoup: AddStation(Target.SoupPan, p); break;
            }
        }

        steps.Add(new Step { target = plate, part = parts[0], isStation = true });

        UpdateHand();
    }

    void AddStation(Target t, Part p)
    {
        steps.Add(new Step { target = t, part = p, isStation = true });
    }

    static Level3FoodType FoodType(Part p)
    {
        switch (p)
        {
            case Part.Waakye: return Level3FoodType.Waakye;
            case Part.Banku: return Level3FoodType.Banku;
            case Part.Tilapia: return Level3FoodType.Tilapia;
            case Part.Fufu: return Level3FoodType.Fufu;
            default: return Level3FoodType.LightSoup;
        }
    }

    static Target IngredientTarget(string ingredient)
    {
        switch (ingredient)
        {
            case "Rice": return Target.Rice;
            case "Beans": return Target.Beans;
            case "Salt": return Target.Salt;
            case "Corn Dough": return Target.CornDough;
            case "Cassava Dough": return Target.CassavaDough;
            case "Raw Tilapia": return Target.RawTilapia;
            case "Ginger": return Target.Ginger;
            case "Cassava": return Target.Cassava;
            case "Plantain": return Target.Plantain;
            case "Meat": return Target.Meat;
            case "Tomato": return Target.Tomato;
            case "Onion": return Target.Onion;
            case "Pepper": return Target.Pepper;
            default: return Target.None;
        }
    }

    Step Current()
    {
        foreach (Step s in steps)
            if (!s.done)
                return s;

        return null;
    }

    // =========================================================
    // CLICK
    // =========================================================

    /// <summary>Only the current step is accepted. Anything else: nothing happens.</summary>
    public void Click(Target t)
    {
        if (served)
            return;

        Step s = Current();

        if (s == null || s.target != t)
            return;

        if (!s.isStation)
        {
            s.done = true;

            if (AllIngredientsAdded(s.part))
                StartCoroutine(Cook(s.part));

            UpdateHand();
            return;
        }

        switch (t)
        {
            case Target.WaakyePot:
                if (Get(Part.Waakye) != State.Ready) return;
                Restore(waakyePot);
                if (waakyeOnPlate != null) waakyeOnPlate.SetActive(true);
                Set(Part.Waakye, State.Plated);
                break;

            case Target.BoilPot:
                if (s.part == Part.Banku)
                {
                    if (Get(Part.Banku) != State.Ready) return;
                    SetActive(bankuReady, false);
                    boilingPot.enabled = true;
                    SetActive(bankuOnPlate, true);
                    Set(Part.Banku, State.Plated);
                }
                else
                {
                    if (Get(Part.Fufu) != State.Ready) return;
                    SetActive(fufuBoiled, false);
                    boilingPot.enabled = true;
                    StartCoroutine(Pound());
                }
                break;

            case Target.Mortar:
                if (Get(Part.Fufu) != State.Pounded) return;
                Restore(fufuPounded);
                SetActive(fufuPounded.gameObject, false);
                mortar.enabled = true;
                SetActive(fufuOnPlate, true);
                Set(Part.Fufu, State.Plated);
                break;

            case Target.Grill:
                if (Get(Part.Tilapia) == State.Burned) { ClearBurned(Part.Tilapia); UpdateHand(); return; }
                if (Get(Part.Tilapia) != State.Ready) return;
                ResetColour(tilapiaCooked);
                SetActive(tilapiaCooked.gameObject, false);
                SetActive(tilapiaOnPlate, true);
                Set(Part.Tilapia, State.Plated);
                break;

            case Target.SoupPan:
                if (Get(Part.LightSoup) == State.Burned) { ClearBurned(Part.LightSoup); UpdateHand(); return; }
                if (Get(Part.LightSoup) != State.Ready) return;
                ResetColour(soupReady);
                SetActive(soupReady.gameObject, false);
                SetActive(soupOnPlate, true);
                Set(Part.LightSoup, State.Plated);
                break;

            case Target.Plate1:
            case Target.Plate2:
                Serve(t);
                return;
        }

        s.done = true;
        UpdateHand();
    }

    bool AllIngredientsAdded(Part p)
    {
        foreach (Step s in steps)
            if (s.part == p && !s.isStation && !s.done)
                return false;

        return true;
    }

    // =========================================================
    // COOKING (runs in parallel with the next steps)
    // =========================================================

    IEnumerator Cook(Part p)
    {
        Set(p, State.Cooking);
        float time = Level3Recipes.GetRecipe(FoodType(p)).cookingTime;

        switch (p)
        {
            case Part.Waakye:
                yield return Animate(waakyePot, waakyeCookingFrames, Orig(waakyePot).bounds, time);
                Fit(waakyePot, waakyeReadySprite, Orig(waakyePot).bounds);
                break;

            case Part.Banku:
                boilingPot.enabled = false;
                SetActive(bankuCooking, true);
                yield return new WaitForSeconds(time);
                SetActive(bankuCooking, false);
                SetActive(bankuReady, true);
                break;

            case Part.Fufu:
                boilingPot.enabled = false;
                SetActive(fufuBoiling, true);
                yield return new WaitForSeconds(time);
                SetActive(fufuBoiling, false);
                SetActive(fufuBoiled, true);
                break;

            case Part.Tilapia:
                yield return Animate(grill, tilapiaGrillFrames, Orig(grill).bounds, time);
                Restore(grill);
                SetActive(tilapiaCooked.gameObject, true);
                break;

            case Part.LightSoup:
                SetActive(soupCooking, true);
                yield return new WaitForSeconds(time);
                SetActive(soupCooking, false);
                SetActive(soupReady.gameObject, true);
                break;
        }

        Set(p, State.Ready);

        if (p == Part.Tilapia)
            StartCoroutine(BurnWarning(p, tilapiaCooked, tilapiaBurned));
        else if (p == Part.LightSoup)
            StartCoroutine(BurnWarning(p, soupReady, soupBurned));
    }

    IEnumerator Pound()
    {
        Set(Part.Fufu, State.Pounding);
        mortar.enabled = false;

        SetActive(fufuPounding.gameObject, true);
        yield return Animate(fufuPounding, fufuPoundingFrames, Orig(mortar).bounds, poundingTime);
        Restore(fufuPounding);
        SetActive(fufuPounding.gameObject, false);

        Fit(fufuPounded, fufuPoundedSprite, Orig(mortar).bounds);
        SetActive(fufuPounded.gameObject, true);
        Set(Part.Fufu, State.Pounded);
    }

    /// <summary>Same warning as Level One/Two: white -> yellow -> red, then burned.</summary>
    IEnumerator BurnWarning(Part p, SpriteRenderer ready, GameObject burned)
    {
        float t = 0f;

        while (t < burnTime)
        {
            if (Get(p) != State.Ready)
            {
                ResetColour(ready);
                yield break;
            }

            t += Time.deltaTime;
            float k = t / burnTime;

            if (ready != null)
            {
                ready.color = k < 0.5f
                    ? Color.Lerp(Color.white, Color.yellow, k * 2f)
                    : Color.Lerp(Color.yellow, Color.red, (k - 0.5f) * 2f);
            }

            yield return null;
        }

        if (Get(p) != State.Ready)
            yield break;

        ResetColour(ready);
        SetActive(ready.gameObject, false);
        SetActive(burned, true);
        Set(p, State.Burned);
    }

    /// <summary>Burned food is thrown away; that part restarts from its first ingredient.</summary>
    void ClearBurned(Part p)
    {
        if (p == Part.Tilapia) SetActive(tilapiaBurned, false);
        if (p == Part.LightSoup) SetActive(soupBurned, false);

        foreach (Step s in steps)
            if (s.part == p && s.target != Target.Plate1 && s.target != Target.Plate2)
                s.done = false;

        Set(p, State.Idle);
    }

    IEnumerator Animate(SpriteRenderer sr, Sprite[] frames, Bounds area, float duration)
    {
        if (sr == null || frames == null || frames.Length == 0)
        {
            yield return new WaitForSeconds(duration);
            yield break;
        }

        float t = 0f;

        while (t < duration)
        {
            int i = (int)(t * frameRate) % frames.Length;
            Fit(sr, frames[i], area);
            t += Time.deltaTime;
            yield return null;
        }
    }

    // =========================================================
    // SERVE
    // =========================================================

    void Serve(Target plate)
    {
        served = true;

        foreach (Step s in steps)
            s.done = true;

        SetActive(waakyeOnPlate, false);
        SetActive(bankuOnPlate, false);
        SetActive(tilapiaOnPlate, false);
        SetActive(fufuOnPlate, false);
        SetActive(soupOnPlate, false);
        SetActive(handCursor, false);

        // Existing serving flow: served UI -> customer -> coins -> next.
        menuManager.ServePlate1();

        // ServePlate1 takes plate 1 away; Fufu & Light Soup is on plate 2.
        if (plate == Target.Plate2)
        {
            SetActive(plate1, true);
            SetActive(plate2, false);
        }
    }

    // =========================================================
    // RESET
    // =========================================================

    void ResetKitchen()
    {
        StopAllCoroutines();

        steps.Clear();
        state.Clear();
        served = false;

        Restore(waakyePot);
        Restore(grill);
        Restore(fufuPounding);
        Restore(fufuPounded);

        if (boilingPot != null) boilingPot.enabled = true;
        if (mortar != null) mortar.enabled = true;

        SetActive(bankuCooking, false);
        SetActive(bankuReady, false);
        SetActive(fufuBoiling, false);
        SetActive(fufuBoiled, false);

        ResetColour(tilapiaCooked);
        if (tilapiaCooked != null) SetActive(tilapiaCooked.gameObject, false);
        SetActive(tilapiaBurned, false);

        if (fufuPounding != null) SetActive(fufuPounding.gameObject, false);
        if (fufuPounded != null) SetActive(fufuPounded.gameObject, false);

        SetActive(soupCooking, false);
        ResetColour(soupReady);
        if (soupReady != null) SetActive(soupReady.gameObject, false);
        SetActive(soupBurned, false);

        SetActive(waakyeOnPlate, false);
        SetActive(bankuOnPlate, false);
        SetActive(tilapiaOnPlate, false);
        SetActive(fufuOnPlate, false);
        SetActive(soupOnPlate, false);

        SetActive(plate1, true);
        SetActive(plate2, true);

        foreach (GameObject g in new[] { rice, beans, salt, cornDough, cassavaDough, rawTilapia, ginger,
                                         cassava, plantain, meat, tomato, onion, pepper })
            SetActive(g, true);

        SetActive(handCursor, false);
    }

    // =========================================================
    // HAND
    // =========================================================

    void UpdateHand()
    {
        if (handCursor == null)
            return;

        Step s = Current();

        if (s == null || served)
        {
            handCursor.SetActive(false);
            return;
        }

        Vector3 p = Point(s.target);
        handCursor.transform.position = new Vector3(p.x, p.y, handCursor.transform.position.z);
        handCursor.SetActive(true);
    }

    Vector3 Point(Target t)
    {
        if (stationPoint.TryGetValue(t, out Vector3 station))
            return station;

        GameObject g = ObjectFor(t);

        if (g == null)
            return handCursor.transform.position;

        SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
        return sr != null ? sr.bounds.center : g.transform.position;
    }

    GameObject ObjectFor(Target t)
    {
        switch (t)
        {
            case Target.Rice: return rice;
            case Target.Beans: return beans;
            case Target.Salt: return salt;
            case Target.CornDough: return cornDough;
            case Target.CassavaDough: return cassavaDough;
            case Target.RawTilapia: return rawTilapia;
            case Target.Ginger: return ginger;
            case Target.Cassava: return cassava;
            case Target.Plantain: return plantain;
            case Target.Meat: return meat;
            case Target.Tomato: return tomato;
            case Target.Onion: return onion;
            case Target.Pepper: return pepper;
            case Target.Plate1: return plate1;
            case Target.Plate2: return plate2;
            default: return null;
        }
    }

    // =========================================================
    // HELPERS
    // =========================================================

    State Get(Part p)
    {
        return state.TryGetValue(p, out State s) ? s : State.Idle;
    }

    void Set(Part p, State s)
    {
        state[p] = s;
    }

    static void SetActive(GameObject g, bool on)
    {
        if (g != null)
            g.SetActive(on);
    }

    static void ResetColour(SpriteRenderer sr)
    {
        if (sr != null)
            sr.color = Color.white;
    }

    static Vector3 Centre(SpriteRenderer sr)
    {
        return sr != null ? sr.bounds.center : Vector3.zero;
    }

    void Remember(SpriteRenderer sr)
    {
        if (sr == null || originals.ContainsKey(sr))
            return;

        originals[sr] = new Original
        {
            sprite = sr.sprite,
            scale = sr.transform.localScale,
            position = sr.transform.position,
            bounds = SpriteBounds(sr)
        };
    }

    Original Orig(SpriteRenderer sr)
    {
        return originals[sr];
    }

    void Restore(SpriteRenderer sr)
    {
        if (sr == null || !originals.TryGetValue(sr, out Original o))
            return;

        sr.sprite = o.sprite;
        sr.transform.localScale = o.scale;
        sr.transform.position = o.position;
    }

    /// <summary>World bounds of the sprite, valid even while the object is inactive.</summary>
    static Bounds SpriteBounds(SpriteRenderer sr)
    {
        if (sr.sprite == null)
            return new Bounds(sr.transform.position, Vector3.zero);

        Bounds local = sr.sprite.bounds;
        Vector3 s = sr.transform.lossyScale;
        Vector3 c = sr.transform.position + Vector3.Scale(local.center, s);
        return new Bounds(new Vector3(c.x, c.y, sr.transform.position.z),
                          new Vector3(Mathf.Abs(local.size.x * s.x), Mathf.Abs(local.size.y * s.y), 0f));
    }

    /// <summary>Shows sprite s on sr, as wide as area and centred on it.</summary>
    static void Fit(SpriteRenderer sr, Sprite s, Bounds area)
    {
        if (sr == null || s == null)
            return;

        sr.sprite = s;

        float k = area.size.x / s.bounds.size.x;
        Vector3 parent = sr.transform.parent != null ? sr.transform.parent.lossyScale : Vector3.one;

        sr.transform.localScale = new Vector3(k / parent.x, k / parent.y, 1f);
        sr.transform.position = new Vector3(
            area.center.x - s.bounds.center.x * k,
            area.center.y - s.bounds.center.y * k,
            sr.transform.position.z);
    }
}
