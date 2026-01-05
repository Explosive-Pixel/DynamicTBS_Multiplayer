using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealthBarHandler : MonoBehaviour
{
    [SerializeField] private GameObject healthLeftPrefab;
    [SerializeField] private GameObject healthRightPrefab;
    [SerializeField] private GameObject healthMiddlePrefab;

    [SerializeField] private GameObject startPosition;
    [SerializeField] private float distance;

    private readonly List<GameObject> healthPoints = new();

    public void UpdateHP(Character character)
    {
        ResetHealthPoints();

        if (character.HitPoints > 0)
        {
            InitHealth(character.HitPoints);
            SetAllActive();
            gameObject.GetComponentsInChildren<SideHandler>(true).ToList().ForEach(sideHandler => sideHandler.SetSide(character.Side));
        }
    }

    public void InitHealth(int maxHP)
    {
        if (maxHP == 0)
            return;

        if (maxHP == 1)
        {
            AppendHealthPoint(healthMiddlePrefab);
        }
        else
        {
            GameObject healthLeft = AppendHealthPoint(healthLeftPrefab);

            if (maxHP % 2 == 0)
            {
                Translate(healthLeft, healthLeft.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2);
            }

            Translate(healthLeft, -(distance * healthLeft.transform.lossyScale.x * (maxHP / 2)));

            for (int i = 1; i < maxHP - 1; i++)
            {
                AppendHealthPoint(healthMiddlePrefab);
            }

            AppendHealthPoint(healthRightPrefab);
        }
    }

    private void SetAllActive()
    {
        healthPoints.ForEach(hp => hp.GetComponent<ActiveHandler>().SetActive(true));
    }

    private GameObject AppendHealthPoint(GameObject hpPrefab)
    {
        GameObject hp = Instantiate(hpPrefab);
        hp.transform.SetParent(this.transform, false);
        hp.transform.localScale = hpPrefab.transform.localScale;

        if (healthPoints.Count == 0)
        {
            hp.transform.position = startPosition.transform.position;
        }
        else
        {
            hp.transform.position = healthPoints[^1].transform.position;
            Translate(hp, distance * hp.transform.lossyScale.x);
        }

        healthPoints.Add(hp);
        return hp;
    }

    private void ResetHealthPoints()
    {
        for (int i = 0; i < healthPoints.Count; i++)
        {
            Destroy(healthPoints[i]);
        }

        healthPoints.Clear();
    }

    private void Translate(GameObject go, float x)
    {
        go.transform.position = new Vector3(go.transform.position.x + x, go.transform.position.y, go.transform.position.z);
    }
}
