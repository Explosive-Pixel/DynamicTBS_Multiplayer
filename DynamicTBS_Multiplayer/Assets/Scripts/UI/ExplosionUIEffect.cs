using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionUIEffect : MonoBehaviour
{
    [SerializeField] private GameObject blueExplosionPrefab;
    [SerializeField] private GameObject pinkExplosionPrefab;
    [SerializeField] private float explosionTime;
    [SerializeField] private float yOffset;

    private void Awake()
    {
        CharacterEvents.OnCharacterDeath += InstantiatePrefab;
    }

    private void InstantiatePrefab(Character character, Vector3 lastPosition)
    {
        Vector3 instantiationPosition = lastPosition + new Vector3(0f, yOffset, 0f);

        if (character.GetPassiveAbility().GetType() == typeof(ExplodePA))
        {
            if (character.GetSide().GetPlayerType() == PlayerType.blue)
            {
                GameObject newBlueExplosion = Instantiate(blueExplosionPrefab, instantiationPosition, character.GetCharacterGameObject().transform.rotation);
                StartCoroutine(ExplosionCoroutine(newBlueExplosion, explosionTime));
            }
            else
            {
                GameObject newPinkExplosion = Instantiate(pinkExplosionPrefab, instantiationPosition, character.GetCharacterGameObject().transform.rotation);
                StartCoroutine(ExplosionCoroutine(newPinkExplosion, explosionTime));
            }
        }
    }

    private IEnumerator ExplosionCoroutine(GameObject explosionPrefab, float timerDuration)
    {
        float timerEndpoint = 0;

        while (timerDuration > timerEndpoint)
        {
            timerDuration -= Time.deltaTime;
            yield return null;
        }

        Destroy(explosionPrefab);
        yield break;
    }

    private void OnDestroy()
    {
        CharacterEvents.OnCharacterDeath -= InstantiatePrefab;
    }
}