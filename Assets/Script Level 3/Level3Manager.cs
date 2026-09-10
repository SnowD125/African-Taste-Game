using UnityEngine;

public class Level3Manager : MonoBehaviour
{
    public static Level3Manager Instance;

    [Header("Level 3 Settings")]
    public bool level3Active = true;

    [Header("Available Foods")]
    public Level3FoodType[] availableFoods =
    {
        Level3FoodType.Waakye,
        Level3FoodType.Banku,
        Level3FoodType.Tilapia,
        Level3FoodType.Fufu,
        Level3FoodType.LightSoup
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("LEVEL 3 GHANA SYSTEM STARTED");

        Debug.Log(
            "Foods available: " +
            availableFoods.Length
        );
    }

    public Level3Recipe GetRecipe(Level3FoodType food)
    {
        return Level3Recipes.GetRecipe(food);
    }

    public bool IsLevel3Food(Level3FoodType food)
    {
        for (int i = 0; i < availableFoods.Length; i++)
        {
            if (availableFoods[i] == food)
                return true;
        }

        return false;
    }
}