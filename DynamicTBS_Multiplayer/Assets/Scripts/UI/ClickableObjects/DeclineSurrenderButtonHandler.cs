using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeclineSurrenderButtonHandler : MonoBehaviour, IClickableObject
{
    [SerializeField] private GameObject surrenderConfirmationBox;

    public void OnClick()
    {
        surrenderConfirmationBox.SetActive(false);
    }
}
