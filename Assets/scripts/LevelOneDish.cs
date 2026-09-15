/// <summary>
/// The identity of a Level One dish, used ONLY to decide which plate the customer
/// carries away.
///
/// This exists because <c>CustomerMenuManager.selectedFood</c> cannot answer the
/// question "what did we just serve?":
///
///   * SelectUgali() and SelectAnchoves() both set selectedFood = 0, so the two
///     are indistinguishable downstream.
///   * The delayed Invoke(HideMenu, 5f) can rewrite selectedFood to 4 after the
///     player has already chosen something else.
///
/// Nothing in the cooking, ingredient, coin or progression pipeline reads this
/// enum. It is purely additive: selectedFood and isComboOrder keep their exact
/// original meaning and every existing code path still uses them.
/// </summary>
public enum LevelOneDish
{
    None = 0,
    Ugali = 1,
    Anchoves = 2,
    PotatoLeaves = 3,
    /// <summary>The combo order (selectedFood 4) served from plate 2.</summary>
    UgaliTembele = 4,
}
