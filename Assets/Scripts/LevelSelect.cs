using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public void GoToTitleScreen()
    {
        GameManager.GoToTitle();
    }

    public void GoToWorkshop()
    {
        GameManager.GoToBuilder();
    }

    public void GoToLevel(int level)
    {
        GameManager.GoToSiege(level);
    }
}