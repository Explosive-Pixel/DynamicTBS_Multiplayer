using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using System.Linq;
using System;

public class AbilityLocalization : MonoBehaviour
{
    [System.Serializable]
    public class ActiveAbilityEntry
    {
        public ActiveAbilityType activeAbilityType;
        public LocalizedString localizedString;
    }

    [SerializeField]
    private ActiveAbilityEntry[] activeAbilityEntries;


    #region SingletonImplementation
    public static AbilityLocalization Instance { set; get; }

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
        }
    }
    #endregion

    private static readonly Dictionary<ActiveAbilityType, string> activeAbilityLocalizedNames = Enum.GetValues(typeof(ActiveAbilityType))
        .Cast<ActiveAbilityType>()
        .ToDictionary(a => a, a => a.Description());

    private readonly Dictionary<LocalizedString, LocalizedString.ChangeHandler> handlers
        = new();

    void Start()
    {
        foreach (var entry in activeAbilityEntries)
        {
            LocalizedString.ChangeHandler handler = (value) =>
            {
                activeAbilityLocalizedNames[entry.activeAbilityType] = value;
            };

            handlers[entry.localizedString] = handler;
            entry.localizedString.StringChanged += handler;
            entry.localizedString.RefreshString();
        }
    }

    public static string GetActiveAbilityName(ActiveAbilityType activeAbilityType)
    {
        return activeAbilityLocalizedNames.GetValueOrDefault(activeAbilityType, "???");
    }

    private void OnDestroy()
    {
        foreach (var kv in handlers)
        {
            kv.Key.StringChanged -= kv.Value;
        }

        handlers.Clear();
    }
}
