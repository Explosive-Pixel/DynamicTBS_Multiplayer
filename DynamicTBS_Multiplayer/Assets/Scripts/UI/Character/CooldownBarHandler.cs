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
            InitCooldown();
            init = true;
        }

        SetAllActive(false);

        for (int i = 0; i < Mathf.Min(cd, maxCooldown); i++)
        {
            cooldownPoints[i].GetComponent<ActiveHandler>().SetActive(true);
        }
    }

    private void InitCooldown()
    {
        maxCooldown = gameObject.GetComponentInParent<Character>().ActiveAbility.Cooldown;

        cooldown.transform.position = startPosition;
        Translate(cooldown, -(distance * (maxCooldown - 2)));
        if (maxCooldown % 2 == 0)
        {
            Translate(cooldown, -(cooldown.GetComponentInChildren<SpriteRenderer>().localBounds.size.x / 2));
        }
        cooldownPoints.Add(cooldown);

        for (int i = 1; i < maxCooldown; i++)
        {
            AppendCooldownPoint(Instantiate(cooldown));
        }
    }

    private void SetAllActive(bool active)
    {
        cooldownPoints.ForEach(hp => hp.GetComponent<ActiveHandler>().SetActive(active));
    }

    private void AppendCooldownPoint(GameObject hp)
    {
        hp.transform.parent = cooldown.transform.parent;
        hp.transform.position = cooldownPoints[^1].transform.position;
        Translate(hp, distance);
        cooldownPoints.Add(hp);
    }

    private void Translate(GameObject go, float x)
    {
        go.transform.position = new Vector3(go.transform.position.x + x, go.transform.position.y, go.transform.position.z);
    }
}
