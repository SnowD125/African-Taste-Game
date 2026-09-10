using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public Image fillImage;
    public TextMeshProUGUI percentText;
    public float speed = 0.5f;

    void Start()
    {
        fillImage.fillAmount = 0f;
    }

    void Update()
    {
        fillImage.fillAmount += Time.deltaTime * speed;

        int percent = Mathf.RoundToInt(fillImage.fillAmount * 100);
        percentText.text = percent + "%";

        if (fillImage.fillAmount >= 1f)
        {
            SceneManager.LoadScene(2);
        }
    }
}
