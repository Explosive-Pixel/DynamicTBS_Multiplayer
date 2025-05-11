using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : IndicatorEffect
{
    [SerializeField] private GameObject attackerPrefab_blue;
    [SerializeField] private GameObject attackerPrefab_pink;
    [SerializeField] private List<GameObject> hitPrefabs;
    [SerializeField] private GameObject deathPrefab;

    [SerializeField] private float indicatorTime;

    private void Awake()
    {
        CharacterEvents.OnCharacterDeath += OnCharacterDeath;
        CharacterEvents.OnCharacterTakesDamage += OnCharacterTakesDamage;
        GameplayEvents.OnFinishAction += OnAttackCharacter;
    }

    private void OnAttackCharacter(ActionMetadata actionMetadata)
    {
        if (actionMetadata.ExecutedActionType == ActionType.Attack)
        {
            AnimateIndicator(actionMetadata.CharacterInAction.Side == PlayerType.blue ? attackerPrefab_blue : attackerPrefab_pink, actionMetadata.CharacterInitialPosition.Value, indicatorTime);
        }
    }

    private void OnCharacterTakesDamage(Character character, int damage)
    {
        if (!character.IsDead())
            AnimateIndicator(GetRandomPrefab(hitPrefabs), character.CurrentTile.gameObject.transform.position, indicatorTime);
    }

    private void OnCharacterDeath(Character character, Vector3 lastPosition)
    {
        AnimateIndicator(deathPrefab, lastPosition, indicatorTime);
    }

    private void OnDestroy()
    {
        CharacterEvents.OnCharacterDeath -= OnCharacterDeath;
        CharacterEvents.OnCharacterTakesDamage -= OnCharacterTakesDamage;
        GameplayEvents.OnFinishAction -= OnAttackCharacter;
    }
}
