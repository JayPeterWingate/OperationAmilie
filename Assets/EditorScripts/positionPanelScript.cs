using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class positionPanelScript : MonoBehaviour {

	//List<Vector2> currentMin, currentMax;
	GameEvent currentEvent;
	Dropdown posDropDown;
	TileMap tileMap;
	int currentIndex;
	Text xMin, xMax, yMin, yMax;
	InputField[] inputValues;
	private void Start()
	{
		tileMap = GameObject.FindGameObjectWithTag("GameController").GetComponent<TileMap>();
		posDropDown = transform.FindChild("PosIndex").GetComponent<Dropdown>();
		xMin = transform.FindChild("minXInput").GetChild(0).GetComponent<Text>();
		xMax = transform.FindChild("maxXInput").GetChild(0).GetComponent<Text>();
		yMin = transform.FindChild("minYInput").GetChild(0).GetComponent<Text>();
		yMax = transform.FindChild("maxYInput").GetChild(0).GetComponent<Text>();

		inputValues = new InputField[4];
		inputValues[0] = transform.FindChild("minXInput").GetComponent<InputField>();
		inputValues[1] = transform.FindChild("maxXInput").GetComponent<InputField>();
		inputValues[2] = transform.FindChild("minYInput").GetComponent<InputField>();
		inputValues[3] = transform.FindChild("maxYInput").GetComponent<InputField>();

	}

	public void updateValues(ref GameEvent e)
	{
		tileMap.UnHilightMap();
		currentEvent = e;
		posDropDown.ClearOptions();
		List<string> options = new List<string>();
		for(int i = 0; i < e.getMinimums().Count; i++)
		{
			options.Add(i.ToString());
		}
		posDropDown.AddOptions(options);

		posDropDown.value = 0;
		showValues(0);
		

	}
	public void showValues(int i)
	{
		unHighlightTrigger();
		currentIndex = i;

		foreach(InputField t in inputValues)
		{
			t.text = "";
		}

		xMin.text = currentEvent.getMinimums()[currentIndex].x.ToString();
		yMin.text = currentEvent.getMinimums()[currentIndex].y.ToString();
		xMax.text = currentEvent.getMaximums()[currentIndex].x.ToString();
		yMax.text = currentEvent.getMaximums()[currentIndex].y.ToString();

		transform.FindChild("reUse").GetComponent<Toggle>().isOn = currentEvent.getResuable();

		highlightTrigger();

		posDropDown.RefreshShownValue();
	}
	public void updateMinX(string i)
	{
		if (i.Length > 0)
		{
			unHighlightTrigger();
			currentEvent.setMinAt(currentIndex, new Vector2(int.Parse(i), currentEvent.getMinimums()[currentIndex].y));
			highlightTrigger();
		}
	}
	public void updateMinY(string i)
	{
		if (i.Length > 0)
		{
			unHighlightTrigger();
			currentEvent.setMinAt(currentIndex, new Vector2(currentEvent.getMinimums()[currentIndex].x, int.Parse(i)));
			highlightTrigger();
		}
	}
	public void updateMaxX(string i)
	{
		if (i.Length > 0)
		{
			unHighlightTrigger();
			currentEvent.setMaxAt(currentIndex, new Vector2(int.Parse(i), currentEvent.getMaximums()[currentIndex].y));
			highlightTrigger();
		}

	}
	public void updateMaxY(string i)
	{
		if (i.Length > 0)
		{
			unHighlightTrigger();
			currentEvent.setMaxAt(currentIndex, new Vector2(currentEvent.getMaximums()[currentIndex].x, int.Parse(i)));
			highlightTrigger();
		}
	}

	public void updateUsable(bool v)
	{
		currentEvent.setReusable(v);
	}

	public void setTrigger(Vector2 min, Vector2 max)
	{
		unHighlightTrigger();
		currentEvent.setMinAt(currentIndex, min);
		currentEvent.setMaxAt(currentIndex,max);
		highlightTrigger();
		showValues(currentIndex);
	}

	public void newPosition()
	{

		//Add index to options
		List<string> options = new List<string>();
		options.Add(currentEvent.getMaximums().Count.ToString());
		posDropDown.AddOptions(options);
		
		// make vectors
		Vector2 newMin = new Vector2();
		Vector2 newMax = new Vector2();

		//Add them to current list
		currentEvent.addTrigger(newMin, newMax);

		//Set dropdown to last value
		posDropDown.RefreshShownValue();
		posDropDown.value = posDropDown.options.Count - 1;
		showValues(currentEvent.getMaximums().Count-1);
		posDropDown.RefreshShownValue();

	}
	public void removeTrigger()
	{
		//Unhighlight area
		unHighlightTrigger();
		//Remove last element from dropdown
		posDropDown.options.RemoveAt(posDropDown.options.Count-1);
		posDropDown.RefreshShownValue();
		//Remove the trigger 
		currentEvent.removeTrigger(currentIndex);

		//Set the drop down to 0
		posDropDown.value = 0;
		posDropDown.RefreshShownValue();

		//Show the new values
		showValues(currentIndex);

	}

	public int getIndex()
	{
		return currentIndex;
	}

	public void highlightTrigger()
	{
		tileMap.HighlightBox(currentEvent.getMinimums()[currentIndex], currentEvent.getMaximums()[currentIndex]);
	}
	public void unHighlightTrigger()
	{
		tileMap.UnHilightBox(currentEvent.getMinimums()[currentIndex], currentEvent.getMaximums()[currentIndex]);
	}
}
