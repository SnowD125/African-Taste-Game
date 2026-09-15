using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GhanaStoryDialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text characterNameText;
    public TMP_Text dialogueText;
    public Button nextButton;
    public Button skipButton;

    [Header("Chef Styles")]
    public SpriteRenderer chefSpriteRenderer;
    public Sprite[] chefStyles;

    [Header("Next Scene")]
    public string levelThreeSceneName = "Level Three";

    private string[] dialogues =
    {
        "Excellent work! You have completed the Nigerian challenge.",
        "But our culinary journey doesn't end here.",
        "Our next destination is Ghana!",
        "Ghana is famous for its bold coastal flavours and hearty street food.",
        "In this level, you will prepare three delicious Ghanaian dishes.",
        "Waakye, Banku & Tilapia, and Fufu & Light Soup.",
        "Are you ready for the Ghanaian challenge?",
        "Let's start cooking!"
    };

    private int currentDialogueIndex = 0;

    private void Start()
    {
        characterNameText.text = "Chef";

        nextButton.onClick.AddListener(NextDialogue);
        skipButton.onClick.AddListener(SkipStory);

        ShowDialogue();
    }

    private void ShowDialogue()
    {
        dialogueText.text = dialogues[currentDialogueIndex];

        if (chefSpriteRenderer != null &&
            chefStyles != null &&
            currentDialogueIndex < chefStyles.Length &&
            chefStyles[currentDialogueIndex] != null)
        {
            chefSpriteRenderer.sprite = chefStyles[currentDialogueIndex];
        }
    }

    private void NextDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex >= dialogues.Length)
        {
            LoadLevelThree();
            return;
        }

        ShowDialogue();
    }

    private void SkipStory()
    {
        LoadLevelThree();
    }

    private void LoadLevelThree()
    {
        SceneManager.LoadScene(levelThreeSceneName);
    }
}
