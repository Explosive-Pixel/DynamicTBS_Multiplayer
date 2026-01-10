using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NonUIButton : MonoBehaviour
{
    public UnityEvent OnButtonUsed = null;

    private void OnMouseDown()
    {
        OnButtonUsed?.Invoke();
    }
}
