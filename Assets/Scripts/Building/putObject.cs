using UnityEngine;
using UnityEngine.UI;

public class putObject : MonoBehaviour
{
    public bool isUsed;
    public GameObject objInComponent;
    public Sprite square;
    public GameObject kingComponent;
    public bool TheresKing;
    public GameObject kingButton;

    private void Start()
    {
        TheresKing = false;
        isUsed = false;
    }

    private void Update()
    {
        if (isUsed)
            GetComponent<Image>().sprite = objInComponent.GetComponent<SpriteRenderer>().sprite;
        else
            GetComponent<Image>().sprite = square;
        if (!GetComponentInParent<siegeBuilder>().TheresKing)
        {
            kingButton.SetActive(true);
            kingComponent.SetActive(false);
        }
        else if (TheresKing)
        {
            kingButton.SetActive(false);
            kingComponent.SetActive(true);
        }
    }

    public void pressbutton()
    {
        if (GetComponentInParent<siegeBuilder>().chosenComponent != null)
        {
            if (GetComponentInParent<siegeBuilder>().chosenComponent.name != "king")
            {
                isUsed = true;
                objInComponent = GetComponentInParent<siegeBuilder>().chosenComponent;
            }
            else
            {
                TheresKing = true;
                GetComponentInParent<siegeBuilder>().TheresKing = true;
            }
        }
        else
        {
            isUsed = false;
            objInComponent = null;
        }

        GetComponentInParent<siegeBuilder>().chosenComponent = null;
    }
}