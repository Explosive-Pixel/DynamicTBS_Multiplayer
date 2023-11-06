using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ExplosionUIEffect : MonoBehaviour
{
    [SerializeField] private GameObject blueExplosionPrefab;
    [SerializeField] private GameObject pinkExplosionPrefab;
    [SerializeField] private GameObject parent;
    [SerializeField] private float explosionTime;
    [SerializeField] private float yOffset;

    private void Awake()
    {
        CharacterEvents.OnCharacterDeath += InstantiatePrefab;
    }

    private void InstantiatePrefab(Character character, Vector3 lastPosition)
    {
        if (character.PassiveAbility.GetType() == typeof(ExplodePA))
        {
            Vector3 instantiationPosition = lastPosition + new Vector3(0f, yOffset, 0f);
            Quaternion rotation = character.gameObject.transform.rotation;

            AnimateExplosion(character.Side == PlayerType.blue ? blueExplosionPrefab : pinkExplosionPrefab, instantiationPosition, rotation);
        }
    }

    private void AnimateExplosion(GameObject explosionPrefab, Vector3 instantiationPosition, Quaternion rotation)
    {
        GameObject explosionGameObject = Instantiate(explosionPrefab, instantiationPosition, rotation);
        explosionGameObject.transform.SetParent(parent.transform);
        StartCoroutine(ExplosionCoroutine(explosionGameObject, explosionTime));
    }

    private IEnumerator ExplosionCoroutine(GameObject explosionGameObject, float timerDuration)
    {
        float timerEndpoint = 0;

        while (timerDuration > timerEndpoint)
        {
            timerDuration -= Time.deltaTime;
            yield return null;
        }

        Destroy(explosionGameObject);
        yield break;
    }

    private void OnDestroy()
    {
        CharacterEvents.OnCharacterDeath -= InstantiatePrefab;
    }
}