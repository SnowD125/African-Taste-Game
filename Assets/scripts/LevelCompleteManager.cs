using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelCompleteManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI totalCoinsText;
    public Animator chefAnimator;

    [Header("Sound")]
    public AudioClip celebrationSound;

    [Header("Level Settings")]
    public bool isLevelOneComplete = true;

    // Optional. When set, this scene loads instead of the default branch.
    // Existing scenes leave it empty, so Tanzania -> Nigeria is unchanged.
    public string nextSceneOverride = "";


    void Start()
    {
        int coins = PlayerPrefs.GetInt("TotalCoins", 0);

        if (totalCoinsText != null)
        {
            totalCoinsText.text =
                "<size=46><color=#FFD34D>TOTAL COINS</color></size><br>" +
                "<size=104>" + coins.ToString() + "</size>";
        }

        if (celebrationSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(
                celebrationSound,
                Camera.main.transform.position
            );
        }

        StartCoroutine(ChefEntrance());
    }


    IEnumerator ChefEntrance()
    {
        if (chefAnimator == null)
            yield break;

        RectTransform rt =
            chefAnimator.GetComponent<RectTransform>();

        if (rt == null)
            yield break;

        Vector2 startPos =
            new Vector2(-635f, -1100f);

        Vector2 endPos =
            new Vector2(-635f, -86f);

        float t = 0f;
        float duration = 0.8f;

        rt.anchoredPosition = startPos;

        while (t < duration)
        {
            t += Time.deltaTime;

            float progress = t / duration;

            float easedProgress =
                1f - Mathf.Pow(1f - progress, 3f);

            rt.anchoredPosition =
                Vector2.Lerp(
                    startPos,
                    endPos,
                    easedProgress
                );

            yield return null;
        }

        rt.anchoredPosition = endPos;
    }


    public void OnContinueButton()
    {
        if (!string.IsNullOrEmpty(nextSceneOverride))
        {
            Debug.Log("LEVEL COMPLETE -> " + nextSceneOverride);
            SceneManager.LoadScene(nextSceneOverride);
            return;
        }

        if (isLevelOneComplete)
        {
            Debug.Log(
                "LEVEL ONE COMPLETE → NIGERIA STORY"
            );

            SceneManager.LoadScene("Nigeria Story");
        }
        else
        {
            Debug.Log(
                "LEVEL TWO COMPLETE → MAIN MENU"
            );

            SceneManager.LoadScene("Main Menu");
        }
    }
}