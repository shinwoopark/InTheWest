using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.VisualScripting;

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager;

    [Header("BGM")]
    AudioSource _bGmSource;
    public AudioClip[] BgmClips;
    public float BgmVolume;

    public enum Bgm {MainMenu, Play}

    [Header("SFX")]
    AudioSource[] _sfxSource;
    public AudioClip[] SfxClips;
    public float SfxVolume;
    public int Channels;
    private int _channelIndex;

    public enum Sfx { Pistol, Rifle, Reload, Roll, FireBlindlyPistol, GetItem, Hit, Swich, Swing, Throw, Swings, SpAtk1, SpAtk2, QuickReload, GameClear, GameOver }

    private void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        Init();
    }

    private void Init()
    {
        GameObject bgmObject = new GameObject("Bgm");
        bgmObject.transform.parent = transform;
        _bGmSource = bgmObject.AddComponent<AudioSource>();
        _bGmSource.playOnAwake = false;
        _bGmSource.loop = true;
        _bGmSource.volume = BgmVolume;

        GameObject sfxObject = new GameObject("Sfx");
        sfxObject.transform.parent = transform;
        _sfxSource = new AudioSource[Channels];

        for (int i = 0; i < _sfxSource.Length; i++)
        {
            _sfxSource[i] = sfxObject.AddComponent<AudioSource>();
            _sfxSource[i].playOnAwake = false;
            _sfxSource[i].volume = SfxVolume;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainScene":
                PlayBgm(Bgm.MainMenu);
                break;
            case "PlayScene":
                PlayBgm(Bgm.Play);
                break;
        }
    }

    public void PlayBgm(Bgm bgm)
    {
        if(_bGmSource.clip != null)
            _bGmSource.Stop();

        _bGmSource.clip = BgmClips[(int)bgm];
        _bGmSource.Play();
    }

    public void StopBgm()
    {
        _bGmSource.Stop();
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < _sfxSource.Length; i++)
        {
            int loopIndex = (i + _channelIndex) % _sfxSource.Length;

            if (_sfxSource[loopIndex].isPlaying)
                continue;

            _channelIndex = loopIndex;
            _sfxSource[loopIndex].clip = SfxClips[(int)sfx];
            _sfxSource[loopIndex].Play();
            break;
        }
    }
}
