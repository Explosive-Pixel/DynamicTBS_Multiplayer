using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    private GameObject audioManagerObject;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource atmoSource;
    [SerializeField] private AudioSource fxSource;

    private List<AudioClip> moveClipsList = new List<AudioClip>();
    private List<AudioClip> masterVoiceClipsList = new List<AudioClip>();
    private List<AudioClip> tankVoiceClipsList = new List<AudioClip>();
    private List<AudioClip> shooterVoiceClipsList = new List<AudioClip>();
    private List<AudioClip> runnerVoiceClipsList = new List<AudioClip>();
    private List<AudioClip> mechanicVoiceClipsList = new List<AudioClip>();
    private List<AudioClip> medicVoiceClipsList = new List<AudioClip>();

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
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip adrenalinClip;
    
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

    private void ActionAudio(ActionMetadata actionMetadata)
    {
        ActionType actionType = actionMetadata.ExecutedActionType;
        Character character = actionMetadata.CharacterInAction;

        if (character == null)
            return;

        if (actionType == ActionType.Move)
        {
            int rnd;
            rnd = Random.Range(0, moveClipsList.Count);
            fxSource.PlayOneShot(moveClipsList[rnd]);
        }
        if (actionType == ActionType.Attack)
        {
            if (character.GetCharacterType() == CharacterType.TankChar)
                fxSource.PlayOneShot(tankAttackClip);
            if (character.GetCharacterType() == CharacterType.ShooterChar)
                fxSource.PlayOneShot(shooterAttackClip);
            if (character.GetCharacterType() == CharacterType.RunnerChar)
                fxSource.PlayOneShot(runnerDoubleAttackClip);
            if (character.GetCharacterType() == CharacterType.MechanicChar)
                fxSource.PlayOneShot(mechanicAttackClip);
            if (character.GetCharacterType() == CharacterType.MedicChar)
                fxSource.PlayOneShot(medicAttackClip);
        }
        if (actionType == ActionType.ActiveAbility)
        {
            if (character.GetActiveAbility().GetType() == typeof(TakeControlAA))
                fxSource.PlayOneShot(takeControlClip);
            if (character.GetActiveAbility().GetType() == typeof(BlockAA))
                fxSource.PlayOneShot(blockClip);
            if (character.GetActiveAbility().GetType() == typeof(PowershotAA))
                fxSource.PlayOneShot(powershotClip);
            if (character.GetActiveAbility().GetType() == typeof(JumpAA))
                fxSource.PlayOneShot(jumpClip);
            if (character.GetActiveAbility().GetType() == typeof(ChangeFloorAA))
                fxSource.PlayOneShot(changeFloorClip);
            if (character.GetActiveAbility().GetType() == typeof(HealAA))
                fxSource.PlayOneShot(healClip);
        }
    }

    #region GameplayAudio
    private void TurnChangeAudio(Player player)
    {
        fxSource.PlayOneShot(turnChangeClip);
    }

    private void UnitDraftAudio(Character character)
    {
        fxSource.PlayOneShot(unitDraftedClip);
    }

    private void UnitPlacementAudio(Character character)
    {
        fxSource.PlayOneShot(unitPlacedClip);

        if (character.GetCharacterType() == CharacterType.TankChar)
        {
            int rnd = Random.Range(0, tankVoiceClipsList.Count);
            fxSource.PlayOneShot(tankVoiceClipsList[rnd]);
        }   
        if (character.GetCharacterType() == CharacterType.ShooterChar)
        {
            int rnd = Random.Range(0, shooterVoiceClipsList.Count);
            fxSource.PlayOneShot(shooterVoiceClipsList[rnd]);
        }
        if (character.GetCharacterType() == CharacterType.RunnerChar)
        {
            int rnd = Random.Range(0, runnerVoiceClipsList.Count);
            fxSource.PlayOneShot(runnerVoiceClipsList[rnd]);
        }
        if (character.GetCharacterType() == CharacterType.MechanicChar)
        {
            int rnd = Random.Range(0, mechanicVoiceClipsList.Count);
            fxSource.PlayOneShot(mechanicVoiceClipsList[rnd]);
        }
        if (character.GetCharacterType() == CharacterType.MedicChar)
        {
            int rnd = Random.Range(0, medicVoiceClipsList.Count);
            fxSource.PlayOneShot(medicVoiceClipsList[rnd]);
        }
    }
    #endregion

    #region AbilityAudio
    private void ExplosionAudio()
    {
        fxSource.PlayOneShot(explosionClip);
    }

    private void AdrenalinAudio()
    {
        fxSource.PlayOneShot(adrenalinClip);
    }
    #endregion

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
        GameplayEvents.OnGameplayPhaseStart += SubscribeEventsAfterPlacement;
        AudioEvents.OnMainMenuEnter += UnsubscribeEventsOnReturnToMenu;
    }

    private void SubscribeEventsOnDraftStart()
    {
        DraftEvents.OnCharacterCreated += UnitDraftAudio;
        PlacementEvents.OnPlaceCharacter += UnitPlacementAudio;
        GameplayEvents.OnPlayerTurnEnded += TurnChangeAudio;
        GameplayEvents.OnRestartGame += PlayAtmo;
        GameplayEvents.OnGameOver += StopAtmo;
    }

    private void SubscribeEventsAfterPlacement()
    {
        GameplayEvents.OnFinishAction += ActionAudio;
        AudioEvents.OnAdrenalin += AdrenalinAudio;
        AudioEvents.OnExplode += ExplosionAudio;
    }

    private void UnsubscribeEventsOnReturnToMenu()
    {
        GameplayEvents.OnFinishAction -= ActionAudio;
        AudioEvents.OnAdrenalin -= AdrenalinAudio;
        AudioEvents.OnExplode -= ExplosionAudio;
        DraftEvents.OnCharacterCreated -= UnitDraftAudio;
        PlacementEvents.OnPlaceCharacter -= UnitPlacementAudio;
        GameplayEvents.OnPlayerTurnEnded -= TurnChangeAudio;
        GameplayEvents.OnRestartGame -= PlayAtmo;
        GameplayEvents.OnGameOver -= StopAtmo;
    }

    private void UnsubscribeEvents()
    {
        AudioEvents.OnMainMenuEnter -= PlayMainTheme;
        GameEvents.OnGameStart -= StopMainTheme;
        GameEvents.OnGameStart -= PlayAtmo;
        AudioEvents.OnButtonPress -= ButtonPressAudio;

        GameEvents.OnGameStart -= SubscribeEventsOnDraftStart;
        GameplayEvents.OnGameplayPhaseStart -= SubscribeEventsAfterPlacement;
        AudioEvents.OnMainMenuEnter -= UnsubscribeEventsOnReturnToMenu;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion
}