using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownBarHandler : MonoBehaviour
{
    [SerializeField] private GameObject cooldown;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float distance;

    private readonly List<GameObject> cooldownPoints = new();

    private bool init = false;
    private int maxCooldown;

    public void UpdateCooldown(int cd)
    {
        if (!init)
        {
            InitCooldown(cd);
            init = true;
        }

        SetAllActive();

        int diff = maxCooldown - cd;

        while (diff > 0)
        {
            cooldownPoints[maxCooldown - diff - 1].GetComponent<ActiveHandler>().SetActive(false);
            diff--;
        }
    }

    private void InitCooldown(int maxCooldown)
    {
        this.maxCooldown = maxCooldown;

        cooldown.transform.position = startPosition;
        Translate(cooldown, -(distance * (maxCooldown - 1) / 2));
        cooldownPoints.Add(cooldown);

        for (int i = 1; i < maxCooldown - 1; i++)
        {
            AppendCooldownPoint(Instantiate(cooldown));
        }
    }

    private void SetAllActive()
    {
        cooldownPoints.ForEach(hp => hp.GetComponent<ActiveHandler>().SetActive(true));
    }

    private void AppendCooldownPoint(GameObject hp)
    {
        hp.transform.position = cooldownPoints[^1].transform.position;
        Translate(hp, distance);
        cooldownPoints.Add(hp);
    }

    private void Translate(GameObject go, float x)
    {
        go.transform.position = new Vector3(go.transform.position.x + x, go.transform.position.y, go.transform.position.z);
    }
}
