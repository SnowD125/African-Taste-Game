using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryDialogueManager : MonoBehaviour
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
    public string levelOneSceneName = "Level_1_Tanzania";

    private string[] dialogues =
    {
        "Welcome to Tanzania!",
        "Tanzania is a country rich in culture and traditional foods.",
        "In this first level, you will prepare two traditional Tanzanian meals.",
        "First, we have Ugali with Tembele.",
        "And then, Ugali with Dagaa.",
        "Are you ready to discover the taste of Tanzania?",
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
            LoadLevelOne();
            return;
        }

        ShowDialogue();
    }

    private void SkipStory()
    {
        LoadLevelOne();
    }

    private void LoadLevelOne()
    {
        SceneManager.LoadScene(levelOneSceneName);
    }
}