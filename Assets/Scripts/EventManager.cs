using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager {

	public List<GameEvent> levelEvents;

	public EventManager()
	{
		levelEvents = new List<GameEvent>();
	}
	public EventManager(string saveString)
	{
		levelEvents = new List<GameEvent>();

		string[] eventStrings = saveString.Split(';');

		for(int i = 0; i < eventStrings.Length-1; i++)
		{
			levelEvents.Add(new GameEvent(i,eventStrings[i]));
		}


	}
	public void activateTriggers(GameObject unit)
	{
		Vector2 unitPosition = unit.GetComponent<GridItem>().getPos();
		foreach(GameEvent e in levelEvents)
		{
			if(e.checkTriggers(unitPosition))
			{
				e.triggerEvent(unit);
			}
		}
	}
	public List<GameEvent> getEvents()
	{
		return levelEvents;
	}

	public void addEvent(ref GameEvent e)
	{
		levelEvents.Add(e);
	}

	public void removeEvent(int index)
	{
		levelEvents.RemoveAt(index);
	}

	public void getEvent(out GameEvent e, int i)
	{
		e = levelEvents[i];
	}
	public string getSaveString()
	{
		string returnString = "";

		for(int i = 0; i < levelEvents.Count; i++)
		{
			returnString += levelEvents[i].getSaveString() + ';';
		}

		return returnString;
	}
}
