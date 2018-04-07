using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectPanelScript : MonoBehaviour {

	Vector2 clickedPosition;
	bool noUpdate = false;
	GameEvent currentEvent;
	GameObject currentEffectPanel;
	Dropdown effectDropDown, typeDropDown;
	int currentEffectIndex;
	// Use this for initialization
	void Start () {
		effectDropDown = transform.FindChild("EffectDropDown").GetComponent<Dropdown>();

		typeDropDown = transform.FindChild("TypeList").GetComponent<Dropdown>();

		typeDropDown.ClearOptions();
		List<string> options = new List<string>(Enum.GetNames(typeof(EffectTypes)));
		typeDropDown.AddOptions(options);
		


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void updateValues(ref GameEvent e)
	{
		currentEvent = e;

		effectDropDown.ClearOptions();

		List<string> options = new List<string>();
		// populate options
		for(int i = 0; i < currentEvent.getEffectCount(); i++)
		{
			options.Add(i.ToString());
		}
		//add options to dropdown
		effectDropDown.AddOptions(options);
		effectDropDown.RefreshShownValue();
		
		if(options.Count > 0)
		{
			showEffect(0);
			showValues(0);
		}

	}
	// FUNCTIONS RELATED TO THE SELECTION, CREATION AND DELETION OF EFFECTS
	public void showValues(int i)
	{
		currentEffectIndex = i;
		print(currentEvent);
		noUpdate = true;
		//print("T" + (int)currentEvent.getEffects()[i].getEffectType());
		typeDropDown.value = (int)currentEvent.getEffects()[i].getEffectType();
		typeDropDown.RefreshShownValue();

		showEffect((int)currentEvent.getEffects()[i].getEffectType());
	}
	public void newEffect()
	{
		currentEvent.addEffect(EffectTypes.noEffect);

		List<string> options = new List<string>();
		options.Add(effectDropDown.options.Count.ToString());
		effectDropDown.AddOptions(options);

		effectDropDown.value = effectDropDown.options.Count-1;
		effectDropDown.RefreshShownValue();

		if(effectDropDown.options.Count == 1)
		{
			showValues(0);
		}

		
	}
	public void removeEffect()
	{
		effectDropDown.options.RemoveAt(currentEvent.getEffectCount() - 1);

		currentEvent.removeEffect(currentEffectIndex);

		effectDropDown.value = 0;
		effectDropDown.RefreshShownValue();
	}
	// FUNCTIONS FOR THE UPDATE AND DISPLAY OF EFFECTS PANELS
	public void updateType(int i)
	{
		// If no update then don't update
		if(noUpdate)
		{
			noUpdate = false;
			return;
		}

		currentEvent.replaceEvent(currentEffectIndex, (EffectTypes)i);
		showEffect(i);
	}

	public void showEffect(int i)
	{
		if(currentEffectPanel != null)
			currentEffectPanel.SetActive(false);

		Vector2 pos;

		switch ((EffectTypes)i)
		{
			case EffectTypes.test:
				TestEffect effect = (TestEffect)currentEvent.getEffects()[currentEffectIndex];
				currentEffectPanel = transform.FindChild("testPanel").gameObject;
				currentEffectPanel.SetActive(true);

				currentEffectPanel.transform.FindChild("InputField").GetComponent<InputField>().text = "";
				print(effect.getData());
				currentEffectPanel.transform.FindChild("InputField").GetChild(1).GetComponent<Text>().text = effect.getData();
				break;

			case EffectTypes.moveUnit:
				MoveEffect moveEffect = (MoveEffect)currentEvent.getEffects()[currentEffectIndex];
				currentEffectPanel = transform.FindChild("moveUnitPanel").gameObject;
				currentEffectPanel.SetActive(true);

				currentEffectPanel.transform.FindChild("XField").GetComponent<InputField>().text = "";
				currentEffectPanel.transform.FindChild("YField").GetComponent<InputField>().text = "";

				pos = moveEffect.getPosition();
				currentEffectPanel.transform.FindChild("XField").GetChild(1).GetComponent<Text>().text = pos.x.ToString();
				currentEffectPanel.transform.FindChild("YField").GetChild(1).GetComponent<Text>().text = pos.y.ToString();

				break;
			case EffectTypes.enemySpawn:
				EnemyEffect enemyEffect = (EnemyEffect)currentEvent.getEffects()[currentEffectIndex];
				currentEffectPanel = transform.FindChild("EnemyPanel").gameObject;
				currentEffectPanel.SetActive(true);

				currentEffectPanel.transform.FindChild("XField").GetComponent<InputField>().text = "";
				currentEffectPanel.transform.FindChild("YField").GetComponent<InputField>().text = "";
				currentEffectPanel.transform.FindChild("UnitIndex").GetComponent<InputField>().text = "";

				pos = enemyEffect.getPosition();
				currentEffectPanel.transform.FindChild("XField").GetChild(1).GetComponent<Text>().text = pos.x.ToString();
				currentEffectPanel.transform.FindChild("YField").GetChild(1).GetComponent<Text>().text = pos.y.ToString();
				currentEffectPanel.transform.FindChild("UnitIndex").GetChild(1).GetComponent<Text>().text = enemyEffect.getIndex().ToString();
				break;
            case EffectTypes.enemyConversion:
                ConversationEffect conEffect = (ConversationEffect)currentEvent.getEffects()[currentEffectIndex];
                currentEffectPanel = transform.FindChild("conversationPanel").gameObject;
                currentEffectPanel.SetActive(true);

                currentEffectPanel.transform.FindChild("InputField").GetComponent<InputField>().text = "";
                print(conEffect.getData());
                currentEffectPanel.transform.FindChild("InputField").GetChild(1).GetComponent<Text>().text = conEffect.getData();
                break;
			case EffectTypes.noEffect:
				currentEffectPanel = null;
				break;

		}
			
	}

	// FUNCTIONS FOR INPUT FROM PANELS

	public void enterClickMode()
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<editorLoop>().setPanelIndex(3);
	}
	public void updateClickedPosition(Vector2 p)
	{
		clickedPosition = p;
		currentEffectPanel.transform.FindChild("XField").GetComponent<InputField>().text = p.x.ToString();
		currentEffectPanel.transform.FindChild("YField").GetComponent<InputField>().text = p.y.ToString();

	}

	public void updateTestData(string d)
	{
		if (d.Length > 0)
		{
			Effect e;
			currentEvent.getEffect(currentEffectIndex, out e);
			TestEffect t = (TestEffect)e;
			t.setData(d);

		}
	}
    public void updateConversationData(string d)
    {
        if (d.Length > 0)
        {
            Effect e;
            currentEvent.getEffect(currentEffectIndex, out e);
            ConversationEffect t = (ConversationEffect)e;
            t.setString(d);

        }
    }

	public void updateMoveX(string d)
	{
		if (d.Length > 0)
		{
			Effect e;
			currentEvent.getEffect(currentEffectIndex, out e);

			switch(e.getEffectType())
			{
				case EffectTypes.moveUnit:
					MoveEffect moveEffect = (MoveEffect)e;
					moveEffect.setX(int.Parse(d));
					break;
				case EffectTypes.enemySpawn:
					EnemyEffect enemyEffect = (EnemyEffect)e;
					enemyEffect.setX(int.Parse(d));
					break;
			}
			
			

		}
	}

	public void updateMoveY(string d)
	{
		if (d.Length > 0)
		{
			Effect e;
			currentEvent.getEffect(currentEffectIndex, out e);

			switch (e.getEffectType())
			{
				case EffectTypes.moveUnit:
					MoveEffect moveEffect = (MoveEffect)e;
					moveEffect.setY(int.Parse(d));
					break;
				case EffectTypes.enemySpawn:
					EnemyEffect enemyEffect = (EnemyEffect)e;
					enemyEffect.setY(int.Parse(d));
					break;
			}

		}
	}
	public void updateEnemyIndex(string d)
	{
		if (d.Length > 0)
		{
			Effect e;
			currentEvent.getEffect(currentEffectIndex, out e);
			EnemyEffect enemyEffect = (EnemyEffect)e;
			enemyEffect.setIndex(int.Parse(d));
		}
		

	}

}
