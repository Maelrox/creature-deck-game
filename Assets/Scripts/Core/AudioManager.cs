using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource menuMusic;
    public AudioSource battleSelectMusic;
    public AudioSource[] bgm;
    public AudioSource attackSound;
    private int currentBGM;
    private bool playingBGM;
    public AudioSource[] sfx;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (playingBGM && !bgm[currentBGM].isPlaying)
        {
            currentBGM++;
            if (currentBGM >= bgm.Length)
            {
                currentBGM = 0;
            }
            bgm[currentBGM].Play();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {

        if (pauseStatus)
        {
            StopMusic();
        }
        else
        {
            if (!playingBGM) {
                PlayBGM();
            }
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {

        if (!hasFocus)
        {
            StopMusic();
        }
        else
        {
            if (!playingBGM)
            {
                PlayBGM();
            }
        }
    }

    public void StopMusic()
    {
        menuMusic.Stop();
        battleSelectMusic.Stop();
        foreach (AudioSource track in bgm)
        {
            track.Stop();
        }
        playingBGM = false;
    }

    public void PlayMenuMusic()
    {
        StopMusic();
        menuMusic.Play();
    }

    public void PlayBattleSelectMusic()
    {
        if (!battleSelectMusic.isPlaying)
        {
            StopMusic();
            battleSelectMusic.Play();
        }
    }

    public void PlayBGM()
    {
        StopMusic();
        currentBGM = Random.Range(0, bgm.Length);
        bgm[currentBGM].Play();
        playingBGM = true;
    }

    public void PlaySFX(int sfxToPlay)
    {
        sfx[sfxToPlay].Stop();
        sfx[sfxToPlay].Play();
    }

    public void PlayHit()
    {
        attackSound.Play();
    }
}