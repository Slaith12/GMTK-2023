using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public const int LEVEL_NONE = 0;
    public const int LEVEL_WORKSHOP = 1;
    public const int LEVEL_LEVEL_SELECT = 2;
    public const int LEVEL_SIEGE_PREVIEW = 3;
    public const int LEVEL_SIEGE_PLAY = 4;

    [SerializeField] private AudioSource string1;
    [SerializeField] private AudioSource highReed;
    [SerializeField] private AudioSource string2;
    [SerializeField] private AudioSource flute;
    [SerializeField] private AudioSource drums;
    [SerializeField] private AudioSource bassReed;

    public static MusicManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
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
        var time = AudioSettings.dspTime;
        string1.PlayScheduled(time);
        highReed.PlayScheduled(time);
        string2.PlayScheduled(time);
        flute.PlayScheduled(time);
        drums.PlayScheduled(time);
        bassReed.PlayScheduled(time);
    }
}