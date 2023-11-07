using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] private GameObject activePink;
    [SerializeField] private GameObject activeBlue;
    [SerializeField] private GameObject inactive;

    public void SetActive(bool active, PlayerType side)
    {
        activePink.SetActive(active && side == PlayerType.pink);
        activeBlue.SetActive(active && side == PlayerType.blue);
        inactive.SetActive(!active);
    }
}
