using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadOnClick : MonoBehaviour {
	public GameObject newLevelName;
	public GameObject newLevelX, newLevelY;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void newLevel()
	{
		LevelData.LevelName = newLevelName.GetComponent<UnityEngine.UI.Text>().text;
		LevelData.xSize = int.Parse(newLevelX.GetComponent<UnityEngine.UI.Text>().text);
		LevelData.ySize = int.Parse(newLevelY.GetComponent<UnityEngine.UI.Text>().text);
		LevelData.newMap = true;

		SceneManager.LoadScene("Editor");
	}
	public void LoadLevel()
	{
		LevelData.LevelName = newLevelName.GetComponent<UnityEngine.UI.Text>().text;
		TextAsset mapFile = Resources.Load(LevelData.getLevelPath()) as TextAsset;

		if(mapFile != null)
		{
			LevelData.newMap = false;
			SceneManager.LoadScene("Editor");
		}

	}
	public void PlayLevel()
	{
		LevelData.LevelName = newLevelName.GetComponent<UnityEngine.UI.Text>().text;
		TextAsset mapFile = Resources.Load(LevelData.getLevelPath()) as TextAsset;

		if (mapFile != null)
		{
			LevelData.newMap = false;
			SceneManager.LoadScene("_scene");
		}

	}
}
