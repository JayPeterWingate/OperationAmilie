using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameScript : MonoBehaviour {

	bool HumanVictory, AiVictory;
	TileMap map;
	public GameObject endPanel;
	Controller[] players;
	int currentTurnIndex;
	bool gameOver = false;
	// Use this for initialization
	void Start () {

		Controller.newGame();
		map = GetComponent<TileMap>();
		players = new Controller[2];
		players[0] = transform.Find("Player").GetComponent<PlayerController>();
		players[1] = transform.Find("AI").GetComponent<AIController>();
		endPanel.SetActive(false);
		currentTurnIndex = 0;

		
	}
	private void Update()
	{
		if(gameOver)
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{

				SceneManager.LoadScene("Overgame");
			}
		}
		else if(!players[currentTurnIndex].isTurn())
		{
			print("gameplay");
			currentTurnIndex = (currentTurnIndex+1) % players.Length;
			players[currentTurnIndex].newTurn();
		}
		else if (players[1].remainingAlive() == 0)
		{
			GameEnd(0);
		}
		else if(players[0].remainingAlive() == 0)
		{
			GameEnd(1);
		}
		
		
	}
	public void GameEnd(int winnerIndex)
	{
		gameOver = true;
		endPanel.SetActive(true);
		//Show win screen
		if (winnerIndex == 0)
		{
			LevelData.levelComplete = true;
			endPanel.transform.GetChild(1).GetComponent<Text>().text = "Victory";
		}
		else
		{
			LevelData.levelComplete = false;
			endPanel.transform.GetChild(1).GetComponent<Text>().text = "Defeat";
		}
			
	}

}
