using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanelScript : MonoBehaviour {
	
	int currentEventIndex;

	EventManager eventManager;
	Dropdown eventDropDown;
	positionPanelScript positionPanel;
	EffectPanelScript effectPanel;
	void Update()
	{
		if(eventManager == null)
		{
			TileMap tileMap = GameObject.FindGameObjectWithTag("GameController").GetComponent<TileMap>();
			eventManager = tileMap.eventManager;

			positionPanel = transform.FindChild("PositionPanel").GetComponent<positionPanelScript>();
			effectPanel = transform.FindChild("EffectPanel").GetComponent<EffectPanelScript>();

			eventDropDown = transform.FindChild("EventList").GetComponent<Dropdown>();
			eventDropDown.ClearOptions();
			foreach (GameEvent e in eventManager.getEvents())
			{
				List<string> options = new List<string>();
				options.Add(e.getIndex().ToString());
				eventDropDown.AddOptions(options);
			}
			eventDropDown.value = eventDropDown.options.Count - 1;
			eventDropDown.RefreshShownValue();
			setValues(eventDropDown.options.Count - 1);
		}
		

	}

	public void addNewEvent()
	{
		GameEvent newEvent = new GameEvent(eventManager.getEvents().Count);

		eventManager.addEvent(ref newEvent);
		

		List<string> options = new List<string>();
		options.Add(newEvent.getIndex().ToString());
		eventDropDown.AddOptions(options);
		
		eventDropDown.value = eventDropDown.options.Count-1;
		eventDropDown.RefreshShownValue();

		//First value appears not to refresh values....
		if (eventDropDown.options.Count == 1)
		{
			setValues(0);
		}
	}

	public void removeEvent()
	{
		eventDropDown.options.RemoveAt(eventManager.getEvents().Count - 1);
		eventDropDown.RefreshShownValue();
		eventManager.removeEvent(eventDropDown.value);

		eventDropDown.value = 0;
		eventDropDown.RefreshShownValue();

	}

	public void setValues(int i)
	{
		print(i);
		GameEvent e;
		eventManager.getEvent(out e, i);
		positionPanel.updateValues(ref e);
		effectPanel.updateValues(ref e);
	}

	public void getPositionIndex(out int eventIndex, out int positionIndex)
	{
		eventIndex = currentEventIndex;
		positionIndex = positionPanel.getIndex();

	}


}
