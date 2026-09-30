using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Purely visual: draws a name under (or over) each ingredient that sits on the kitchen table.
///
/// The labels are world-space TextMeshPro objects created at runtime as children of THIS object,
/// never of the ingredients, so no ingredient is modified and no parent scale distorts the text.
/// They carry no collider and no raycast target, so they cannot intercept a click meant for an
/// ingredient. Each frame a label copies its ingredient's position and visibility, so it follows
/// anything that moves and disappears with an ingredient that is used up.
/// </summary>
public class IngredientLabels : MonoBehaviour
{
    public enum Placement { Below, Above }

    [System.Serializable]
    public class Entry
    {
        [Tooltip("The ingredient object already on the table. Never modified.")]
        public GameObject ingredient;

        [Tooltip("Name shown to the player.")]
        public string label;

        public Placement placement = Placement.Below;

        [Tooltip("Extra nudge in world units, when a neighbour is in the way.")]
        public Vector2 offset;
    }

    [Header("Ingredients on the table")]
    public Entry[] entries;

    [Header("Style (same in every level)")]
    [Tooltip("Game font for the names. Left empty, TextMeshPro's default is used.")]
    public TMP_FontAsset font;
    public float fontSize = 2.1f;
    public Color textColour = new Color(1f, 0.949f, 0.851f);      // warm off-white, as the menu rows
    public Color outlineColour = new Color(0.05f, 0.04f, 0.03f);
    [Range(0f, 1f)] public float outlineWidth = 0.28f;
    [Tooltip("Gap between the ingredient artwork and its name, in world units.")]
    public float gap = 0.55f;
    [Header("Label chip behind the text")]
    [Tooltip("Small rounded plate drawn behind each name. Leave empty for plain outlined text.")]
    public Sprite chipSprite;
    public Color chipColour = new Color(1f, 1f, 1f, 1f);
    [Tooltip("Space between the text and the chip edge, in world units (x = sides, y = top/bottom).")]
    public Vector2 chipPadding = new Vector2(0.55f, 0.18f);

    [Tooltip("The layer the kitchen sprites use. NOT the background layer, or the table hides the names.")]
    public string sortingLayer = "Default";
    [Tooltip("Same order as the hand cursor; the small z below keeps the hand and the food in front.")]
    public int sortingOrder = 6;

    readonly List<TextMeshPro> labels = new List<TextMeshPro>();
    readonly List<Entry> live = new List<Entry>();

    void Start()
    {
        if (entries == null)
            return;

        foreach (Entry e in entries)
        {
            if (e == null || e.ingredient == null || string.IsNullOrEmpty(e.label))
                continue;

            GameObject go = new GameObject("Label_" + e.label);
            go.transform.SetParent(transform, false);

            TextMeshPro tmp = go.AddComponent<TextMeshPro>();

            if (font != null)
                tmp.font = font;

            tmp.text = e.label;
            tmp.fontSize = fontSize;
            tmp.color = textColour;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.textWrappingMode = TextWrappingModes.NoWrap;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.raycastTarget = false;

            // A dark outline keeps the name readable over any part of the kitchen.
            tmp.outlineWidth = outlineWidth;
            tmp.outlineColor = outlineColour;

            RectTransform rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(20f, 3f);

            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            int layer = SortingLayer.NameToID(sortingLayer);
            layer = SortingLayer.IsValid(layer) ? layer : 0;
            mr.sortingLayerID = layer;
            mr.sortingOrder = sortingOrder;

            // The rounded plate behind the name, sized to the text.
            if (chipSprite != null)
            {
                tmp.ForceMeshUpdate();
                Vector2 text = tmp.GetRenderedValues(false);

                GameObject chip = new GameObject("Chip");
                chip.transform.SetParent(go.transform, false);
                chip.transform.localPosition = new Vector3(0f, 0f, 0.01f);   // just behind the text

                SpriteRenderer csr = chip.AddComponent<SpriteRenderer>();
                csr.sprite = chipSprite;
                csr.color = chipColour;
                csr.drawMode = SpriteDrawMode.Sliced;
                csr.size = new Vector2(text.x + chipPadding.x * 2f, text.y + chipPadding.y * 2f);
                csr.sortingLayerID = layer;
                csr.sortingOrder = sortingOrder;
            }

            labels.Add(tmp);
            live.Add(e);
            Place(tmp, e);
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < labels.Count; i++)
        {
            Entry e = live[i];
            TextMeshPro tmp = labels[i];

            if (e.ingredient == null)
            {
                tmp.gameObject.SetActive(false);
                continue;
            }

            bool visible = e.ingredient.activeInHierarchy;

            if (tmp.gameObject.activeSelf != visible)
                tmp.gameObject.SetActive(visible);

            if (visible)
                Place(tmp, e);
        }
    }

    void Place(TextMeshPro tmp, Entry e)
    {
        SpriteRenderer sr = e.ingredient.GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            tmp.transform.position = e.ingredient.transform.position + (Vector3)e.offset;
            return;
        }

        Bounds b = sr.bounds;
        float y = e.placement == Placement.Below ? b.min.y - gap : b.max.y + gap;

        // z slightly away from the camera: at equal sorting order the hand cursor and the
        // ingredients stay in front, so a name can never cover them.
        tmp.transform.position = new Vector3(b.center.x + e.offset.x, y + e.offset.y, 0.05f);
    }
}
