using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileButton : MonoBehaviour {

	TilePanel panel;
	public int index;
	// Use this for initialization
	void Start () {
		panel = transform.parent.GetComponent<TilePanel>();

	}
	public void pressed()
	{
		GameObject control = GameObject.FindGameObjectWithTag("GameController");
		control.GetComponent<editorLoop>().setIndex(panel.getIndex(),index);
		print(index);
	}

	public void exitPressed()
	{
		GameObject control = GameObject.FindGameObjectWithTag("GameController");
		control.GetComponent<editorLoop>().SendMessage("exit");
	}
}
