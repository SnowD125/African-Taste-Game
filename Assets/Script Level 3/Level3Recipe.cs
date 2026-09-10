using System.Collections.Generic;
using UnityEngine;

public enum Level3FoodType
{
    None,
    Waakye,
    Banku,
    Tilapia,
    Fufu,
    LightSoup
}

public enum Level3CookingState
{
    Empty,
    IngredientsAdded,
    Cooking,
    Ready,
    Burned
}

[System.Serializable]
public class Level3Recipe
{
    public Level3FoodType foodType;
    public string displayName;

    [TextArea(2, 5)]
    public string[] requiredIngredients;

    public float cookingTime = 5f;
}

public static class Level3Recipes
{
    public static Level3Recipe GetRecipe(Level3FoodType food)
    {
        switch (food)
        {
            case Level3FoodType.Waakye:
                return new Level3Recipe
                {
                    foodType = Level3FoodType.Waakye,
                    displayName = "Waakye",
                    requiredIngredients = new string[]
                    {
                        "Rice",
                        "Beans",
                        "Waakye Leaves",
                        "Salt"
                    },
                    cookingTime = 8f
                };

            case Level3FoodType.Banku:
                return new Level3Recipe
                {
                    foodType = Level3FoodType.Banku,
                    displayName = "Banku",
                    requiredIngredients = new string[]
                    {
                        "Corn Dough",
                        "Cassava Dough",
                        "Salt"
                    },
                    cookingTime = 7f
                };

            case Level3FoodType.Tilapia:
                return new Level3Recipe
                {
                    foodType = Level3FoodType.Tilapia,
                    displayName = "Grilled Tilapia",
                    requiredIngredients = new string[]
                    {
                        "Raw Tilapia",
                        "Ginger"
                    },
                    cookingTime = 6f
                };

            case Level3FoodType.Fufu:
                return new Level3Recipe
                {
                    foodType = Level3FoodType.Fufu,
                    displayName = "Fufu",
                    requiredIngredients = new string[]
                    {
                        "Cassava",
                        "Plantain"
                    },
                    cookingTime = 8f
                };

            case Level3FoodType.LightSoup:
                return new Level3Recipe
                {
                    foodType = Level3FoodType.LightSoup,
                    displayName = "Light Soup",
                    requiredIngredients = new string[]
                    {
                        "Meat",
                        "Tomato",
                        "Onion",
                        "Pepper",
                        "Ginger",
                        "Salt"
                    },
                    cookingTime = 10f
                };
        }

        return null;
    }
}