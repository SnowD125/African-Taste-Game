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


    void Start()
    {
        int coins = PlayerPrefs.GetInt("TotalCoins", 0);

        if (totalCoinsText != null)
        {
            totalCoinsText.text =
                "Total Coins: " + coins.ToString();
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
            new Vector2(-450f, -600f);

        Vector2 endPos =
            new Vector2(-450f, -50f);

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