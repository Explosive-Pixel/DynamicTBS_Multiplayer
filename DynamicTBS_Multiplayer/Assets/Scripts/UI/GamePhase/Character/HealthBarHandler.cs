using System.Collections.Generic;
using UnityEngine;

public class HealthBarHandler : MonoBehaviour
{
    [SerializeField] private GameObject healthLeft;
    [SerializeField] private GameObject healthRight;
    [SerializeField] private GameObject healthMiddle;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float distance;

    private readonly List<GameObject> healthPoints = new();

    private bool init = false;
    private int maxHP;

    public void UpdateHP(int hp)
    {
        if (!init)
        {
            InitHealth(hp);
            init = true;
        }

        SetAllActive();

        int diff = maxHP - hp;

        while (diff > 0)
        {
            healthPoints[maxHP - diff].GetComponent<ActiveHandler>().SetActive(false);
            diff--;
        }
    }

    public void InitHealth(int maxHP)
    {
        this.maxHP = maxHP;

        if (maxHP == 1)
        {
            healthLeft.SetActive(false);
            healthRight.SetActive(false);
            healthMiddle.transform.position = startPosition;
            healthPoints.Add(healthMiddle);
        }
        else
        {
            healthLeft.transform.position = startPosition;

            if (maxHP % 2 == 0)
            {
                Translate(healthLeft, -(healthLeft.GetComponentInChildren<SpriteRenderer>().localBounds.size.x / 2));
            }

            Translate(healthLeft, -(distance * (maxHP - 2)));

            healthPoints.Add(healthLeft);

            if (maxHP == 2)
                healthMiddle.SetActive(false);

            for (int i = 1; i < maxHP - 1; i++)
            {
                GameObject midHP = i == 1 ? healthMiddle : Instantiate(healthMiddle);
                AppendHealthPoint(midHP);
            }

            AppendHealthPoint(healthRight);
        }
    }

    private void SetAllActive()
    {
        healthPoints.ForEach(hp => hp.GetComponent<ActiveHandler>().SetActive(true));
    }

    private void AppendHealthPoint(GameObject hp)
    {
        hp.transform.position = healthPoints[^1].transform.position;
        Translate(hp, distance);
        healthPoints.Add(hp);
    }

    private void Translate(GameObject go, float x)
    {
        go.transform.position = new Vector3(go.transform.position.x + x, go.transform.position.y, go.transform.position.z);
    }
}
