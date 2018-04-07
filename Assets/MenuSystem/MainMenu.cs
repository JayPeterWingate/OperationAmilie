using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void NewGame()
    {
		GameObject.Find("LoadScreen").transform.GetChild(0).gameObject.SetActive(true);
		LevelData.LevelName = "MeleeTest";
		Application.LoadLevel("_Scene");
    }
    public void LoadGame()
    {
        
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
