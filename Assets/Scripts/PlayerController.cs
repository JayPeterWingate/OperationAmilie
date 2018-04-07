using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Xml;

public class PlayerController : Controller {

	public GameObject gui;
	public GameObject eventSys;
	//public Gradient g;
	LineRenderer rangeRenderer;

	
	bool attackMode;
	GameObject selected;
	// Use this for initialization
	void Start () {
		controllerID = 0;
		currentTurn = true;
		gui.SendMessage("Hide");

		var newGameObject = new GameObject("Renderer");
		newGameObject.transform.parent = transform;

		rangeRenderer = newGameObject.AddComponent<LineRenderer>();
		rangeRenderer.colorGradient = new Gradient();
		rangeRenderer.material = new Material(Shader.Find("Sprites/Default"));
		rangeRenderer.startWidth = 0.2f;
		rangeRenderer.endWidth = 0.2f;
		//rangeRenderer.colorGradient;
	}
	public override void spawnTeam()
	{
		Vector2[] spawns = map.getSpawnPoints();



		XmlDocument xml = new XmlDocument();
		xml.LoadXml(playerData.text);

		XmlNode data = xml.SelectSingleNode("data");
		playerUnitList = new unitScript[data.ChildNodes.Count];

		for (int i = 0; i < data.ChildNodes.Count; i++)
		{
			if (LevelData.party[i] != 1)
			{
				playerUnitList[i] = null;
				continue;
			}
				
			XmlNode current = data.ChildNodes[i];
			string unitName = current.SelectSingleNode("name").InnerText;
			string unitClass = current.SelectSingleNode("class").InnerText;
			int health = int.Parse(current.SelectSingleNode("health").InnerText);
			int speed = int.Parse(current.SelectSingleNode("speed").InnerText);
			int skill = int.Parse(current.SelectSingleNode("skill").InnerText);
			int strength = int.Parse(current.SelectSingleNode("strength").InnerText);
			int smarts = int.Parse(current.SelectSingleNode("smarts").InnerText);
			string portraitString = current.SelectSingleNode("portait").InnerText;
			string source = current.SelectSingleNode("source").InnerText;
			GameObject newUnit = (GameObject)Instantiate(Resources.Load(source), transform, true);
			unitScript unit = newUnit.GetComponent<unitScript>();

			XmlNode weaponNode = current.SelectSingleNode("weapons");
			List<string> unitWeapons = new List<string>();
			foreach (XmlNode weapon in weaponNode.ChildNodes)
			{
				unitWeapons.Add(weapon.InnerText);
			}

			unit.setData(unitName, unitClass, health, speed, skill, strength, smarts, portraitString, unitWeapons);
			unit.gameObject.layer = 8;

			unit.setPosition(Mathf.FloorToInt(spawns[i].x), Mathf.FloorToInt(spawns[i].y));
			playerUnitList[i] = unit;
			unitsInGame.Add(unit);
			
		}
		gui.GetComponent<GUIController>().setPlayerTeam(playerUnitList);
	}
	// Update is called once per frame
	void Update()
	{
		if(map == null)
		{
			initiateController();
		}
			
		if(isTurn() == true)
		{
			if (Input.GetMouseButtonDown(0) && !eventSys.GetComponent<EventSystem>().IsPointerOverGameObject())
			{
				leftButtonPressed();
			}
			else if (Input.GetMouseButtonDown(1))
			{
				rightButtonPressed();
			}
			else
			{
				mouseOver();
			}
		}
		
		
		//If space pressed end turn
		if (Input.GetKeyDown(KeyCode.Space) && isTurn() == true)
		{
			endTurn();

		}
		

	}
	void leftButtonPressed()
	{
		RaycastHit hit;
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 8)))
		{

			DeselectCurrent();

			//set selected to hit 
			selected = hit.collider.gameObject;

			//Select new unit
			if (selected)
			{

				unitScript unit = selected.GetComponent<unitScript>();

				if (unit != null)
				{
                    if(unit.getOwner() == 0)
                    {
                        selectUnit(unit);
                    }
                    else
                    {
                        DeselectCurrent();
                    }
					
				}
                
			}

		}
	}

	void rightButtonPressed()
	{
		if (selected)
		{
			unitScript unit = selected.GetComponent<unitScript>();
			if (unit != null && unit.getOwner() == 0)
			{
				RaycastHit hit;
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 8)))
				{

					unitSelectedRightClick(hit, unit);
				}

			}
		}
	}
	void mouseOver()
	{
		//Mouse over tile while unit selected
		if (selected)
		{
			unitScript unit = selected.GetComponent<unitScript>();
			if (unit != null && unit.getOwner() == 0)
			{
				
				RaycastHit hit;
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 8)))
				{
					clickableTile tile = hit.collider.gameObject.GetComponent<clickableTile>();

					if (tile && !unit.hasMoved() && !attackMode)
					{
						GridItem tilePos = tile.GetComponent<GridItem>();
						//print(tilePos);
						if (pathFinder != null)
							pathFinder.drawPath(tilePos.getPos());
						else
							print("pathfinder dead");
					}

					unitScript enemyUnit = hit.collider.GetComponent<unitScript>();
					
					if(enemyUnit && attackMode == true && enemyUnit.getOwner() != 0 )
					{
						GridItem playerPos = unit.GetComponent<GridItem>();
						GridItem enemyPos = hit.collider.GetComponent<GridItem>();

						MeleeWeapon currentMeleeWeapon = null;
						if(unit.getCurrentWeapon().type == WeaponType.melee)
						{
							currentMeleeWeapon = (MeleeWeapon)unit.getCurrentWeapon();


							bool withinMeleeRange = currentMeleeWeapon != null && Vector2.Distance(enemyPos.getPos(), playerPos.getPos()) <= currentMeleeWeapon.range;
							if (withinMeleeRange || currentMeleeWeapon == null)
							{
								GUIController cont = gui.GetComponent<GUIController>();

								cont.setCombatPanelVisibility(true, unit, enemyUnit, meleeChanceToHit(unit, enemyUnit));
							}
						}
						else if(unit.getCurrentWeapon().type == WeaponType.ranged)
						{
							
							if (canSee(unit, enemyUnit))
							{
								rangeRenderer.enabled = true;
								updateRangefinder(unit, enemyUnit.GetComponent<GridItem>());
								GUIController cont = gui.GetComponent<GUIController>();
								float distance = Vector2.Distance(unit.GetComponent<GridItem>().getPos(), enemyUnit.GetComponent<GridItem>().getPos());

								cont.setCombatPanelVisibility(true, unit, enemyUnit, rangedChanceToHit(unit, enemyUnit));
								cont.loadRangedData(isShielded(unit,enemyUnit),getBaseHitChance(unit), getRangePenalty(unit, distance),getCoverPenalty(unit,enemyUnit,distance));

							}
							else if(rangeRenderer.enabled == true)
							{
								rangeRenderer.enabled = false;
							}
						}

						

					}
					else
					{
						GUIController cont = gui.GetComponent<GUIController>();

						cont.setCombatPanelVisibility(false);
						rangeRenderer.enabled = false;
					}

				}
				
			}
		}
        
        //We also want to set the enemyStatPanel
        
        RaycastHit result;
        Ray line = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(line, out result, Mathf.Infinity, (1 << 8)))
        {
            //set selected to hit 
            GameObject enemy = result.collider.gameObject;

            //Select new unit
            if (enemy)
            {
                var guiCont = gui.GetComponent<GUIController>();
                unitScript unit = enemy.GetComponent<unitScript>();

                if (unit != null && unit.getOwner() == 1)
                {
                    guiCont.setCharacterPanelVisibility(true,unit,false);
                }
                else
                {
                    guiCont.setCharacterPanelVisibility(false,null,false);
                }
            }

        }
    }
	
	public void selectUnit(unitScript unit)
	{
		GUIController cont = gui.GetComponent<GUIController>();

		DeselectCurrent();
		selected = unit.gameObject;

		cont.setCharacterPanelVisibility(true, unit);
		
        if(unit.getOwner() == 0)
        {
            unit.SendMessage("select");
            // IF unit has not moved
            if (unit != null && !unit.hasMoved())
            {
                // Allow them to

                if (unit.getOwner() == 0 && unit.hasMoved() == false)
                {
                    GridItem pos = selected.GetComponent<GridItem>();

                    pathFinder.getMovementPaths(pos.getPos(), unit.getMovementDistance(), true);
                    //map.toggleHighlight(true, Color.cyan, pos.getX(), pos.getY(), unit.getMovementDistance());
                }
            }
        }
		
	}
	void DeselectCurrent()
	{
		//Deselect current unit
		if (selected )
		{
			unitScript unit = selected.GetComponent<unitScript>();
			
			if (unit != null && unit.getOwner() == 0) 
			{
				gui.GetComponent<GUIController>().setCharacterPanelVisibility(false);
				gui.GetComponent<GUIController>().setCombatPanelVisibility(false);

				unit.SendMessage("deselect");
				
				if (unit.getOwner() == 0)
				{
					GridItem pos = selected.GetComponent<GridItem>();
					map.UnHilightMap();
					//map.toggleHighlight(false,Color.white, pos.getX(), pos.getY(), unit.getSpeed());
					pathFinder.setPathfinding(false);
					rangeRenderer.enabled = false;
					attackMode = false;
				}
			}

		}
		
		selected = null;
	}

	void unitSelectedRightClick(RaycastHit hit, unitScript unit)
	{
		clickableTile tile = hit.collider.gameObject.GetComponent<clickableTile>();
		if (tile)
		{
			GridItem tilePos = tile.GetComponent<GridItem>();
			GridItem pos = selected.GetComponent<GridItem>();

			if(!unit.hasMoved())
			{
				//draw path only if not in attack mode
				Vector2[] path = pathFinder.drawPath(tilePos.getPos(), !attackMode);


				if (path != null)
				{
					if (!attackMode && !unit.hasMoved())
					{
						map.UnHilightMap();
						//map.toggleHighlight(false, Color.white, pos.getX(), pos.getY(), unit.getSpeed());
						unit.setPath(path);
						pathFinder.setPathfinding(false);
					}

				}
			}
			if(attackMode && unit.getCurrentWeapon().type == WeaponType.shield)
			{
				activateShield(unit,tilePos);
			}
			
		}
		//If not tile may be unit
		unitScript clickedUnit = hit.collider.gameObject.GetComponent<unitScript>();
		// if is enemy unit
		if (attackMode && !unit.hasAttacked() && clickedUnit && clickedUnit.getOwner() != 0)
		{
			Attack(unit, clickedUnit);
		}
	}


	override public bool newTurn()
	{
		DeselectCurrent();
		return base.newTurn();
	}
	

	void AttackCommand()
	{
		if (attackMode)
		{
			attackMode = false;
			rangeRenderer.enabled = false;
			map.UnHilightMap();
			selectUnit(selected.GetComponent<unitScript>());
			
		}
		else
		{
			unitScript unit = selected.GetComponent<unitScript>();
			if (unit.hasAttacked())
			{
				return;
			}
			Weapon currentWeapon = unit.getCurrentWeapon();
			GridItem pos = selected.GetComponent<GridItem>();

			map.UnHilightMap();

			if (currentWeapon.type == WeaponType.melee)
			{
				MeleeWeapon currentMeleeWeapon = (MeleeWeapon)currentWeapon;
				map.toggleHighlight(true, Color.red, pos.getX(), pos.getY(), currentMeleeWeapon.range);

			}
			else if(currentWeapon.type == WeaponType.shield)
			{
				Shield currentShield = (Shield)currentWeapon;
				map.toggleHighlight(true, Color.yellow, pos.getX(), pos.getY(), currentShield.range);

			}

			attackMode = true;
			pathFinder.setPathfinding(false);
		}
	}
	protected override bool Attack(unitScript unit, unitScript clickedUnit)
	{
		bool value = base.Attack(unit, clickedUnit);

		if(value)
		{
			attackMode = false;
			rangeRenderer.enabled = false;
			gui.GetComponent<GUIController>().hideRangedPanel();
			map.UnHilightMap();
		}
		return value;
	}
	void DoorCommand()
	{
		foreach(GameObject door in selected.GetComponent<unitScript>().doorsWithinRange)
		{
			door.SendMessage("toggleDoor");
		}
		leftButtonPressed();
		
		
	}
	void saveTeam()
	{
		
	}

	void updateRangefinder(unitScript unit, GridItem position)
	{

		RangedWeapon weapon = (RangedWeapon)unit.getCurrentWeapon();

		// Set line data
		Vector3[] positions = new Vector3[4];
		positions[0] = new Vector3(unit.GetComponent<GridItem>().getX(), 1.0f, unit.GetComponent<GridItem>().getY());
		positions[3] = new Vector3(position.getX(), 1.0f, position.getY());

		float distance = Vector3.Distance(positions[0], positions[3]);

		positions[1] = Vector3.Lerp(positions[0], positions[3], Mathf.Max(0.001f,weapon.minRange / distance));
		positions[2] = Vector3.Lerp(positions[0], positions[3], Mathf.Min(0.9999f, (float)weapon.maxRange / distance));
		
		rangeRenderer.numPositions = 4;
		rangeRenderer.SetPositions(positions);



		GradientColorKey[] colorKey = new GradientColorKey[4];
		GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];

		//Set ends to red and red
		colorKey[2] = new GradientColorKey(new Color(1 ,0, 0), 0.0f);


		Color endColor;

		if (distance < weapon.minRange || distance > weapon.maxRange)
			endColor = new Color(1, 0, 0);
		else
			endColor = new Color(0, 0, 0);

		colorKey[3] = new GradientColorKey(endColor, 1.0f);

		//Set alphaKeys
		alphaKey[0] =  new GradientAlphaKey(1.0f, 0.0f);
		alphaKey[1] = new GradientAlphaKey(1.0f, 1.0f);
		//Set color key's two and three according to range

		GradientColorKey minKey, maxKey;

		if (distance > weapon.minRange)
			minKey = new GradientColorKey(new Color(0, 0, 0), (float)weapon.minRange / distance);
		else
			minKey = new GradientColorKey(new Color(1, 0, 0), 0.2f);

		colorKey[0] = minKey;

		if (distance > weapon.minRange)
			maxKey = new GradientColorKey(new Color(0, 0, 0), Mathf.Min(0.99999f, (float)weapon.maxRange / distance));
		else
			maxKey = new GradientColorKey(new Color(1, 0, 0), 0.5f);

		colorKey[1] = maxKey;

		Gradient grad = new Gradient();
		grad.mode = GradientMode.Blend;
		grad.SetKeys(colorKey, alphaKey);
		
		rangeRenderer.colorGradient = grad;
		

	}
}

