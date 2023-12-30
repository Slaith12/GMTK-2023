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
    private bool onLevelSelect => currentLevel == 0;
    private int currentLevel;

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
        currentLevel = 0;
        siegeMachineData = new List<ModuleData>() { new ModuleData(Builder2.ModuleBase.ModuleTypes["cockpit"], new Vector2Int(3, 1)) };
    }

    public static void GoToTitle()
    {
        SceneManager.LoadScene(TITLE_SCENE);
        MusicManager.instance.SetMusicLevel(MusicManager.LEVEL_LEVEL_SELECT);
    }

    public static void LeaveTitleScreen()
    {
        GoToLevelSelect();
    }

    public static void GoToBuilder()
    {
        SceneManager.LoadScene(BUILDER_SCENE);
        MusicManager.instance.SetMusicLevel(gameManager.onLevelSelect ? MusicManager.LEVEL_WORKSHOP_FROM_LS : MusicManager.LEVEL_WORKSHOP_FROM_PREVIEW);
    }

    public static void LeaveBuilder()
    {
        if (gameManager.onLevelSelect)
            GoToLevelSelect();
        else
            GoToSiege(gameManager.currentLevel);
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
        gameManager.currentLevel = 0;
    }

    public static void GoToSiege(int level)
    {
        SceneManager.LoadScene(SIEGE_SCENE_PREFIX + level);
        MusicManager.instance.SetMusicLevel(MusicManager.LEVEL_SIEGE_PREVIEW);
        gameManager.currentLevel = level;
    }
}