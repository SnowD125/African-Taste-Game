using UnityEngine;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
    public GameObject orderCanvas;

    public Image foodImage;

    public Sprite ugaliDagaaSprite;
    public Sprite dafuSprite;

    int selectedFood;

    void Start()
    {
        if (orderCanvas != null)
            orderCanvas.SetActive(false);
    }

    public void ShowOrder(int foodIndex)
    {
        selectedFood = foodIndex;

        if (orderCanvas != null)
            orderCanvas.SetActive(true);

        if (selectedFood == 0)
        {
            foodImage.sprite = ugaliDagaaSprite;
        }
        else
        {
            foodImage.sprite = dafuSprite;
        }

        foodImage.gameObject.SetActive(true);
    }

    // HII NDIYO MPYA
    public void HideFoodImage()
    {
        if (foodImage != null)
            foodImage.gameObject.SetActive(false);
    }
}