using UnityEngine;

public class UnselectCharacterHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        ActionHandler.Instance.ResetActions();
        GameplayEvents.ChangeCharacterSelection(null);
    }
}
