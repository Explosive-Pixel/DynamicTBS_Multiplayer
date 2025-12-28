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
    [SerializeField] private AudioSource voiceSource;

    private List<AudioClip> moveClipsList = new List<AudioClip>();
    private List<AudioClip> masterVoiceClipsList = new List<AudioClip>();
    private List<AudioClip> tankVoiceClipsList = new List<AudioClip>();
    private List<AudioClip> shooterVoiceClipsList = new List<AudioClip>();
    private List<AudioClip> runnerVoiceClipsList = new List<AudioClip>();
    private List<AudioClip> mechanicVoiceClipsList = new List<AudioClip>();
    private List<AudioClip> medicVoiceClipsList = new List<AudioClip>();

    private bool subscriptionsActive;

    #region ClipsRegion
    // Music & Atmo
    [SerializeField] private AudioClip mainThemeClip;
    [SerializeField] private AudioClip atmoClip;

    // Actions
    [SerializeField] private AudioClip buttonPressClip;
    [SerializeField] private AudioClip turnChangeClip;
    [SerializeField] private AudioClip unitDraftedClip;
    [SerializeField] private AudioClip unitPlacedClip;
    [SerializeField] private AudioClip moveClip1;
    [SerializeField] private AudioClip moveClip2;
    [SerializeField] private AudioClip moveClip3;
    [SerializeField] private AudioClip tankAttackClip;
    [SerializeField] private AudioClip shooterAttackClip;
    [SerializeField] private AudioClip runnerDoubleAttackClip;
    [SerializeField] private AudioClip mechanicAttackClip;
    [SerializeField] private AudioClip medicAttackClip;
    [SerializeField] private AudioClip takeDamageClip;
    [SerializeField] private AudioClip takeControlClip;
    [SerializeField] private AudioClip blockClip;
    [SerializeField] private AudioClip powershotClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip changeFloorClip;
    [SerializeField] private AudioClip healClip;
    [SerializeField] private AudioClip lastTenSeconds;
    [SerializeField] private AudioClip timeRanOut;
    [SerializeField] private AudioClip rechargeClip;

    // Voicelines
    [SerializeField] private AudioClip masterVoicelineClip1;
    [SerializeField] private AudioClip masterVoicelineClip2;
    [SerializeField] private AudioClip masterVoicelineClip3;
    [SerializeField] private AudioClip tankVoicelineClip1;
    [SerializeField] private AudioClip tankVoicelineClip2;
    [SerializeField] private AudioClip tankVoicelineClip3;
    [SerializeField] private AudioClip shooterVoicelineClip1;
    [SerializeField] private AudioClip shooterVoicelineClip2;
    [SerializeField] private AudioClip shooterVoicelineClip3;
    [SerializeField] private AudioClip runnerVoicelineClip1;
    [SerializeField] private AudioClip runnerVoicelineClip2;
    [SerializeField] private AudioClip runnerVoicelineClip3;
    [SerializeField] private AudioClip mechanicVoicelineClip1;
    [SerializeField] private AudioClip mechanicVoicelineClip2;
    [SerializeField] private AudioClip mechanicVoicelineClip3;
    [SerializeField] private AudioClip medicVoicelineClip1;
    [SerializeField] private AudioClip medicVoicelineClip2;
    [SerializeField] private AudioClip medicVoicelineClip3;
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
        subscriptionsActive = false;
    }

    private void Start()
    {
        SubscribeEvents();
        audioManagerObject = this.gameObject;
        DontDestroyOnLoad(audioManagerObject);
        SetClipsLists();
    }

    private void SetClipsLists()
    {
        if (moveClipsList.Count == 0)
        {
            moveClipsList.Add(moveClip1);
            moveClipsList.Add(moveClip2);
            moveClipsList.Add(moveClip3);
        }
        if (masterVoiceClipsList.Count == 0)
        {
            masterVoiceClipsList.Add(masterVoicelineClip1);
            masterVoiceClipsList.Add(masterVoicelineClip2);
            masterVoiceClipsList.Add(masterVoicelineClip3);
        }
        if (tankVoiceClipsList.Count == 0)
        {
            tankVoiceClipsList.Add(tankVoicelineClip1);
            tankVoiceClipsList.Add(tankVoicelineClip2);
            tankVoiceClipsList.Add(tankVoicelineClip3);
        }
        if (shooterVoiceClipsList.Count == 0)
        {
            shooterVoiceClipsList.Add(shooterVoicelineClip1);
            shooterVoiceClipsList.Add(shooterVoicelineClip2);
            shooterVoiceClipsList.Add(shooterVoicelineClip3);
        }
        if (runnerVoiceClipsList.Count == 0)
        {
            runnerVoiceClipsList.Add(runnerVoicelineClip1);
            runnerVoiceClipsList.Add(runnerVoicelineClip2);
            runnerVoiceClipsList.Add(runnerVoicelineClip3);
        }
        if (mechanicVoiceClipsList.Count == 0)
        {
            mechanicVoiceClipsList.Add(mechanicVoicelineClip1);
            mechanicVoiceClipsList.Add(mechanicVoicelineClip2);
            mechanicVoiceClipsList.Add(mechanicVoicelineClip3);
        }
        if (medicVoiceClipsList.Count == 0)
        {
            medicVoiceClipsList.Add(medicVoicelineClip1);
            medicVoiceClipsList.Add(medicVoicelineClip2);
            medicVoiceClipsList.Add(medicVoicelineClip3);
        }
    }

    #region UIAudio
    private void ButtonPressAudio()
    {
        fxSource.PlayOneShot(buttonPressClip);
    }
    #endregion

    #region MusicAndAtmo
    private void PlayMainTheme()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.clip = mainThemeClip;
            musicSource.loop = true;
            musicSource.volume = 0.3f;
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

    private void StopAtmo(PlayerType? winner, GameOverCondition endGameCondition)
    {
        StartCoroutine(FadeAudio(atmoSource, 2f, 0f));
    }
    #endregion

    private void ActionAudio(Action action)
    {
        if (action.PlayerActionType != null)
        {
            if (action.PlayerActionType == PlayerActionType.Refresh)
                PlayRefreshAudio();
        }

        if (action.ActionSteps == null)
            return;

        foreach (ActionStep actionStep in action.ActionSteps)
        {
            if (!actionStep.ActionFinished || actionStep.CharacterInAction == null)
                continue;

            switch (actionStep.ActionType)
            {
                case ActionType.Move:
                    PlayMoveAudio();
                    break;
                case ActionType.Attack:
                    PlayAttackAudio(actionStep.CharacterInAction);
                    break;
                case ActionType.ActiveAbility:
                    PlayActiveAbilityAudio(actionStep.CharacterInAction.ActiveAbility);
                    break;
            }
        }
    }

    private void PlayRefreshAudio()
    {
        PlayOneShot(fxSource, rechargeClip);
    }

    private void PlayMoveAudio()
    {
        int rnd;
        rnd = Random.Range(0, moveClipsList.Count);
        PlayOneShot(fxSource, moveClipsList[rnd]);
    }

    private void PlayAttackAudio(Character character)
    {
        if (character == null)
            return;

        var clip = character.CharacterType switch
        {
            CharacterType.TankChar => tankAttackClip,
            CharacterType.ShooterChar => shooterAttackClip,
            CharacterType.RunnerChar => runnerDoubleAttackClip,
            CharacterType.DocChar => medicAttackClip,
            CharacterType.MechanicChar => mechanicAttackClip,
            _ => null
        };

        PlayOneShot(fxSource, clip);
    }

    private void PlayActiveAbilityAudio(IActiveAbility ability)
    {
        var clip = ability switch
        {
            JumpAA => jumpClip,
            RamAA => blockClip,
            LongshotAA => powershotClip,
            RepairAA => changeFloorClip,
            TransfusionAA => healClip,
            HypnotizeAA => takeControlClip,
            _ => null
        };

        PlayOneShot(fxSource, clip);
    }

    #region GameplayAudio
    private void TurnChangeAudio(PlayerType player)
    {
        PlayOneShot(fxSource, turnChangeClip);
    }

    private void UnitDraftAudio(Character character)
    {
        PlayOneShot(fxSource, unitDraftedClip);
    }

    private void UnitPlacementAudio(Character character)
    {
        PlayOneShot(fxSource, unitPlacedClip);

        if (character.CharacterType == CharacterType.TankChar)
        {
            int rnd = Random.Range(0, tankVoiceClipsList.Count);
            PlayOneShot(voiceSource, tankVoiceClipsList[rnd]);
        }
        if (character.CharacterType == CharacterType.ShooterChar)
        {
            int rnd = Random.Range(0, shooterVoiceClipsList.Count);
            PlayOneShot(voiceSource, shooterVoiceClipsList[rnd]);
        }
        if (character.CharacterType == CharacterType.RunnerChar)
        {
            int rnd = Random.Range(0, runnerVoiceClipsList.Count);
            PlayOneShot(voiceSource, runnerVoiceClipsList[rnd]);
        }
        if (character.CharacterType == CharacterType.MechanicChar)
        {
            int rnd = Random.Range(0, mechanicVoiceClipsList.Count);
            PlayOneShot(voiceSource, mechanicVoiceClipsList[rnd]);
        }
        if (character.CharacterType == CharacterType.DocChar)
        {
            int rnd = Random.Range(0, medicVoiceClipsList.Count);
            PlayOneShot(voiceSource, medicVoiceClipsList[rnd]);
        }
    }

    private void MasterSpawnAudio()
    {
        int rnd = Random.Range(0, masterVoiceClipsList.Count);
        PlayOneShot(voiceSource, masterVoiceClipsList[rnd]);
    }

    private void LowTimeAudio()
    {
        PlayOneShot(fxSource, lastTenSeconds);
    }

    private void TimeoutAudio()
    {
        PlayOneShot(fxSource, timeRanOut);
    }
    #endregion

    private void PlayOneShot(AudioSource source, AudioClip clip)
    {
        if (GameplayManager.IsLoadingGame || clip == null || source == null)
            return;

        source.PlayOneShot(clip);
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
        GameEvents.OnGameStart += StopMainTheme;
        GameEvents.OnGameStart += PlayAtmo;
        AudioEvents.OnButtonPress += ButtonPressAudio;

        GameEvents.OnGameStart += SubscribeEventsOnDraftStart;
        GameEvents.OnGamePhaseStart += SubscribeGameplayEvents;
        AudioEvents.OnMainMenuEnter += UnsubscribeEventsOnReturnToMenu;
    }

    private void SubscribeEventsOnDraftStart()
    {
        if (!subscriptionsActive)
        {
            DraftEvents.OnCharacterCreated += UnitDraftAudio;
            PlacementEvents.OnPlaceCharacter += UnitPlacementAudio;
            GameplayEvents.OnPlayerTurnEnded += TurnChangeAudio;
            GameplayEvents.OnGameOver += StopAtmo;
            AudioEvents.OnSpawnMasters += MasterSpawnAudio;
            AudioEvents.OnTimeLow += LowTimeAudio;
            AudioEvents.OnTimeout += TimeoutAudio;
        }
    }

    private void SubscribeGameplayEvents(GamePhase gamePhase)
    {
        if (!subscriptionsActive && gamePhase == GamePhase.GAMEPLAY)
        {
            GameplayEvents.OnFinishAction += ActionAudio;
            subscriptionsActive = true;
        }
    }

    private void UnsubscribeEventsOnReturnToMenu()
    {
        GameplayEvents.OnFinishAction -= ActionAudio;
        DraftEvents.OnCharacterCreated -= UnitDraftAudio;
        PlacementEvents.OnPlaceCharacter -= UnitPlacementAudio;
        GameplayEvents.OnPlayerTurnEnded -= TurnChangeAudio;
        GameplayEvents.OnGameOver -= StopAtmo;
        AudioEvents.OnSpawnMasters -= MasterSpawnAudio;
        AudioEvents.OnTimeLow -= LowTimeAudio;
        AudioEvents.OnTimeout -= TimeoutAudio;
        subscriptionsActive = false;
    }

    private void UnsubscribeEvents()
    {
        AudioEvents.OnMainMenuEnter -= PlayMainTheme;
        GameEvents.OnGameStart -= StopMainTheme;
        GameEvents.OnGameStart -= PlayAtmo;
        AudioEvents.OnButtonPress -= ButtonPressAudio;

        GameEvents.OnGameStart -= SubscribeEventsOnDraftStart;
        GameEvents.OnGamePhaseStart -= SubscribeGameplayEvents;
        AudioEvents.OnMainMenuEnter -= UnsubscribeEventsOnReturnToMenu;
    }

    private void OnDestroy()
    {
        UnsubscribeEventsOnReturnToMenu();
        UnsubscribeEvents();
    }

    #endregion
}