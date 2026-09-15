using UnityEngine;

/// <summary>
/// The single source of truth for "which chef cooks which level".
///
/// African Taste alternates chefs by level:
///     Level 1 -> male, Level 2 -> female, Level 3 -> male, Level 4 -> female, ...
///
/// The mapping lives in DATA (the ChefRoster asset), not in code, so adding
/// Level 4 later means adding one entry in the Inspector - no script changes
/// and no per-scene duplication. Scenes reference the roster through
/// <see cref="LevelChefBinder"/>.
///
/// Create/edit the asset via  Assets > Create > African Taste > Chef Roster.
/// </summary>
[CreateAssetMenu(fileName = "ChefRoster", menuName = "African Taste/Chef Roster")]
public class ChefRoster : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        [Tooltip("Level this chef cooks. 1 = Tanzania, 2 = Nigeria, 3 = Ghana, ...")]
        public int level = 1;

        [Tooltip("For your reference only - e.g. \"Male chef\" / \"Female chef\".")]
        public string displayName = "";

        [Tooltip("Full-body cutout used in the menu, story and level-complete screens.")]
        public Sprite portrait;
    }

    [Tooltip("One entry per level. Levels with no entry fall back to alternating " +
             "male/female, then to the first entry that has a sprite.")]
    public Entry[] chefs;

    [Tooltip("Used when a level has no entry of its own and no alternating match.")]
    public Sprite fallbackPortrait;

    /// <summary>Portrait for a level, or null if the roster is empty.</summary>
    public Sprite PortraitFor(int level)
    {
        Entry e = EntryFor(level);
        if (e != null && e.portrait != null) return e.portrait;
        if (fallbackPortrait != null) return fallbackPortrait;
        if (chefs != null)
            for (int i = 0; i < chefs.Length; i++)
                if (chefs[i] != null && chefs[i].portrait != null) return chefs[i].portrait;
        return null;
    }

    public Entry EntryFor(int level)
    {
        if (chefs == null || chefs.Length == 0) return null;

        for (int i = 0; i < chefs.Length; i++)
            if (chefs[i] != null && chefs[i].level == level) return chefs[i];

        // No explicit entry: fall back to the alternating pattern, so a future
        // Level 5/6/7 still gets the right chef without editing the asset.
        bool wantEven = (level % 2) == 0;
        for (int i = 0; i < chefs.Length; i++)
            if (chefs[i] != null && chefs[i].portrait != null && ((chefs[i].level % 2) == 0) == wantEven)
                return chefs[i];

        return null;
    }
}
