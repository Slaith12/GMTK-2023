using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string TITLE_SCENE = "TitleScreen";
    private const string BUILDER_SCENE = "BuilderTakeTwo";
    private const string LEVEL_SELECT_SCENE = "Level Select";
    private const string SIEGE_SCENE_PREFIX = "Level";
    private static GameManager m_gameManager;
    public List<ModuleData> siegeMachineData;
    [SerializeField] private bool onLevelSelect; //whether to go straight to level select when leaving title screen or go to workshop first

    public static GameManager gameManager
    {
        get
        {
            if (m_gameManager == null) m_gameManager = new GameObject("Game Manager").AddComponent<GameManager>();
            return m_gameManager;
        }
    }

    private void Awake()
    {
        if (m_gameManager != null)
        {
            Destroy(gameObject);
            return;
        }

        m_gameManager = this;
        DontDestroyOnLoad(gameObject);
        onLevelSelect = false;
        siegeMachineData = new List<ModuleData>() { new ModuleData(Builder2.ModuleBase.ModuleTypes["cockpit"], new Vector2Int(3, 1)) };
    }

    public static void GoToTitle()
    {
        SceneManager.LoadScene(TITLE_SCENE);
        MusicManager.instance.SetMusicLevel(gameManager.onLevelSelect
            ? MusicManager.LEVEL_LEVEL_SELECT
            : MusicManager.LEVEL_WORKSHOP);
    }

    public static void LeaveTitleScreen()
    {
        if (gameManager.onLevelSelect)
            GoToLevelSelect();
        else
            GoToBuilder();
    }

    public static void GoToBuilder()
    {
        SceneManager.LoadScene(BUILDER_SCENE);
        MusicManager.instance.SetMusicLevel(MusicManager.LEVEL_WORKSHOP);
        gameManager.onLevelSelect = false;
    }

    public static void SetSiegeMachineData(List<ModuleData> data)
    {
        gameManager.siegeMachineData = data;
        foreach (ModuleData module in data)
        {
            Debug.Log(module);
        }
    }

    public static void GoToLevelSelect()
    {
        SceneManager.LoadScene(LEVEL_SELECT_SCENE);
        MusicManager.instance.SetMusicLevel(MusicManager.LEVEL_LEVEL_SELECT);
        gameManager.onLevelSelect = true;
    }

    public static void GoToSiege(int level)
    {
        SceneManager.LoadScene(SIEGE_SCENE_PREFIX + level);
        MusicManager.instance.SetMusicLevel(MusicManager.LEVEL_SIEGE_PLAY);
        MusicManager.instance.Restart();
    }
}