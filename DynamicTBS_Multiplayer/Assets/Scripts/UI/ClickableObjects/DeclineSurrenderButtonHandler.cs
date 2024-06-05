using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeclineSurrenderButtonHandler : MonoBehaviour, IClickableObject
{
    [SerializeField] private GameObject surrenderConfirmationBox;
    [SerializeField] private ClickPermission clickPermission;

    public ClickPermission ClickPermission { get { return clickPermission; } }

    public void OnClick()
    {
        surrenderConfirmationBox.SetActive(false);
    }
}
