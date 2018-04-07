using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EffectTypes
{
	noEffect,
	test,
	itemRecover,
	itemRecall,
	discussion,
	enemySpawn,
	enemyConversion,
	moveUnit
};



public abstract class Effect {

	EffectTypes type;
	
	public Effect(EffectTypes t)
	{
		type = t;
	}

	abstract public void takeEffect(GameObject unit);
	abstract public string getSaveString();
	public EffectTypes getEffectType() { return type; }

}

public class TestEffect : Effect
{
	string data;
	public TestEffect(string d):base(EffectTypes.test)
	{
		data = d;
	}

	public override void takeEffect(GameObject unit)
	{
		Debug.Log(data);
		GameObject.Find("GameScripts").GetComponent<GameScript>().GameEnd(0);
		
	}
	public void setData(string d)
	{
		data = d;
	}
	public string getData()
	{
		return data;
	}

	override public string getSaveString()
	{
		return (int)EffectTypes.test + "," + data;
	}

}
public class MoveEffect : Effect
{
	Vector2 position;
	public MoveEffect() : base(EffectTypes.moveUnit)
	{
		position = new Vector2();
	}
	public MoveEffect(Vector2 data) : base(EffectTypes.moveUnit)
	{
		position = data;
	}
	public override void takeEffect(GameObject unit)
	{
		// If no players on teleport square
		var units = Controller.getAllUnits();
		foreach(unitScript u in units)
		{
			if (u != null && u.GetComponent<GridItem>().getPos() == position)
				return;
		}
		//Teleport there
		unit.GetComponent<unitScript>().setPosition((int)position.x,(int)position.y);
		Vector3 originalPosition = GameObject.Find("camPosition").transform.position;
		GameObject.Find("camPosition").transform.position = new Vector3(unit.transform.position.x, originalPosition.y, unit.transform.position.z);
	}
	public Vector2 getPosition()
	{
		return position;
	}
	public void setX(int x)
	{
		position = new Vector2(x, position.y);
	}
	public void setY(int y)
	{
		position = new Vector2(position.x,y);
	}
	override public string getSaveString()
	{
		return (int)EffectTypes.moveUnit + "," + position.x + "," + position.y;
	}
}
public class ConversationEffect : Effect
{
    string conversationString;
    public ConversationEffect():base(EffectTypes.enemyConversion)
    {
        conversationString = "";
    }
    public ConversationEffect(string str):base(EffectTypes.enemyConversion)
    {
        conversationString = str;
    }
    public void setString(string s)
    {
        conversationString = s;
    }
    public override void takeEffect(GameObject unit)
    {
        Debug.Log(conversationString);
        GameObject.Find("DiscussionSystem").GetComponent<DiscussionSystem>().show(conversationString);
    }
    public string getData()
    {
        return conversationString;
    }
    override public string getSaveString()
    {
        return (int)EffectTypes.enemyConversion + "," + conversationString;
    }
}
public class EnemyEffect : Effect
{
	Vector2 position;
	int index;
	public EnemyEffect() : base(EffectTypes.enemySpawn)
	{
		position = new Vector2();
		index = 0;
	}
	public override void takeEffect(GameObject unit)
	{
		Debug.Log("Fired");
		AIController controller = GameObject.Find("AI").GetComponent<AIController>();
		controller.addUnit(index, (int)position.x, (int)position.y);
	}
	public Vector2 getPosition()
	{
		return position;
	}
	public int getIndex()
	{
		return index;
	}
	public void setIndex(int i)
	{
		index = i;
	}
	public void setX(int x)
	{
		position = new Vector2(x, position.y);
	}
	public void setY(int y)
	{
		position = new Vector2(position.x, y);
	}

	override public string getSaveString()
	{
		return (int)EffectTypes.enemySpawn + "," + index + "," + position.x + "," + position.y;
	}
}

public class NullEffect : Effect
{
	public NullEffect():base(EffectTypes.noEffect)
	{

	}
	public override void takeEffect(GameObject unit)
	{
		Debug.Log("nullEffectHasTriggered");

	}
	override public string getSaveString()
	{
		return ""+(int)EffectTypes.noEffect;
	}
}


public class GameEvent
{

	int index;
	bool triggered;
	bool reusable;
	List<Vector2> min;
	List<Vector2> max;

	List<Effect> effects; 

	public GameEvent(int i)
	{
		triggered = false;
		// Identifer
		index = i;
		//Trigger data
		min = new List<Vector2>();
		max = new List<Vector2>();
		min.Add(new Vector2(0, 0));
		max.Add(new Vector2(0, 0));

		//Effect data
		effects = new List<Effect>();
	}
	public GameEvent(int eventIndex, string saveString)
	{
		triggered = false;
		index = eventIndex;
		string[] saveStringSeperated = saveString.Split('|');
		string triggerData = saveStringSeperated[0];
		string reusableData = saveStringSeperated[1];
		string effectData = saveStringSeperated[2];

		//Add all triggers
		min = new List<Vector2>();
		max = new List<Vector2>();
		//Split at T
		string[] triggerStrings = triggerData.Split('T');
		
		for(int i = 1; i < triggerStrings.Length; i++)
		{
			string[] positionData = triggerStrings[i].Split(',');
			
			min.Add(new Vector2(int.Parse(positionData[0]), int.Parse(positionData[1])));
			max.Add(new Vector2(int.Parse(positionData[2]), int.Parse(positionData[3])));
		}
		//Check if reusable
		if (reusableData == "T")
			reusable = true;
		else
			reusable = false;

		//Add all effects
		effects = new List<Effect>();

		string[] effectStrings = effectData.Split('E');

		for(int i = 1; i < effectStrings.Length; i++)
		{
			string[] effectInfomation = effectStrings[i].Split(',');
			//bug.Log(effectInfomation);
			switch((EffectTypes)int.Parse(effectInfomation[0]))
			{
				case EffectTypes.test:
					Debug.Log(effectInfomation[1]);
					effects.Add(new TestEffect(effectInfomation[1]));
					break;
				case EffectTypes.moveUnit:
					effects.Add(new MoveEffect(new Vector2(int.Parse(effectInfomation[1]), int.Parse(effectInfomation[2]))));
					break;
			}
		}

	}

	public int getIndex()
	{
		return index;
	}
	public List<Vector2> getMinimums() { return min; }
	public List<Vector2> getMaximums() { return max; }
	public void setMinAt(int i, Vector2 v) { min[i] = v; }
	public void setMaxAt(int i, Vector2 v) { max[i] = v; }
	public void setReusable(bool b) { reusable = b; }
	public bool getResuable() { return reusable; }
	public void addTrigger(Vector2 minV, Vector2 maxV) { min.Add(minV); max.Add(maxV); }
	public void removeTrigger(int i) { min.RemoveAt(i); max.RemoveAt(i); }
	public int getEffectCount() { return effects.Count; }
	public List<Effect> getEffects() { return effects; }

	public void addEffect(EffectTypes type)
	{
				
		effects.Add(getEffectofType(type));
	}
	public Effect getEffectofType(EffectTypes type)
	{
		// This makes a new effect of specified type
		Effect e;
		switch (type)
		{
			case EffectTypes.test:
				e = new TestEffect("");
				break;
			case EffectTypes.moveUnit:
				e = new MoveEffect();
				break;
			case EffectTypes.enemySpawn:
				e = new EnemyEffect();
				break;
            case EffectTypes.enemyConversion:
                e = new ConversationEffect();
                break;
			default:
				e = new NullEffect();
				break;
		}
		return e;
	}
	public void removeEffect(int i)
	{
		effects.RemoveAt(i);
	}
	public void replaceEvent(int i, EffectTypes type)
	{
		effects[i] = getEffectofType(type);
	}
	public void getEffect(int i, out Effect e)
	{
		e = effects[i];
	}

	public string getSaveString()
	{
		string returnString = "";
		for(int i = 0; i < min.Count; i++)
		{
			returnString += "T" + min[i].x + ',' + min[i].y + ',' + max[i].x + ',' + max[i].y;
		}
		returnString += '|';
		if (reusable)
			returnString += 'T';
		else
			returnString += 'F';
		returnString += '|';
		for (int i = 0; i < getEffectCount(); i++)
		{
			returnString += "E"+effects[i].getSaveString();
		}

		return returnString;
	}
	public bool checkTriggers(Vector2 position)
	{
		if (triggered == true)
			return false;

		for (int i = 0; i < min.Count; i++)
		{
			if (min[i].x <= position.x && position.x <= max[i].x && min[i].y <= position.y && position.y <= max[i].y)
				return true;
		}
		return false;
	}
	public void triggerEvent(GameObject unit)
	{
		foreach(Effect e in effects)
		{
			e.takeEffect(unit);
		}
		if(!reusable)
			triggered = true;
	}
}
