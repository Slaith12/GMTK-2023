using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    const string TITLE_SCENE = "TitleScreen";
    const string BUILDER_SCENE = "BuilderTakeTwo";
    const string LEVEL_SELECT_SCENE = "Level Select";
    const string SIEGE_SCENE_PREFIX = "Level";

    static GameManager gameManager { get 
        {
            if(m_gameManager == null)
            {
                m_gameManager = new GameObject("Game Manager").AddComponent<GameManager>();
            }
            return m_gameManager;
        } }
    private static GameManager m_gameManager;
    private object siegeMachineData; //TODO: replace with proper data type

    private void Awake()
    {
        if(m_gameManager != null)
        {
            Destroy(gameObject);
        }
        m_gameManager = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void GoToTitle()
    {
        SceneManager.LoadScene(TITLE_SCENE);
    }

    public static void GoToBuilder()
    {
        SceneManager.LoadScene(BUILDER_SCENE);
    }

    public static void SetSiegeMachineData(object data) //TODO: replace with proper data type
    {
        gameManager.siegeMachineData = data;
    }

    public static void GoToLevelSelect()
    {
        SceneManager.LoadScene(LEVEL_SELECT_SCENE);
    }

    public static void GoToSiege(int level)
    {
        SceneManager.LoadScene(SIEGE_SCENE_PREFIX + level);
        //change music
    }
}
