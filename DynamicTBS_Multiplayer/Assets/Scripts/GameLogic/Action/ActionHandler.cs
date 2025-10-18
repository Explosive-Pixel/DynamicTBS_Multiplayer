using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    public static ActionHandler Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else
            Instance = this;
    }


}
