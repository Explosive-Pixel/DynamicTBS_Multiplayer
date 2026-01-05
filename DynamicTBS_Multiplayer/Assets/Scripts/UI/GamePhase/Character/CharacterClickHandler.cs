using UnityEngine;

public class CharacterClickHandler : MonoBehaviour
{
    private Character character;

    private void Awake()
    {
        character = gameObject.GetComponent<Character>();
    }

    public void SelectCharacter()
    {
        if (!character.IsClickable)
            return;

        ActionHandler.Instance.ResetActionDestinations();

        if (GameManager.IsSpectator() || character.Side == PlayerManager.ExecutingPlayer)
            ActionHandler.Instance.InstantiateAllActionPositions(character);

        GameplayEvents.ChangeCharacterSelection(character);
    }

    private void OnMouseDown()
    {
        if (SceneChangeManager.Instance.CurrentScene == Scene.GAME && !GameplayManager.UIInteractionAllowed)
            return;

        SelectCharacter();
    }
}
