using System.Collections.Generic;
using UnityEngine;

public class LevelPreview : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> thingsToActivate;
    [SerializeField] private GameObject ui;

    private void Awake()
    {
        foreach (var script in thingsToActivate) script.enabled = false;
    }

    public void Begin()
    {
        foreach (var script in thingsToActivate) script.enabled = true;
        ui.SetActive(false);
        MusicManager.instance.SetMusicLevel(MusicManager.LEVEL_SIEGE_PLAY);
        MusicManager.instance.Restart();
    }
}