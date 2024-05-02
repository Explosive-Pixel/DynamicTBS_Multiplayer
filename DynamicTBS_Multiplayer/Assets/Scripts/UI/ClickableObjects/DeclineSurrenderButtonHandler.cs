using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeclineSurrenderButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject surrenderConfirmationBox;

    public void OnMouseDown()
    {
        if (GameManager.IsSpectator())
            return;

        surrenderConfirmationBox.SetActive(false);
    }
}
