using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClickPermission
{
    ANY_CLIENT,
    ANY_PLAYER,
    CURRENT_PLAYER
}

public interface IClickableObject
{
    ClickPermission ClickPermission { get; }
    void OnClick();
}
