using UnityEngine;
using System.Collections;

public class CustomerOrder : MonoBehaviour
{
    [Header("Food Icons")]
    public GameObject ugaliIcon;
    public GameObject dagaaIcon;

    [Header("Served UI")]
    public GameObject servedText;
    public GameObject tickIcon;

    [Header("Customer Reference")]
    public FirstCustomer firstCustomer; // bado ni ile ile (no change)

    void Start()
    {
        ShowOrder();
    }

    public void ShowOrder()
    {
        gameObject.SetActive(true);

        if (ugaliIcon != null) ugaliIcon.SetActive(true);
        if (dagaaIcon != null) dagaaIcon.SetActive(true);

        if (servedText != null) servedText.SetActive(false);
        if (tickIcon != null) tickIcon.SetActive(false);
    }

    public void HideOrder()
    {
        gameObject.SetActive(false);
    }

    public void ShowServed()
    {
        if (ugaliIcon != null) ugaliIcon.SetActive(false);
        if (dagaaIcon != null) dagaaIcon.SetActive(false);

        if (servedText != null) servedText.SetActive(true);
        if (tickIcon != null) tickIcon.SetActive(true);

        StartCoroutine(ServedDelay());
    }

    IEnumerator ServedDelay()
    {
        yield return new WaitForSeconds(2f);

        if (firstCustomer != null)
        {
            firstCustomer.ServeCustomer();
        }
    }
}