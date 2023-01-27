using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    private GameObject audioManagerObject;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource atmoSource;
    [SerializeField] private AudioSource fxSource;

    #region ClipsRegion
    // Music & Atmo
    [SerializeField] private AudioClip mainThemeClip;
    [SerializeField] private AudioClip atmoClip;

    // Actions
    [SerializeField] private AudioClip buttonPressClip;
    [SerializeField] private AudioClip turnChangeClip;
    [SerializeField] private AudioClip unitDraftedClip;
    [SerializeField] private AudioClip unitPlacedClip;
    [SerializeField] private AudioClip moveClip; // hat idealerweise auch einen pro Einheit, aber erstmal egal
    [SerializeField] private AudioClip tankAttackClip;
    [SerializeField] private AudioClip shooterAttackClip;
    [SerializeField] private AudioClip runnerDoubleAttackClip;
    [SerializeField] private AudioClip mechanicAttackClip;
    [SerializeField] private AudioClip docAttackClip;
    [SerializeField] private AudioClip takeDamageClip; // erstmal nur einen, nicht als hit oder voice, sondern als lebenspunkte verlieren
    [SerializeField] private AudioClip takeControlClip;
    [SerializeField] private AudioClip blockClip;
    [SerializeField] private AudioClip powershotClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip changeFloorToHoleClip;
    [SerializeField] private AudioClip changeHoleToFloorClip;
    [SerializeField] private AudioClip healClip;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip adrenalinClip;
    
    // Voicelines
    [SerializeField] private AudioClip masterVoicelineClip;
    [SerializeField] private AudioClip tankVoicelineClip;
    [SerializeField] private AudioClip shooterVoicelineClip;
    [SerializeField] private AudioClip runnerVoicelineClip;
    [SerializeField] private AudioClip mechanicVoicelineClip;
    [SerializeField] private AudioClip medicVoicelineClip;
    #endregion

    private void Awake()
    {
        #region SingletonImpelementation
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        #endregion
        SubscribeEvents();
    }

    private void Start()
    {
        audioManagerObject = this.gameObject;
        DontDestroyOnLoad(audioManagerObject);
    }

    private void PlayMainTheme()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.clip = mainThemeClip;
            musicSource.loop = true;
            musicSource.volume = 0.5f;
            musicSource.Play();
        }
    }

    private void StopMainTheme()
    {
        StartCoroutine(FadeAudio(musicSource, 2f, 0f));
    }

    private void PlayAtmo()
    {
        if (!atmoSource.isPlaying)
        {
            atmoSource.clip = atmoClip;
            atmoSource.loop = true;
            atmoSource.Play();
        }
    }

    private void TurnChangeAudio(Player player)
    {
        fxSource.PlayOneShot(turnChangeClip);
    }

    private IEnumerator FadeAudio(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float startVolume = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }

        audioSource.Stop();
        yield break;
    }

    #region EventsRegion
    private void SubscribeEvents()
    {
        AudioEvents.OnMainMenuEnter += PlayMainTheme;
        GameplayEvents.OnPlayerTurnEnded += TurnChangeAudio;
        GameEvents.OnGameStart += StopMainTheme;
    }

    private void UnsubscribeEvents()
    {
        AudioEvents.OnMainMenuEnter -= PlayMainTheme;
        GameplayEvents.OnPlayerTurnEnded -= TurnChangeAudio;
        GameEvents.OnGameStart -= StopMainTheme;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion
}