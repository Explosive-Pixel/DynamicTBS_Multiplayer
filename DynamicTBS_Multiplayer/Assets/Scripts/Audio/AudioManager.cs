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

    #region ClipsRegion
    // Music & Atmo
    [SerializeField] private AudioClip mainThemeClip;
    [SerializeField] private AudioClip atmoClip;

    // Actions
    [SerializeField] private AudioClip buttonPressClip;
    [SerializeField] private AudioClip turnChangeClip;
    [SerializeField] private AudioClip unitDraftedClip;
    [SerializeField] private AudioClip unitPlacedClip;
    [SerializeField] private AudioClip moveClip;
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

    #region UIAudio
    private void ButtonPressAudio()
    {
        fxSource.PlayOneShot(buttonPressClip);
        Debug.Log("ButtonPress");
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
            fxSource.PlayOneShot(moveClip);
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

    private void UnitDraftAudio()
    {
        fxSource.PlayOneShot(unitDraftedClip);
    }

    private void UnitPlacementAudio(Character character)
    {
        fxSource.PlayOneShot(unitPlacedClip);

        if (character.GetCharacterType() == CharacterType.TankChar)
            fxSource.PlayOneShot(tankVoicelineClip);
        if (character.GetCharacterType() == CharacterType.ShooterChar)
            fxSource.PlayOneShot(shooterVoicelineClip);
        if (character.GetCharacterType() == CharacterType.RunnerChar)
            fxSource.PlayOneShot(runnerVoicelineClip);
        if (character.GetCharacterType() == CharacterType.MechanicChar)
            fxSource.PlayOneShot(mechanicVoicelineClip);
        if (character.GetCharacterType() == CharacterType.MedicChar)
            fxSource.PlayOneShot(medicVoicelineClip);
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
        GameplayEvents.OnPlayerTurnEnded += TurnChangeAudio;
        GameEvents.OnGameStart += StopMainTheme;
        GameEvents.OnGameStart += PlayAtmo;
        GameplayEvents.OnRestartGame += PlayAtmo;
        GameplayEvents.OnGameOver += StopAtmo;
        AudioEvents.OnButtonPress += ButtonPressAudio;
        AudioEvents.OnAdrenalin += AdrenalinAudio;
        AudioEvents.OnExplode += ExplosionAudio;
        GameplayEvents.OnFinishAction += ActionAudio;
        AudioEvents.OnUnitDrafted += UnitDraftAudio;
        PlacementEvents.OnPlaceCharacter += UnitPlacementAudio;
    }

    private void UnsubscribeEvents()
    {
        AudioEvents.OnMainMenuEnter -= PlayMainTheme;
        GameplayEvents.OnPlayerTurnEnded -= TurnChangeAudio;
        GameEvents.OnGameStart -= StopMainTheme;
        GameEvents.OnGameStart -= PlayAtmo;
        GameplayEvents.OnRestartGame -= PlayAtmo;
        GameplayEvents.OnGameOver -= StopAtmo;
        AudioEvents.OnButtonPress -= ButtonPressAudio;
        AudioEvents.OnAdrenalin -= AdrenalinAudio;
        AudioEvents.OnExplode -= ExplosionAudio;
        GameplayEvents.OnFinishAction -= ActionAudio;
        AudioEvents.OnUnitDrafted -= UnitDraftAudio;
        PlacementEvents.OnPlaceCharacter += UnitPlacementAudio;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion
}