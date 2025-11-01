using System;

[Serializable]
public class ActionStepDto
{
    public int characterType;
    public string characterInitialTileName;
    public float characterX;
    public float characterY;
    public ActionType actionType;
    public string actionDestinationTileName;
    public float actionDestinationX;
    public float actionDestinationY;
}
