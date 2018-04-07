using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour {

	public static int levelId = 0;
	public static bool levelComplete = false;
	public static int progression = 3;
	public static int playerPosition = 0;

	public static int[] party = { 1, 1, 0, 0, 0, 0 };

	public static bool[] levelCompletion = { false, false, false, false, false };

	public static string LevelName = "RangedTest";

	public static bool newMap = false;
	public static int xSize,ySize;

	public static string getLevelPath()
	{
		return "Maps/" + LevelName;
	}
	public static string getSavePath()
	{
		return "Assets/Resources/Maps/" + LevelName+".txt";
	}
}
