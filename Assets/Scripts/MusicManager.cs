using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public const int LEVEL_NONE = 0;
    public const int LEVEL_LEVEL_SELECT = 1;
    public const int LEVEL_WORKSHOP_FROM_LS = 1;
    public const int LEVEL_WORKSHOP_FROM_PREVIEW = 2;
    public const int LEVEL_SIEGE_PREVIEW = 3;
    public const int LEVEL_SIEGE_PLAY = 4;

    [SerializeField] private AudioSource string1;
    [SerializeField] private AudioSource highReed;
    [SerializeField] private AudioSource string2;
    [SerializeField] private AudioSource flute;
    [SerializeField] private AudioSource drums;
    [SerializeField] private AudioSource bassReed;

    private int currentLevel;
    private Coroutine currentTransition;

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
        SetMusicLevel(1, withTransition: false);
    }

    public void SetMusicLevel(int level, bool withTransition = true)
    {
        if (level == currentLevel)
            return;
        int[] instLevels = {
            1, //string 1
            1, //high reed
            //3, //string 2
            2, //flute
            3, //drums
            4, //bass reed
        };
        AudioSource[] instruments = new AudioSource[] { string1, highReed, /*string2,*/ flute, drums, bassReed };

        if (!withTransition)
        {
            string1.volume = level >= instLevels[0] ? 1 : 0;
            highReed.volume = level >= instLevels[1] ? 1 : 0;
            //string2.volume = level >= instLevels[] ? 1 : 0;
            flute.volume = level >= instLevels[2] ? 1 : 0;
            drums.volume = level >= instLevels[3] ? 1 : 0;
            bassReed.volume = level >= instLevels[4] ? 1 : 0;
            currentLevel = level;
        }
        else
        {
            if(currentTransition != null)
            {
                StopCoroutine(currentTransition);
            }
            if(level < currentLevel) //removing instruments
            {
                SetMusicLevel(currentLevel, false); //instantly remove instruments that won't be included in transition
                List<AudioSource> transitioningInstruments = new List<AudioSource>();
                for(int i = 0; i < instLevels.Length; i++)
                {
                    if (instLevels[i] > level && instLevels[i] <= currentLevel)
                        transitioningInstruments.Add(instruments[i]);
                }
                currentTransition = StartCoroutine(FadeOutInstruments(transitioningInstruments));
                currentLevel = level;
            }
            else
            {
                SetMusicLevel(currentLevel, false); //instantly add instruments that won't be included in transition
                List<AudioSource> transitioningInstruments = new List<AudioSource>();
                for (int i = 0; i < instLevels.Length; i++)
                {
                    if (instLevels[i] <= level && instLevels[i] > currentLevel)
                        transitioningInstruments.Add(instruments[i]);
                }
                currentTransition = StartCoroutine(FadeInInstruments(transitioningInstruments));
                currentLevel = level;
            }
        }
    }

    private IEnumerator FadeInInstruments(List<AudioSource> instruments)
    {
        double startTime = AudioSettings.dspTime;
        double endTime = startTime + 0.75f;
        while (AudioSettings.dspTime < endTime)
        {
            double currentTime = AudioSettings.dspTime;
            float percentage = (float)((currentTime - startTime) / (endTime - startTime));
            foreach (AudioSource instrument in instruments)
                instrument.volume = percentage;
            yield return null;
        }
        foreach (AudioSource instrument in instruments)
            instrument.volume = 1;
    }

    private IEnumerator FadeOutInstruments(List<AudioSource> instruments)
    {
        double startTime = AudioSettings.dspTime;
        double endTime = startTime + 0.75f;
        while(AudioSettings.dspTime < endTime)
        {
            double currentTime = AudioSettings.dspTime;
            float percentage = (float)((currentTime - startTime) / (endTime - startTime));
            foreach (AudioSource instrument in instruments)
                instrument.volume = 1 - percentage;
            yield return null;
        }
        foreach (AudioSource instrument in instruments)
            instrument.volume = 0;
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