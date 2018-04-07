using UnityEngine;
using System.Collections;
//using
using System.Xml;
using System.Collections.Generic;
using System.Linq;

public class TileMap : MonoBehaviour {

	public TextAsset tileXML;
	public TextAsset objectXML;
	public TextAsset modiferXML;
	public TextAsset enemyXML;
	TextAsset mapFile;

	public TileType[] tileTypes;
	public TileType[] objectTypes;
	public TileType[] modiferTypes;
	public EnemyType[] enemyTypes;
	public EventManager eventManager;

	public int xSize = 10;
	public int ySize = 10;

	public int[,] tiles;
	public Dictionary<Vector2, int> modifers;
	public Dictionary<Vector2, int[]> objects;
	Dictionary<Vector2, GameObject> objectVisuals;
	Dictionary<Vector2, GameObject> modiferVisual;

	GameObject[,] tileVisual;
	
	public Dictionary<Vector2, int> spawnData;
	Dictionary<Vector2, GameObject> spawnVisual;
	
	// Use this for initialization
	void Start() {

		string path = LevelData.getLevelPath();
		mapFile = Resources.Load(path) as TextAsset;
		
		initiliseMap(mapFile);

	}
	void initiliseMap(TextAsset map)
	{
		MapLoader.loadTiles(tileXML,out tileTypes);
		MapLoader.loadObjects(objectXML, out objectTypes);
		MapLoader.loadEnemyType(enemyXML, out enemyTypes);
		MapLoader.loadTiles(modiferXML, out modiferTypes);
		MapLoader.generateMap(map, out xSize, out ySize, out tiles, out objects, out modifers, out eventManager, out spawnData);
		GenerateMapVisuals();
	}
	

	public TileType[] getTiles()
	{
		return tileTypes;
	}
	public TileType[] getObjects()
	{
		return objectTypes;
	}
	public TileType[] getModifers()
	{
		return modiferTypes;
	}
	public EnemyType[] getEnemyTypes()
	{
		return enemyTypes;
	}
	public List<GameEvent> getEvents()
	{
		return  eventManager.getEvents();
	}
	// Update is called once per frame
	void Update() {
	}
    public void setTilePassable(int x, int y, bool value)
    {
        this.tileVisual[x, y].GetComponent<clickableTile>().isWalkable = value;
    }
	public int getTileIndex (int x, int y)
	{
		return tiles[x, y];
	}
	public void setTile(int x, int y, int rotation, int value)
	{

		Destroy(tileVisual[x, y]);

		tiles[x, y] = value;
		tileVisual[x, y] = createTile(x, y, rotation, 0.0f);

	}
	public void setObject(int x, int y, int rotation, int value)
	{
		Vector2 position = new Vector2(x, y);
		if (objectVisuals.ContainsKey(position))
		{
			//Destroy current visual
			Destroy(objectVisuals[position]);
			objectVisuals.Remove(position);
		}
		
		//set objects new value
		int[] data = { value, rotation };
		
		if(!objects.ContainsKey(position))
		objects.Add(position, data);
		else
		{
			if(value != 0)
			{
				objects[position][0] = value;
				objects[position][1] = rotation;
			}
			else // Value of 0 must remove the object
			{
				objects.Remove(position);
			}
		}


		//update tileVisual
		if(value != 0)
		{
			//tileVisual[x, y].GetComponent<clickableTile>().updateValues(objectTypes[value]);
			// add visual
			objectVisuals.Add(position, createObject(x, y, 1.0f));
			objectVisuals[position].GetComponent<clickableTile>().updateValues(objectTypes[value]);



            tileVisual[x, y].GetComponent<clickableTile>().updateValues(objectTypes[value]);
		}
		else // If removing object return the value to visual tile
		{
			tileVisual[x, y].GetComponent<clickableTile>().updateValues(tileTypes[tiles[x, y]]);
		}


	}
	public void setModifer(int x, int y, int value,bool showModifer = false)
	{
		Vector2 position = new Vector2(x, y);
		if (modiferVisual.ContainsKey(position))
		{
			//Destroy current visual
			Destroy(modiferVisual[position]);
			modiferVisual.Remove(position);
		}


		if (!modifers.ContainsKey(position))
		{
			modifers.Add(position, value);
		}
		else
		{
			if (value != 0)
			{
				modifers[position] = value;
			}
			else // Value of 0 must remove the object
			{
				modifers.Remove(position);
			}
		}


		//update tileVisual
		if (value != 0)
		{
			//tileVisual[x, y].GetComponent<clickableTile>().updateValues(objectTypes[value]);
			// add visual
			if(showModifer)
				modiferVisual.Add(position, createModiferVisual(x, y, 2.0f));
			tileVisual[x,y].GetComponent<clickableTile>().updateValues(modiferTypes[value]);
		}
		else // If removing object return the value to visual tile
		{
			tileVisual[x, y].GetComponent<clickableTile>().updateValues(tileTypes[tiles[x, y]]);
		}
	}
    
	public void setSpawner(int x, int y, int value)
	{
		Vector2 position = new Vector2(x, y);
		if (spawnVisual.ContainsKey(position))
		{
			//Destroy current visual
			Destroy(spawnVisual[position]);
			spawnVisual.Remove(position);
		}

		//set objects new value
		
		if (!spawnData.ContainsKey(position))
			spawnData.Add(position, value);
		else
		{
			if (value != 0)
			{
				spawnData[position] = value;
			}
			else // Value of 0 must remove the object
			{
				spawnData.Remove(position);
			}
		}


		//update spawnVisual
		if (value != 0 && gameObject.name == "EditorScripts")
		{
			//tileVisual[x, y].GetComponent<clickableTile>().updateValues(objectTypes[value]);
			// add visual
			spawnVisual.Add(position, createSpawnPlaceholder(x, y ));
			//spawnVisual[position].GetComponent<clickableTile>().updateValues(objectTypes[value]);
		}
		else // If removing object return the value to visual tile
			tileVisual[x, y].GetComponent<clickableTile>().updateValues(tileTypes[tiles[x, y]]);

	}
	public clickableTile getTileData(Vector2 v) { return tileVisual[(int)v.x, (int)v.y].GetComponent<clickableTile>(); } 
	void GenerateMapVisuals()
	{
		tileVisual = new GameObject[xSize, ySize];
		objectVisuals = new Dictionary<Vector2, GameObject>();
		modiferVisual = new Dictionary<Vector2, GameObject>();
		spawnVisual = new Dictionary<Vector2, GameObject>();
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				float yPos = tileTypes[tiles[x, y]].height * 5.0f;

				tileVisual[x, y] = createTile(x, y, 0, yPos);
				clickableTile tile = tileVisual[x, y].GetComponent<clickableTile>();
				tile.updateValues(tileTypes[tiles[x, y]]);

				if (objects.ContainsKey(new Vector2(x, y)))
				{
					//print("ADD OBJECT AT " + x + "," + y);
					int objectIndex = objects[new Vector2(x, y)][0];
					int rotation = objects[new Vector2(x, y)][1];
					setObject(x, y, rotation, objectIndex);
				}
				if(modifers.ContainsKey(new Vector2(x, y)) )
				{
					int index = modifers[new Vector2(x, y)];
					setModifer(x,y,index);
				}
				if(spawnData.ContainsKey(new Vector2(x,y)))
				{
					int spawnIndex = spawnData[new Vector2(x, y)];
					setSpawner(x, y, spawnIndex);
				}
			}
		}
	}
	GameObject createTile(int x, int y, int rotation, float zPos)
	{
		GameObject tile = (GameObject)Instantiate(tileTypes[tiles[x, y]].tileVisualPrefab as GameObject, new Vector3(0, rotation, 0), Quaternion.identity);
		tile.GetComponent<GridItem>().setPosition(x, y);
		tile.transform.position = new Vector3(x, zPos, y);
		tile.transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
		tile.transform.parent = transform;
		tile.layer = 8;
		return tile;
	}
	GameObject createObject(int x, int y, float zPos)
	{
		Vector2 objPosition = new Vector2(x, y);
		GameObject objectVisual = (GameObject)Instantiate(objectTypes[objects[objPosition][0]].tileVisualPrefab as GameObject, new Vector3(0, 0, 0), Quaternion.identity);

		objectVisual.GetComponent<GridItem>().setPosition(x, y);
		objectVisual.transform.parent = transform;
		objectVisual.transform.position = new Vector3(x, zPos, y);
		//print(objects[objPosition][0] + " " + objects[objPosition][1]);
		objectVisual.transform.rotation = Quaternion.Euler(new Vector3(0, objects[objPosition][1], 0));
		

		return objectVisual;
	}
	GameObject createModiferVisual(int x, int y, float zPos)
	{
		Vector2 objPosition = new Vector2(x, y);
		GameObject objectVisual = (GameObject)Instantiate(modiferTypes[modifers[objPosition]].tileVisualPrefab as GameObject, new Vector3(0, 0, 0), Quaternion.identity);

		objectVisual.GetComponent<GridItem>().setPosition(x, y);
		objectVisual.transform.parent = transform;
		objectVisual.transform.position = new Vector3(x, -2, y);
		objectVisual.transform.localScale = new Vector3(1, 2, 1);
		
		return objectVisual;
	}
	GameObject createSpawnPlaceholder(int x, int y)
	{
		Vector2 objPosition = new Vector2(x, y);
		GameObject spawnVisual = (GameObject)Instantiate(Resources.Load<GameObject>(enemyTypes[spawnData[objPosition]].source) as GameObject, new Vector3(0, 0, 0), Quaternion.identity);

		spawnVisual.GetComponent<GridItem>().setPosition(x, y);
		spawnVisual.transform.parent = transform;
		spawnVisual.transform.position = new Vector3(x, 1.0f, y);
		

		return spawnVisual;
	}
	void save()
	{
		MapLoader.saveMap(LevelData.getSavePath(), xSize, ySize, ref tiles, ref objects, ref modifers, ref spawnData, ref eventManager);
	}


	public void toggleHighlight(bool highlight, Color color, int x, int y, int size)
	{
		
		Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
		Dictionary<Vector2, int> distanceFromStart = new Dictionary<Vector2, int>();
		Queue frontier = new Queue();
		Vector2 start = new Vector2(x, y);

		cameFrom[start] = new Vector2(-1, -1);
		distanceFromStart[start] = 0;
		frontier.Enqueue(start);

		while (frontier.Count > 0)
		{
			Vector2 current = (Vector2)frontier.Dequeue();
			clickableTile currentTile = tileVisual[Mathf.FloorToInt(current.x), Mathf.FloorToInt(current.y)].GetComponent<clickableTile>();

			if (highlight)
			{
				currentTile.HighLight(color);
			}
			else
			{
				currentTile.unHilight();
			}

			foreach (Vector2 neibour in getNeibour(current))
			{

				if (cameFrom.ContainsKey(neibour) == false && distanceFromStart[current] + 1 <= size)
				{

					cameFrom[neibour] = current;
					distanceFromStart[neibour] = distanceFromStart[current] + 1;
					frontier.Enqueue(neibour);
				}
			}

		}
	}
	
	public Vector2[] getNeibour(Vector2 current)
	{
		List<Vector2> returnValues = new List<Vector2>();

		Vector2[] possibleValues = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };

		foreach (Vector2 possibileMove in possibleValues)
		{
			Vector2 newPos = current + possibileMove;
			if (0 <= newPos.x && newPos.x < xSize && 0 <= newPos.y && newPos.y < ySize)
			{
				TileType tileData = tileTypes[tiles[Mathf.FloorToInt(newPos.x), Mathf.FloorToInt(newPos.y)]];

				//if (tileData.isWalkable)
				bool tileWalkable = tileVisual[(int)newPos.x, (int)newPos.y].GetComponent<clickableTile>().isWalkable;


				bool objectWalkable = true;
				if (objectVisuals.ContainsKey(newPos))
					objectWalkable = objectVisuals[newPos].GetComponent<clickableTile>().isWalkable;

				if (tileWalkable && objectWalkable)
				{
					returnValues.Add(newPos);
				}
			}

		}



		return returnValues.ToArray();
	}

    public Vector2[] getAllNeighbours(Vector2 current)
    {
        List<Vector2> returnValues = new List<Vector2>{ new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) };
        return returnValues.Select(pos=>pos+current).Where(pos => pos.x >= 0 && pos.x < xSize && pos.y >= 0 && pos.y < ySize).ToArray();

    }
    public Vector2[] getSpawnPoints()
	{
		List<Vector2> spawnPoints = new List<Vector2>();

		foreach(Vector2 spawnLocations in spawnData.Keys)
		{
			if(spawnData[spawnLocations] == 1)
			{
				spawnPoints.Add(spawnLocations);
			}
		}

		return spawnPoints.ToArray();
	}
	public Dictionary<Vector2,int > getSpawnData()
	{
		for (int i = 0; i < 100; i++) { };
		return spawnData;
	}
	public void showModifers()
	{
		hideModifers();
		foreach (Vector2 position in modifers.Keys)
		{
			modiferVisual.Add(position, createModiferVisual((int)position.x, (int)position.y, 2.0f));
		}
	}
	public void hideModifers()
	{
		foreach (Vector2 position in modifers.Keys)
		{
			if (modiferVisual.ContainsKey(position))
			{
				//Destroy current visual
				Destroy(modiferVisual[position]);
				modiferVisual.Remove(position);
			}
			
		}
	}

	public void highlightArea(List<Vector2> highlightableTiles, Color colour)
	{
		foreach(Vector2 position in highlightableTiles)
		{
			tileVisual[(int)position.x, (int)position.y].GetComponent<clickableTile>().HighLight(colour);
		}
	}

	public void HighlightBox(Vector2 min, Vector2 max)
	{
		for (int i = (int)min.x; i <= (int)max.x; i++)
		{
			for(int j = (int)min.y; j <= (int)max.y; j++)
			{
				tileVisual[i, j].GetComponent<clickableTile>().HighLight(new Color(0,1,1));
			}
		}
	}
	public void UnHilightBox(Vector2 min, Vector2 max)
	{
		for (int i = (int)min.x; i <= (int)max.x; i++)
		{
			for (int j = (int)min.y; j <= (int)max.y; j++)
			{
				tileVisual[i, j].GetComponent<clickableTile>().unHilight();
			}
		}
	}
	public void UnHilightMap()
	{
		for (int i = 0; i <= xSize-1; i++)
		{
			for (int j = 0; j <= ySize-1; j++)
			{
				tileVisual[i, j].GetComponent<clickableTile>().unHilight();
			}
		}
	}
    public void displayDebugData(int i, int j, string value)
    {
        this.tileVisual[i, j].GetComponent<clickableTile>().showValue(value);
    }
	public void clearDisplay()
	{
		for(int i = 0; i < xSize; i++)
		{
			for(int j = 0; j < ySize; j++)
			{
				this.tileVisual[i, j].GetComponent<clickableTile>().hideValue();
			}
		}
	}
}
