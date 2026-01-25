using UnityEngine;
using UnityEngine.Events;

public class NonUIButton : MonoBehaviour
{
    public UnityEvent OnButtonUsed = null;

    private void OnMouseDown()
    {
        AudioEvents.PressingButton();
        OnButtonUsed?.Invoke();
    }
}
