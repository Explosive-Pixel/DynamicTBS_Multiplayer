using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    private GameObject audioManagerObject;
    [SerializeField] private AudioSource audioSource;

    #region ClipsRegion
    // Music
    [SerializeField] private AudioClip mainThemeClip;
    [SerializeField] private AudioClip fightThemeClip;

    // Actions
    [SerializeField] private AudioClip buttonPressClip;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip doubleAttackClip;
    [SerializeField] private AudioClip takeDamageClip;
    [SerializeField] private AudioClip moveClip;
    [SerializeField] private AudioClip takeControlClip;
    [SerializeField] private AudioClip blockClip;
    [SerializeField] private AudioClip powershotClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip changeFloorToHoleClip;
    [SerializeField] private AudioClip changeHoleToFloorClip;
    [SerializeField] private AudioClip healClip;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip adrenalinClip;
    [SerializeField] private AudioClip unitDraftedClip;
    [SerializeField] private AudioClip unitPlacedClip;

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
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        audioManagerObject = this.gameObject;
        DontDestroyOnLoad(audioManagerObject);
        
        if (!audioSource.isPlaying)
        {
            audioSource.clip = mainThemeClip;
            audioSource.loop = true;
            audioSource.volume = 0.2f;
            audioSource.Play();
            Debug.Log(audioSource.isPlaying);
        }
    }
}