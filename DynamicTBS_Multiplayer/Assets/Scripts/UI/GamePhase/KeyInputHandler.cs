using UnityEngine;

public class KeyInputHandler : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.IsSpectator())
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PauseHandler.HandlePause();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            ActiveAbilityIconHandler.ExecuteActiveAbility();
        }
    }
}
