using UnityEngine;

public class Level3CustomerOrder : MonoBehaviour
{
    [Header("Current Order")]
    public Level3FoodType mainFood = Level3FoodType.None;
    public Level3FoodType sideFood = Level3FoodType.None;

    [Header("Order Status")]
    public bool orderServed = false;

    public void GenerateRandomOrder()
    {
        int randomOrder = Random.Range(0, 3);

        switch (randomOrder)
        {
            case 0:
                // Waakye
                mainFood = Level3FoodType.Waakye;
                sideFood = Level3FoodType.None;
                break;

            case 1:
                // Banku & Tilapia
                mainFood = Level3FoodType.Banku;
                sideFood = Level3FoodType.Tilapia;
                break;

            case 2:
                // Fufu & Light Soup
                mainFood = Level3FoodType.Fufu;
                sideFood = Level3FoodType.LightSoup;
                break;
        }

        orderServed = false;

        Debug.Log(
            "LEVEL 3 ORDER: " +
            GetOrderName()
        );
    }

    public string GetOrderName()
    {
        if (mainFood == Level3FoodType.Waakye)
            return "Waakye";

        if (mainFood == Level3FoodType.Banku &&
            sideFood == Level3FoodType.Tilapia)
            return "Banku & Tilapia";

        if (mainFood == Level3FoodType.Fufu &&
            sideFood == Level3FoodType.LightSoup)
            return "Fufu & Light Soup";

        return "Unknown Order";
    }

    public bool IsCorrectFood(Level3FoodType food)
    {
        return food == mainFood || food == sideFood;
    }

    public bool IsCompleteOrder(
        Level3FoodType firstFood,
        Level3FoodType secondFood)
    {
        bool firstCorrect =
            firstFood == mainFood ||
            firstFood == sideFood;

        bool secondCorrect =
            secondFood == mainFood ||
            secondFood == sideFood;

        return firstCorrect && secondCorrect;
    }

    public void MarkServed()
    {
        orderServed = true;

        Debug.Log(
            "Customer served: " +
            GetOrderName()
        );
    }
}