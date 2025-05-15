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

        /*if (character.CurrentTile != null && character.CurrentTile.gameObject.TryGetComponent(out ActionTileHandler _))
        {
            return;
        }*/

        ActionUtils.ResetActionDestinations();

        if (GameManager.IsSpectator() || character.Side == PlayerManager.ExecutingPlayer)
            ActionUtils.InstantiateAllActionPositions(character);

        GameplayEvents.ChangeCharacterSelection(character);
    }
}
