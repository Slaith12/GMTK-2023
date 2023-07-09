using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public const int LEVEL_NONE = 0;
    public const int LEVEL_WORKSHOP = 1;
    public const int LEVEL_LEVEL_SELECT = 2;
    public const int LEVEL_SIEGE_PREVIEW = 3;
    public const int LEVEL_SIEGE_PLAY = 4;

    public static MusicManager instance { get { return m_instance; } }
    private static MusicManager m_instance;

    [SerializeField] AudioSource string1;
    [SerializeField] AudioSource highReed;
    [SerializeField] AudioSource string2;
    [SerializeField] AudioSource flute;
    [SerializeField] AudioSource drums;
    [SerializeField] AudioSource bassReed;

    private void Awake()
    {
        if(m_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Restart();
        SetMusicLevel(LEVEL_WORKSHOP);
    }

    public void SetMusicLevel(int level)
    {
        string1.volume = level >= 1 ? 1 : 0;
        highReed.volume = level >= 1 ? 1 : 0;
        string2.volume = level >= 2 ? 1 : 0;
        flute.volume = level >= 2 ? 1 : 0;
        drums.volume = level >= 3 ? 1 : 0;
        bassReed.volume = level >= 4 ? 1 : 0;
    }

    public void Restart()
    {
        string1.Stop();
        highReed.Stop();
        string2.Stop();
        flute.Stop();
        drums.Stop();
        bassReed.Stop();
        double time = AudioSettings.dspTime;
        string1.PlayScheduled(time);
        highReed.PlayScheduled(time);
        string2.PlayScheduled(time);
        flute.PlayScheduled(time);
        drums.PlayScheduled(time);
        bassReed.PlayScheduled(time);
    }
}
