using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MoroccoStoryDialogueManager : MonoBehaviour
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
    public string levelFourSceneName = "Level Four";

    private string[] dialogues =
    {
        "Excellent! You have completed the Ghanaian challenge.",
        "But our culinary journey doesn't end here.",
        "Our next destination is Morocco!",
        "Morocco is famous for its rich spices, vibrant markets, and delicious cuisine.",
        "In this level, you will prepare four delicious Moroccan dishes.",
        "Tagine, Couscous, Pastilla, and Harira Soup.",
        "Are you ready for the Moroccan challenge?",
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
            LoadLevelFour();
            return;
        }

        ShowDialogue();
    }

    private void SkipStory()
    {
        LoadLevelFour();
    }

    private void LoadLevelFour()
    {
        SceneManager.LoadScene(levelFourSceneName);
    }
}
