using UnityEngine;

/// <summary>
/// The plate a customer carries away after being served.
///
/// Replaces the old approach, where the carried food was baked into a separate
/// full-body animation per dish. That approach is what made Level One customers
/// leave holding Nigerian jollof rice: every controller hard-coded
/// carryingFood 1/2/3 to the Jollof/Egusi/PoundedYam states, and Level One's own
/// UgaliDagaa / UgaliTembele states had no transitions at all.
///
/// Expected hierarchy (CustomerCarry lives on the customer root so that Awake
/// always runs, even while the plate itself is hidden):
///
///     Customer            &lt;- FirstCustomer, SpriteRenderer, Animator, CustomerCarry
///      +-- CarryFood      &lt;- positioning node, sets the plate's size at the hands
///           +-- FoodSprite  &lt;- SpriteRenderer that shows the served dish
///
/// The plate is a child, so it moves with the customer automatically and never
/// touches the customer's own Transform scale.
/// </summary>
public class CustomerCarry : MonoBehaviour
{
    [Header("Visual")]
    [Tooltip("SpriteRenderer on the FoodSprite child. Found automatically if left empty.")]
    public SpriteRenderer foodSprite;

    [Header("Level One plates")]
    public Sprite ugali;
    public Sprite anchoves;
    public Sprite potatoLeaves;
    [Tooltip("The Ugali + Tembele combo served from plate 2.")]
    public Sprite ugaliTembele;

    /// <summary>The dish currently displayed, or None while hidden.</summary>
    public LevelOneDish Carrying { get; private set; }

    void Awake()
    {
        if (foodSprite == null)
        {
            Transform t = transform.Find("CarryFood/FoodSprite");
            if (t != null)
                foodSprite = t.GetComponent<SpriteRenderer>();
        }

        Hide();
    }

    /// <summary>Shows the plate for <paramref name="dish"/>. Unknown dishes hide it.</summary>
    public void Show(LevelOneDish dish)
    {
        Sprite s = SpriteFor(dish);

        if (foodSprite == null || s == null)
        {
            if (foodSprite == null)
                Debug.LogWarning("[CustomerCarry] " + name + " has no FoodSprite assigned.");
            else
                Debug.LogWarning("[CustomerCarry] " + name + " has no sprite for " + dish + ".");

            Hide();
            return;
        }

        Carrying = dish;
        foodSprite.sprite = s;
        foodSprite.gameObject.SetActive(true);

        Debug.Log("[CustomerCarry] " + name + " is carrying " + dish + ".");
    }

    /// <summary>Hides the plate. Called when a customer spawns and when it exits.</summary>
    public void Hide()
    {
        Carrying = LevelOneDish.None;

        if (foodSprite != null)
        {
            foodSprite.gameObject.SetActive(false);
        }
    }

    Sprite SpriteFor(LevelOneDish dish)
    {
        switch (dish)
        {
            case LevelOneDish.Ugali: return ugali;
            case LevelOneDish.Anchoves: return anchoves;
            case LevelOneDish.PotatoLeaves: return potatoLeaves;
            case LevelOneDish.UgaliTembele: return ugaliTembele;
            default: return null;
        }
    }
}
