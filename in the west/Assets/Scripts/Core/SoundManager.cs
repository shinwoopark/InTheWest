using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager;

    public AudioSource BGmSource;
    public AudioClip[] BGmClips;

    private void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        BGmSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene0, LoadSceneMode scene1)
    {
        for (int i = 0; i < BGmClips.Length; i++)
        {
            if (scene0.name == BGmClips[i].name)
            {
                PlayBGM(BGmClips[i]);
            }             
        }
    }

    private void Update()
    {
        
    }

    public void PlayBGM(AudioClip clip)
    {
        BGmSource.clip = clip;
        BGmSource.loop = true;
        BGmSource.volume = 0.1f;
        BGmSource.Play();
    }
}
