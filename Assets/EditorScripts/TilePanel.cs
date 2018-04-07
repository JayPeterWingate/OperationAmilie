using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum tilePanelType
{
	DisplayTiles,
	DisplayObjects,
	DisplayModifers,
	DisplayEnemies
};

public class TilePanel : MonoBehaviour {
	public tilePanelType type;
	string[] spriteList;
	
	public int index;
	public int startX  = 85,
		startY = 100, rowNumber, columnNumber;
	

	GameObject[] tileButtons;


	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

		TileMap tileMap = GameObject.FindGameObjectWithTag("GameController").GetComponent<TileMap>();

		if (spriteList == null)
		{
			
			if(type == tilePanelType.DisplayTiles)
			{
				
				TileType[] tileTypes;
				tileTypes = tileMap.getTiles();
				spriteList = new string[tileTypes.Length];
				for(int i = 0; i < tileTypes.Length; i++)
				{
					spriteList[i] = tileTypes[i].name;
				}

			}
			else if (type == tilePanelType.DisplayObjects)
			{
				TileType[] objectTypes;
				objectTypes = tileMap.getObjects();
				spriteList = new string[objectTypes.Length];
				for (int i = 0; i < objectTypes.Length; i++)
				{
					spriteList[i] = objectTypes[i].name;
				}
			}
			else if (type == tilePanelType.DisplayModifers)
			{
				TileType[] modiferTypes;
				modiferTypes = tileMap.getModifers();
				spriteList = new string[modiferTypes.Length];
				for (int i = 0; i < modiferTypes.Length; i++)
				{
					spriteList[i] = modiferTypes[i].name;
				}
			}
			else
			{
				EnemyType[] enemyTypes;
				enemyTypes = tileMap.getEnemyTypes();
				spriteList = new string[enemyTypes.Length];
				for (int i = 0; i < enemyTypes.Length; i++)
					spriteList[i] = enemyTypes[i].unitName;

			}
			RectTransform container = gameObject.GetComponent<RectTransform>();
			
			container.sizeDelta = new Vector2(container.sizeDelta.x + rowNumber*20, container.sizeDelta.y + (spriteList.Length)*40);
			container.transform.localPosition = new Vector2(-100, container.sizeDelta.y/2);
			tileButtons = new GameObject[spriteList.Length];
			
			for (int i = 0; i < spriteList.Length; i++)
			{
				tileButtons[i] = newButton(i,spriteList[i],spriteList.Length);
			}
		}
		if (tileMap.getTiles().Length > spriteList.Length)
		{
			
			
		}


	}
	GameObject newButton(int index, string text, int number)
	{
		int rowSize = 40;
		GameObject newTile = Instantiate(Resources.Load("imgSource/MenuTile")) as GameObject;
		newTile.transform.parent = transform;
		newTile.GetComponent<tileButton>().index = index;
		newTile.transform.localPosition = new Vector3(100, 0 - index*rowSize);
		newTile.transform.Find("Text").GetComponent<Text>().text = text;
		

		return newTile;
	}
	public int getIndex()
	{
		return index;
	}
}
