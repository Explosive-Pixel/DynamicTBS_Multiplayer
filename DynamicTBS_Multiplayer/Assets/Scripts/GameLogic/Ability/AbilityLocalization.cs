using System.Collections;
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

    void Start()
    {
        activeAbilityEntries.ToList()
            .ForEach(entry =>
            {
                entry.localizedString.StringChanged += (value) =>
                {
                    activeAbilityLocalizedNames[entry.activeAbilityType] = value;
                };
                entry.localizedString.RefreshString();
            });
    }

    public static string GetActiveAbilityName(ActiveAbilityType activeAbilityType)
    {
        return activeAbilityLocalizedNames.GetValueOrDefault(activeAbilityType, "???");
    }
}
