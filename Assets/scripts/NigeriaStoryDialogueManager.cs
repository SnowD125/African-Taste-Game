using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NigeriaStoryDialogueManager : MonoBehaviour
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
    public string levelTwoSceneName = "Level Two";

    private string[] dialogues =
    {
        "Excellent work! You have completed the Tanzanian challenge.",
        "But our culinary journey doesn't end here.",
        "Our next destination is Nigeria!",
        "Nigeria is famous for its rich and flavorful cuisine.",
        "In this level, you will prepare three delicious Nigerian dishes.",
        "Jollof Rice, Egusi Soup, and Pounded Yam.",
        "Are you ready for the Nigerian challenge?",
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
            LoadLevelTwo();
            return;
        }

        ShowDialogue();
    }

    private void SkipStory()
    {
        LoadLevelTwo();
    }

    private void LoadLevelTwo()
    {
        SceneManager.LoadScene(levelTwoSceneName);
    }
}