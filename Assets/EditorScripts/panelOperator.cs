using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panelOperator : MonoBehaviour {

	GameObject tilePanel, objectPanel, eventPanel,spawnPanel, modiferPanel;

	void Start()
	{
		tilePanel = transform.FindChild("Tile Panel").gameObject;
		objectPanel = transform.FindChild("Object Panel").gameObject;
		eventPanel = transform.FindChild("Event Panel").gameObject;
		spawnPanel = transform.FindChild("Spawn Panel").gameObject;
		modiferPanel = transform.FindChild("Modifer Panel").gameObject; 
	}

	public void showTilePanel()
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<editorLoop>().setPanelIndex(0);
		tilePanel.SetActive(true);
		objectPanel.SetActive(false);
		eventPanel.SetActive(false);
		spawnPanel.SetActive(false);
		modiferPanel.SetActive(false);
	}
	public void showObjectPanel()
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<editorLoop>().setPanelIndex(1);
		tilePanel.SetActive(false);
		objectPanel.SetActive(true);
		eventPanel.SetActive(false);
		spawnPanel.SetActive(false);
		modiferPanel.SetActive(false);
	}
	public void showEventPanel()
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<editorLoop>().setPanelIndex(2);
		tilePanel.SetActive(false);
		objectPanel.SetActive(false);
		eventPanel.SetActive(true);
		spawnPanel.SetActive(false);
		modiferPanel.SetActive(false);
	}
	public void showSpawnPanel()
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<editorLoop>().setPanelIndex(4);
		tilePanel.SetActive(false);
		objectPanel.SetActive(false);
		eventPanel.SetActive(false);
		spawnPanel.SetActive(true);
		modiferPanel.SetActive(false);
	}
	public void showModiferPanel()
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<editorLoop>().setPanelIndex(5);
		tilePanel.SetActive(false);
		objectPanel.SetActive(false);
		eventPanel.SetActive(false);
		spawnPanel.SetActive(false);
		modiferPanel.SetActive(true);
	}
}
