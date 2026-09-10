using UnityEngine;

public class OilClick : MonoBehaviour
{
    public VegCookingManager manager;
    public JollofCookingManager jollofManager; // 🔥 NEW
    public Animator bottleAnimator;
    public float pourDelay = 0.7f;

    bool isPouring = false;

    void Start()
    {
        if (bottleAnimator != null)
            bottleAnimator.enabled = false;
    }

    void OnMouseDown()
    {
        if (isPouring) return;

        // 🔥 Angalia ni manager yupi anayetumika
        if (manager != null && manager.currentStep == 0)
        {
            isPouring = true;
            PlayAnimation();
            Invoke(nameof(FinishPourVeg), pourDelay);
            return;
        }

        if (jollofManager != null && jollofManager.currentStep == 6)
        {
            isPouring = true;
            PlayAnimation();
            Invoke(nameof(FinishPourJollof), pourDelay);
            return;
        }

        Debug.Log($"OilClick — hakuna manager anayekubali click hii (vegStep={manager?.currentStep} jollofStep={jollofManager?.currentStep})");
    }

    void PlayAnimation()
    {
        if (bottleAnimator != null)
        {
            bottleAnimator.enabled = true;
            bottleAnimator.SetTrigger("Pour");
        }
    }

    void FinishPourVeg()
    {
        Debug.Log("FinishPour — calling ClickOil (VegCookingManager)");
        manager?.ClickOil();
        isPouring = false;
    }

    void FinishPourJollof()
    {
        Debug.Log("FinishPour — calling ShowOilInPan (JollofCookingManager)");
        jollofManager?.ShowOilInPan();
        isPouring = false;
    }

    public void SimulateClick()
    {
        OnMouseDown();
    }

    public void ResetBottle()
    {
        isPouring = false;
        CancelInvoke();
        if (bottleAnimator != null)
            bottleAnimator.enabled = false;
    }
}