using UnityEngine;

/// <summary>
/// Minimal, self-contained level progression for African Taste.
/// Uses PlayerPrefs, the project's existing save mechanism (already used for
/// "TotalCoins"). No second save system, and no existing key is touched.
///
/// Tanzania (1) -> Nigeria (2) -> Ghana (3)
/// Level 1 always unlocked. Level N unlocks when N-1 is completed.
/// Completed levels stay replayable.
/// </summary>
public static class LevelProgress
{
    public const int LevelCount = 3;

    const string CompletedKey = "LevelCompleted_";
    const string SelectedKey = "SelectedLevel";

    public static bool IsCompleted(int level)
    {
        if (level < 1 || level > LevelCount) return false;
        return PlayerPrefs.GetInt(CompletedKey + level, 0) == 1;
    }

    public static void MarkCompleted(int level)
    {
        if (level < 1 || level > LevelCount) return;
        PlayerPrefs.SetInt(CompletedKey + level, 1);
        PlayerPrefs.Save();
        Debug.Log("LevelProgress: level " + level + " completed.");
    }

    public static bool IsUnlocked(int level)
    {
        if (level < 1 || level > LevelCount) return false;
        if (level == 1) return true;
        return IsCompleted(level - 1);
    }

    public static int HighestUnlocked()
    {
        int highest = 1;
        for (int i = 2; i <= LevelCount; i++) if (IsUnlocked(i)) highest = i;
        return highest;
    }

    public static int SelectedLevel
    {
        get
        {
            int v = PlayerPrefs.GetInt(SelectedKey, 1);
            if (!IsUnlocked(v)) v = HighestUnlocked();
            return v;
        }
        set
        {
            if (!IsUnlocked(value)) return;
            PlayerPrefs.SetInt(SelectedKey, value);
            PlayerPrefs.Save();
        }
    }

    public static string StorySceneFor(int level)
    {
        switch (level)
        {
            case 1: return "Tanzania Story";
            case 2: return "Nigeria Story";
            case 3: return "Ghana Story";
        }
        return "Tanzania Story";
    }

    public static string CountryFor(int level)
    {
        switch (level)
        {
            case 1: return "TANZANIA";
            case 2: return "NIGERIA";
            case 3: return "GHANA";
        }
        return "TANZANIA";
    }

    /// <summary>Clears progression only. Does not touch TotalCoins.</summary>
    public static void ResetProgress()
    {
        for (int i = 1; i <= LevelCount; i++) PlayerPrefs.DeleteKey(CompletedKey + i);
        PlayerPrefs.DeleteKey(SelectedKey);
        PlayerPrefs.Save();
    }
}
