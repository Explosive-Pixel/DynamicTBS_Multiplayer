using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class AIInputControler : MonoBehaviour
{
	public AIDifficultySO currentStrategy;
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

		ActionUtils.ExecuteAction(currentAction.Target);
		
	}
}
