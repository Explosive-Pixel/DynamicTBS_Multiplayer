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
        OnMouseDown();
    }

    private void OnMouseDown()
    {
        if (!character.IsClickable)
            return;

        ActionHandler.Instance.ResetActionDestinations();

        if (GameManager.IsSpectator() || character.Side == PlayerManager.ExecutingPlayer)
            ActionHandler.Instance.InstantiateAllActionPositions(character);

        GameplayEvents.ChangeCharacterSelection(character);
    }
}
