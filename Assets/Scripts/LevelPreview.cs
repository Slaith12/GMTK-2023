using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this is just so the level preview can grab the correct script when adding them in the inspector
//it's a bit of a bodge but it should be fine
public abstract class Activatable : MonoBehaviour
{ }

public class LevelPreview : MonoBehaviour
{
    [SerializeField] private Activatable[] thingsToActivate;
    [SerializeField] private GameObject ui;

    private void Awake()
    {
        foreach (var script in thingsToActivate) ((MonoBehaviour)script).enabled = false;
    }

    public void Begin()
    {
        foreach (var script in thingsToActivate) ((MonoBehaviour)script).enabled = true;
        ui.SetActive(false);
        MusicManager.instance.SetMusicLevel(MusicManager.LEVEL_SIEGE_PLAY);
        MusicManager.instance.Restart();
    }

    public void GoToLevelSelect()
    {
        GameManager.GoToLevelSelect();
    }

    public void GoToWorkshop()
    {
        GameManager.GoToBuilder();
    }
}