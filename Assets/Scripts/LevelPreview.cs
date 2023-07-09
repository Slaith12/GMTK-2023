using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPreview : MonoBehaviour
{
    [SerializeField] List<MonoBehaviour> thingsToActivate;
    [SerializeField] GameObject ui;

    private void Awake()
    {
        foreach(MonoBehaviour script in thingsToActivate)
        {
            script.enabled = false;
        }
    }

    public void Begin()
    {
        foreach(MonoBehaviour script in thingsToActivate)
        {
            script.enabled = true;
        }
        ui.SetActive(false);
        MusicManager.instance.SetMusicLevel(MusicManager.LEVEL_SIEGE_PLAY);
        MusicManager.instance.Restart();

    }
}
