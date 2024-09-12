using UnityEngine;

public class AIInputControler : MonoBehaviour
{
	private AIDifficultySO currentStrategy;
	private AIAction currentAction;

	private void Start()
	{
		//TODO: After selecting the difficulty in the main menu, load the corresponding SO
	}

	public void PlayMove()
	{
		GetBestMove();
	}

	private void GetBestMove()
	{
		currentAction = currentStrategy.CalculateBestMove();
		if (currentAction.Type == ActionType.Skip)
		{
			SkipAction.Execute();
		}
		else
		{

			if (currentAction.Type == ActionType.ActiveAbility)
			{
				currentAction.Character.ExecuteActiveAbility();
			}
			else
			{
				ActionUtils.InstantiateAllActionPositions(currentAction.Character);
				
				if (currentAction.Target != null)
				{
					ActionUtils.ExecuteAction(currentAction.Target);
				}
			}
		}
	}
}
