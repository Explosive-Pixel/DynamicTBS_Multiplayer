using UnityEngine;

public class CharmedState : MonoBehaviour, IState
{
    private Character character;

    public static CharmedState Create(GameObject parent, int charmDuration)
    {
        CharmedState cs = parent.AddComponent<CharmedState>();
        cs.Init(charmDuration);
        return cs;
    }

    private void Init(int charmDuration)
    {
        character = gameObject.GetComponent<Character>();
        SwapSide();

        RoundBasedCounter.Create(gameObject, charmDuration, Destroy);
    }

    private void SwapSide()
    {
        character.Side = PlayerManager.GetOtherSide(character.Side);
    }

    public void Destroy()
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        SwapSide();
    }
}