using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamScript : MonoBehaviour
{
    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

    private readonly string startAchievement = "ACHIEVEMENT_START";

    private readonly Dictionary<CharacterType, string> characterAchievements = new()
    {
        { CharacterType.MechanicChar, "ACHIEVEMENT_MECHANIC" },
        { CharacterType.DocChar, "ACHIEVEMENT_DOC" },
        { CharacterType.RunnerChar, "ACHIEVEMENT_RUNNER" },
        { CharacterType.ShooterChar, "ACHIEVEMENT_SHOOTER" },
        { CharacterType.TankChar, "ACHIEVEMENT_TANK" },
    };

    private readonly Dictionary<ActiveAbilityType, string> abilityAchievements = new()
    {
        { ActiveAbilityType.HYPNOTIZE, "ACHIEVEMENT_HYPNOTIZE_ABILITY" },
        { ActiveAbilityType.JUMP, "ACHIEVEMENT_JUMP_ABILITY" },
        { ActiveAbilityType.RAM, "ACHIEVEMENT_RAM_ABILITY" },
        { ActiveAbilityType.REPAIR, "ACHIEVEMENT_CHANGEFLOOR_ABILITY" },
        { ActiveAbilityType.TRANSFUSION, "ACHIEVEMENT_TRANSFUSION_ABILITY" },
        { ActiveAbilityType.LONGSHOT, "ACHIEVEMENT_LONGSHOT_ABILITY" }
    };

    private readonly string timeoutAchievement = "ACHIEVEMENT_NOTIME";
    private readonly string winOnlineGameAchievement = "ACHIEVEMENT_CAPTAIN";
    private readonly string rematchAchievement = "ACHIEVEMENT_REMATCH";

    #region SingletonImplementation
    public static SteamScript Instance { set; get; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SubscribeEvents();
        }
    }
    #endregion

    private void Start()
    {
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            PlayerSetup.SetupName(name);
            Debug.Log(name);

            UnlockAchievement(startAchievement);
        }
    }

    private void UnlockAchievement(string achievementId)
    {
        if (!SteamManager.Initialized)
            return;

        if (!IsAchievementUnlocked(achievementId))
        {
            Debug.Log("Unlocked achievement " + achievementId);
            SteamUserStats.SetAchievement(achievementId);
            SteamUserStats.StoreStats();
        }
    }

    private bool IsAchievementUnlocked(string achievementId)
    {
        if (!SteamManager.Initialized)
            return false;

        SteamUserStats.GetAchievement(achievementId, out bool achieved);
        return achieved;
    }

    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        }
    }

    private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
    {
        if (pCallback.m_bActive != 0)
        {
            Debug.Log("Steam Overlay has been activated");
        }
        else
        {
            Debug.Log("Steam Overlay has been closed");
        }
    }

    private void TriggerAchievement(Character character)
    {
        if (characterAchievements.TryGetValue(character.CharacterType, out string achievementId))
            UnlockAchievement(achievementId);
    }

    private void TriggerAchievement(Action action)
    {
        if (action.ActionSteps != null && action.ActionSteps.Count > 0 && action.ActionSteps[0].ActionType == ActionType.ActiveAbility && action.ActionSteps[0].CharacterInAction != null)
        {
            if (abilityAchievements.TryGetValue(action.ActionSteps[0].CharacterInAction.ActiveAbility.AbilityType, out string achievementId))
            {
                UnlockAchievement(achievementId);
            }
        }
    }

    private void TriggerAchievement(GamePhase gamePhase, PlayerType currentPlayer)
    {
        if (GameManager.GameType == GameType.ONLINE && Client.Role == ClientType.PLAYER && PlayerSetup.IsPlayer && PlayerSetup.IsSide(currentPlayer))
        {
            UnlockAchievement(timeoutAchievement);
        }
    }

    private void TriggerAchievement(PlayerType? winner, GameOverCondition endGameCondition)
    {
        if (winner.HasValue && GameManager.GameType == GameType.ONLINE && Client.Role == ClientType.PLAYER && PlayerSetup.IsPlayer && PlayerSetup.IsSide(winner.Value))
        {
            UnlockAchievement(winOnlineGameAchievement);
        }
    }

    private void TriggerAchievement()
    {
        UnlockAchievement(rematchAchievement);
    }

    private void SubscribeEvents()
    {
        DraftEvents.OnCharacterCreated += TriggerAchievement;
        GameplayEvents.OnFinishAction += TriggerAchievement;
        GameplayEvents.OnTimerTimeout += TriggerAchievement;
        GameplayEvents.OnGameOver += TriggerAchievement;
        MenuEvents.OnRematchClicked += TriggerAchievement;
    }

    private void OnDestroy()
    {
        DraftEvents.OnCharacterCreated -= TriggerAchievement;
        GameplayEvents.OnFinishAction -= TriggerAchievement;
        GameplayEvents.OnTimerTimeout -= TriggerAchievement;
        GameplayEvents.OnGameOver -= TriggerAchievement;
        MenuEvents.OnRematchClicked -= TriggerAchievement;
    }
}
