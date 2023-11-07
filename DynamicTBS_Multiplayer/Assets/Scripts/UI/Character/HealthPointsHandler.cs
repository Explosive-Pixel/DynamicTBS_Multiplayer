using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HealthPointsHandler : MonoBehaviour
{
    [SerializeField] private GameObject healthLeft;
    [SerializeField] private GameObject healthRight;
    [SerializeField] private GameObject healthMiddle;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float distance;

    private readonly List<GameObject> healthPoints = new();

    private int maxHP;

    public void UpdateHP(int hp, PlayerType side)
    {
        SetSide(side);

        int diff = maxHP - hp;

        while (diff > 0)
        {
            healthPoints[maxHP - diff - 1].GetComponent<HealthHandler>().SetActive(false, side);
            diff--;
        }
    }

    public void InitHealth(int maxHP, PlayerType side)
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
            Translate(healthLeft, -(distance * (maxHP - 1) / 2));
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

        SetSide(side);
    }

    private void SetSide(PlayerType side)
    {
        healthPoints.ForEach(hp => hp.GetComponent<HealthHandler>().SetActive(true, side));
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
