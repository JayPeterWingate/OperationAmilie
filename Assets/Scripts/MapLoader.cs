using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public class MapLoader : MonoBehaviour {
	public static void loadEnemyType(TextAsset enemyXML, out EnemyType[] enemyTypes)
	{
		XmlDocument xml = new XmlDocument();
		xml.LoadXml(enemyXML.text);

		XmlNode data = xml.SelectSingleNode("data");
		EnemyType[] types = new EnemyType[data.ChildNodes.Count];

		for (int i = 0; i < data.ChildNodes.Count; i++)
		{
			types[i] = new EnemyType();
			XmlNode current = data.ChildNodes[i];
			types[i].unitName = current.SelectSingleNode("name").InnerText;
			types[i].unitClass = current.SelectSingleNode("class").InnerText;
			types[i].health = int.Parse(current.SelectSingleNode("health").InnerText);
			types[i].speed = int.Parse(current.SelectSingleNode("speed").InnerText);
			types[i].skill = int.Parse(current.SelectSingleNode("skill").InnerText);
			types[i].strength = int.Parse(current.SelectSingleNode("strength").InnerText);
			types[i].smarts = int.Parse(current.SelectSingleNode("smarts").InnerText);
			types[i].source = current.SelectSingleNode("source").InnerText;
			string imgPath = current.SelectSingleNode("image").InnerText;
			types[i].healthThreshold = int.Parse(current.SelectSingleNode("healthThreshold").InnerText);
			types[i].sightRange = int.Parse(current.SelectSingleNode("sightRange").InnerText);
			types[i].image = Resources.LoadAll<Sprite>(imgPath)[0];

			XmlNode weaponNode = current.SelectSingleNode("weapons");
			List<string> unitWeapons = new List<string>();
			foreach (XmlNode weapon in weaponNode.ChildNodes)
			{
				unitWeapons.Add(weapon.InnerText);
			}
			types[i].weapons = unitWeapons.ToArray();
		}
		enemyTypes = types;
	}
	public static void loadTiles(TextAsset tileXML, out TileType[] tileTypes)
	{
		XmlDocument xml = new XmlDocument();
		xml.LoadXml(tileXML.text);
		XmlNode data = xml.SelectSingleNode("data");
		tileTypes = new TileType[data.ChildNodes.Count];
		for (int i = 0; i < data.ChildNodes.Count; i++)
		{
			//For each child add to TileType
			XmlNode current = data.ChildNodes[i];
			tileTypes[i] = new TileType();
			tileTypes[i].name = current.SelectSingleNode("name").InnerText;
			string path = current.SelectSingleNode("prefabDir").InnerText;
			tileTypes[i].height = float.Parse(current.SelectSingleNode("height").InnerText);
			tileTypes[i].tileVisualPrefab = (GameObject)Resources.Load(path);
			tileTypes[i].isWalkable = current.SelectSingleNode("passable").InnerText.ToLower() == "true";
			tileTypes[i].cost = int.Parse(current.SelectSingleNode("movementCost").InnerText);
			path = current.SelectSingleNode("imgLocation").InnerText;
			//print(path);
			tileTypes[i].img = Resources.LoadAll<Sprite>(path)[0];
		}
	}
	public static void loadObjects(TextAsset objectXML, out TileType[] objectTypes)
	{
		XmlDocument xml = new XmlDocument();
		xml.LoadXml(objectXML.text);
		XmlNode data = xml.SelectSingleNode("Data");
		objectTypes = new TileType[data.ChildNodes.Count];
		for (int i = 0; i < data.ChildNodes.Count; i++)
		{
			//For each child add to TileType
			XmlNode current = data.ChildNodes[i];
			objectTypes[i] = new TileType();
			objectTypes[i].name = current.SelectSingleNode("name").InnerText;
			string path = current.SelectSingleNode("prefabDir").InnerText;
			objectTypes[i].tileVisualPrefab = (GameObject)Resources.Load(path);
			objectTypes[i].isWalkable = current.SelectSingleNode("passable").InnerText.ToLower() == "true";
			objectTypes[i].cost = int.Parse(current.SelectSingleNode("movementCost").InnerText);
			objectTypes[i].coverValue = int.Parse(current.SelectSingleNode("coverValue").InnerText);
			path = current.SelectSingleNode("imgLocation").InnerText;
			objectTypes[i].img = Resources.LoadAll<Sprite>(path)[0];
		}
	}
	public static void generateMap(TextAsset file,out int xSize, out int ySize, out int[,] tiles, out Dictionary<Vector2, int[]> objects,out Dictionary<Vector2, int> modifiers , out EventManager eventManager, out Dictionary<Vector2,int> spawnPoints)
	{
		if(file == null)
		{
			spawnMap(out xSize, out ySize, out tiles, out objects,out modifiers, out eventManager, out spawnPoints);
		}
		else
		{
			loadMap(file, out xSize, out ySize, out tiles, out objects,out modifiers, out eventManager, out spawnPoints);

		}
	}
	static void spawnMap(out int xSize, out int ySize, out int[,] tiles, out Dictionary<Vector2, int[]> objects, out Dictionary<Vector2, int> modifiers, out EventManager eventManager, out Dictionary<Vector2,int> spawnPoints)
	{
		xSize = LevelData.xSize;
		ySize = LevelData.ySize;

		tiles = new int[xSize, ySize];
		for(int i = 0; i < xSize; i++)
		{
			for(int j = 0; j < ySize; j++)
			{
				tiles[i,j] = 2;
			}

		}
		objects = new Dictionary<Vector2, int[]>();
		eventManager = new EventManager();
		modifiers = new Dictionary<Vector2, int>();
		spawnPoints = new Dictionary<Vector2,int>();

	}
	static void loadMap(TextAsset file, out int xSize, out int ySize, out int[,] tiles, out Dictionary<Vector2, int[]> objects, out Dictionary<Vector2, int> modifiers, out EventManager eventManager, out Dictionary<Vector2, int> spawnPoints)
	{
		string text = file.text;
		string[] arrayString = text.Split('\n');

		xSize = int.Parse(arrayString[0]);
		ySize = int.Parse(arrayString[1]);

		tiles = new int[xSize, ySize];
		objects = new Dictionary<Vector2, int[]>();
		string data = arrayString[2];
		for (int i = 0; i < xSize; i++)
		{
			for (int j = 0; j < ySize * 4; j += 4)
			{
				tiles[i, j / 4] = int.Parse(data.Substring(i * (ySize * 4) + 1 + j, 3));
			}
		}
		data = arrayString[3];
		// iterate through the second line for objects
		string[] objectData = new string[4];
		int counter = 0;
		foreach (char c in data)
		{
			if (c == ';')
			{
				int x = int.Parse(objectData[0]);
				int y = int.Parse(objectData[1]);

				int value = int.Parse(objectData[2]);
				int rotation = int.Parse(objectData[3]);

				//print(rotation);

				int[] dataValue = { value, rotation };

				objects.Add(new Vector2(x, y), dataValue);
				objectData = new string[4];
				counter = 0;
			}
			else if (c == ',')
			{
				counter++;
			}
			else
			{
				objectData[counter] += c;
			}
		}
		// Modifers
		data = arrayString[4];
		modifiers = new Dictionary<Vector2, int>();
		string[] modiferData = new string[3];
		counter = 0;
		foreach (char c in data)
		{
			if (c == ';')
			{
				int x = int.Parse(modiferData[0]);
				int y = int.Parse(modiferData[1]);

				int value = int.Parse(modiferData[2]);
				
				//print(rotation);

				int dataValue = value;

				modifiers.Add(new Vector2(x, y), dataValue);
				modiferData = new string[3];
				counter = 0;
			}
			else if (c == ',')
			{
				counter++;
			}
			else
			{
				modiferData[counter] += c;
			}
		}


		//Triggers are now line 4
		data = arrayString[5];
		eventManager = new EventManager(data);

		//Lighting data
		data = arrayString[6];
		float[] dataArray = new float[7];
		string currentString = "";
		int index = 0;
		for (int i = 0; i < data.Length; i++)
		{
			if (data[i] == ',')
			{
				//print(currentString);
				dataArray[index] = float.Parse(currentString);
				index++;
				currentString = "";
			}
			else
			{
				currentString += data[i];
			}
		}


		Transform light = GameObject.Find("Directional Light").transform;
		Light worldLight = light.GetComponent<Light>();

		if (dataArray[3] == 0)
		{
			light.gameObject.SetActive(false);
		}

		light.rotation = Quaternion.Euler(new Vector3(dataArray[0], dataArray[1], dataArray[2]));
		worldLight.intensity = dataArray[3];

		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor = new Color(dataArray[4], dataArray[5], dataArray[6]);

		//Spawns
		data = arrayString[7];
		Dictionary<Vector2, int> spawns = new Dictionary<Vector2, int>();

		string[] spawnData = data.Split(';');

		foreach (string spawn in spawnData)
		{
			if(spawn.Length != 0)
			{
				string[] spawnValues = spawn.Split(',');
				Vector2 location = new Vector2(int.Parse(spawnValues[0]), int.Parse(spawnValues[1]));
				spawns.Add(location, int.Parse(spawnValues[2]));
			}
		}
		spawnPoints = spawns;
	}

	public static void saveMap(string path, int xSize, int ySize,ref int[,] tiles, ref Dictionary<Vector2, int[]> objects, ref Dictionary<Vector2,int> modifers, ref Dictionary<Vector2,int> spawnData,ref EventManager events)
	{
		string saveData = "";

		//Add sizes
		saveData += xSize.ToString() + "\n" + ySize.ToString() + "\n";

		//Add tilemap
		for (int i = 0; i < xSize; i++)
		{
			for (int j = 0; j < ySize; j++)
			{
				saveData += tiles[i,j].ToString().PadLeft(4, '0');
			}
		}
		saveData += '\n';

		//Add objects
		foreach (Vector2 key in objects.Keys)
		{
			int[] data = objects[key];
			print(data[1]);
			saveData += key.x.ToString() + ',' + key.y.ToString() + ',' + data[0] + ',' + data[1] + ';';
		}
		saveData += '\n';
		//Add Modifers
		foreach(Vector2 key in modifers.Keys)
		{
			int data = modifers[key];
			saveData += key.x.ToString() + ',' + key.y.ToString() +','+ data +";";
		}
		saveData += '\n';

		//Add events
		saveData += events.getSaveString() + "\n";

		//Add lightingData
		if (GameObject.Find("Directional Light") != null)
		{
			Transform light = GameObject.Find("Directional Light").transform;
			Light worldLight = light.GetComponent<Light>();
			saveData += light.rotation.eulerAngles.x + "," + light.rotation.eulerAngles.y + "," + light.rotation.eulerAngles.z + "," + worldLight.intensity;
		}
		else
		{
			saveData += 0 + "," + 0 + "," + 0 + "," + 0;
		}
		Color bg = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor;
		saveData +=  "," + bg.r + "," + bg.g + "," + bg.b + ",\n";


		//Add Spawns
		foreach (Vector2 spawnLocation in spawnData.Keys)
		{
			saveData +=  spawnLocation.x.ToString() + ',' + spawnLocation.y.ToString() + ',' + spawnData[spawnLocation] + ';';
		}

		
		System.IO.File.WriteAllText(LevelData.getSavePath(), saveData);
		print("SAVE");
	}
}
