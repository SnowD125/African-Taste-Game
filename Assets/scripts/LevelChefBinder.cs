using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drops onto any chef object and points it at the <see cref="ChefRoster"/>.
/// Works with either a UI Image or a SpriteRenderer, so the same component
/// serves the Food Menu, the story scenes and the level-complete screens.
///
/// Usage:
///   * Menu  - leave <see cref="level"/> at 0; FoodMenuController calls
///             Apply(selectedLevel) whenever the player picks a level.
///   * Scene - set <see cref="level"/> to that scene's level (2 for Nigeria,
///             etc.) and it applies itself on Awake. Nothing else to wire.
///
/// It never changes the object's transform, so an artist's hand-placed chef
/// keeps its exact position and scale - only the sprite swaps.
/// </summary>
[DisallowMultipleComponent]
public class LevelChefBinder : MonoBehaviour
{
    [Tooltip("Shared asset holding the level -> chef mapping.")]
    public ChefRoster roster;

    [Tooltip("Level this chef belongs to. 0 = follow the player's selected level " +
             "(used by the Food Menu); 1/2/3/... = fixed for this scene.")]
    public int level = 0;

    [Tooltip("Target graphic. Leave empty to use the Image or SpriteRenderer " +
             "on this same GameObject.")]
    public Image targetImage;
    public SpriteRenderer targetRenderer;

    void Reset()
    {
        targetImage = GetComponent<Image>();
        targetRenderer = GetComponent<SpriteRenderer>();
    }

    void Awake()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (targetRenderer == null) targetRenderer = GetComponent<SpriteRenderer>();
        if (level > 0) Apply(level);
    }

    /// <summary>Shows the chef who cooks the given level.</summary>
    public void Apply(int forLevel)
    {
        if (roster == null)
        {
            Debug.LogWarning("LevelChefBinder on " + name + ": no ChefRoster assigned.");
            return;
        }
        Sprite s = roster.PortraitFor(forLevel);
        if (s == null) return;                     // keep whatever is already shown
        if (targetImage != null) targetImage.sprite = s;
        if (targetRenderer != null) targetRenderer.sprite = s;
    }
}
