using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameRecorder : MonoBehaviour
{
    private void Awake()
    {
        SubscribeEvents();
    }

    private void RecordMove()
    {

    }

    #region EventsRegion

    private void SubscribeEvents()
    {

    }

    private void UnsubscribeEvents()
    {

    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}