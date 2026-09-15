using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drink selection for the in-level customer menu.
///
/// Deliberately SEPARATE from CustomerMenuManager and the food/order pipeline:
/// it never reads or writes selectedFood, never touches a cooking manager, and
/// never calls into the customer/serving logic. Picking a drink only records
/// the choice and moves a tick. Nothing in the existing food flow can change
/// behaviour because this component exists.
///
/// One entry per drink row. Only one drink can be ticked at a time.
/// </summary>
public class DrinkMenu : MonoBehaviour
{
    [System.Serializable]
    public class Entry
    {
        [Tooltip("For your reference in the Inspector, e.g. \"Avocado Juice\".")]
        public string displayName;
        public Button button;
        public GameObject tick;
    }

    [Tooltip("Drink rows shown in this level's menu, top to bottom.")]
    public Entry[] drinks;

    /// <summary>Index into <see cref="drinks"/>, or -1 when nothing is chosen.</summary>
    [HideInInspector] public int selectedDrink = -1;

    void Awake()
    {
        for (int i = 0; i < Len(); i++)
        {
            int index = i;
            if (drinks[i] == null || drinks[i].button == null)
            {
                Debug.LogWarning("DrinkMenu: drinks[" + i + "] has no button assigned.");
                continue;
            }
            drinks[i].button.onClick.AddListener(delegate { Select(index); });
        }
        ClearAll();
    }

    int Len() { return drinks == null ? 0 : drinks.Length; }

    /// <summary>Ticks one drink and unticks the rest.</summary>
    public void Select(int index)
    {
        selectedDrink = index;
        Refresh();
    }

    /// <summary>Clears the choice. Called when a customer leaves or the menu closes.</summary>
    public void ClearAll()
    {
        selectedDrink = -1;
        Refresh();
    }

    void Refresh()
    {
        for (int i = 0; i < Len(); i++)
            if (drinks[i] != null && drinks[i].tick != null)
                drinks[i].tick.SetActive(i == selectedDrink);
    }
}
