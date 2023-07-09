using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] TMP_Text endText;
    [SerializeField] GameObject retryButton;
    [SerializeField] GameObject nextLevelButton;

    public void GoToLevel(int level)
    {
        Time.timeScale = 1;
        GameManager.GoToSiege(level);
    }

    public void GoToLevelSelect()
    {
        Time.timeScale = 1;
        GameManager.GoToLevelSelect();
    }

    public void ShowWin()
    {
        gameObject.SetActive(true);
        endText.text = "You Win";
        retryButton.SetActive(false);
        nextLevelButton.SetActive(true);
        Time.timeScale = 0;
    }

    public void ShowLose()
    {
        gameObject.SetActive(true);
        endText.text = "You Lose";
        retryButton.SetActive(true);
        nextLevelButton.SetActive(false);
    }
}
