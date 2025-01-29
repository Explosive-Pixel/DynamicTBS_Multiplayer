using UnityEngine;

public class UnselectCharacterHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        ActionUtils.ResetActionDestinations();
        GameplayEvents.ChangeCharacterSelection(null);
    }
}
