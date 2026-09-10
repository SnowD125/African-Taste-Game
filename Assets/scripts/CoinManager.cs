using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI coinText;
    public GameObject floatingCoinPrefab;
    public Canvas canvas;

    [Header("Coin Settings")]
    public int maxCoinsPerOrder = 30;
    public int minCoinsPerOrder = 5;

    [Header("Sound")]
    public AudioClip coinSound;

    private int totalCoins = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateCoinDisplay();
    }

    public void AwardCoins(float timeUsed, float maxTime)
    {
        float ratio = 1f - Mathf.Clamp01(timeUsed / maxTime);
        int coinsEarned = Mathf.RoundToInt(
            Mathf.Lerp(minCoinsPerOrder, maxCoinsPerOrder, ratio)
        );

        totalCoins += coinsEarned;
        UpdateCoinDisplay();
        ShowFloatingCoins(coinsEarned);

        if (coinSound != null)
            AudioSource.PlayClipAtPoint(coinSound, Camera.main.transform.position);

        Debug.Log($"Coins earned: {coinsEarned} | Total: {totalCoins}");
    }

    // 🔥 Inaitwa na FirstCustomer kabla ya scene kubadilika
    public int GetTotalCoins()
    {
        return totalCoins;
    }

    void UpdateCoinDisplay()
    {
        if (coinText != null)
            coinText.text = totalCoins.ToString();
    }

    void ShowFloatingCoins(int amount)
    {
        if (floatingCoinPrefab == null || canvas == null) return;

        GameObject floating = Instantiate(floatingCoinPrefab, canvas.transform);
        TextMeshProUGUI text = floating.GetComponent<TextMeshProUGUI>();

        if (text != null)
            text.text = $"+{amount}";

        RectTransform rt = floating.GetComponent<RectTransform>();
        if (rt != null)
            rt.anchoredPosition = new Vector2(0, -100);

        StartCoroutine(FloatUp(floating));
    }

    IEnumerator FloatUp(GameObject obj)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();

        if (rt == null || text == null) yield break;

        float duration = 1.5f;
        float t = 0f;
        Vector2 startPos = rt.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, 200f);
        Color startColor = text.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);
            text.color = Color.Lerp(startColor, endColor, progress);

            yield return null;
        }

        Destroy(obj);
    }
}