using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : Controller {

	int currentIndex = 0;
	// Use this for initialization
	void Start () {

	}
	public override void spawnTeam()
	{
		controllerID = 1;
		List<unitScript> team = new List<unitScript>();
		EnemyType[] types = map.getEnemyTypes();
		Dictionary<Vector2, int> spawnData = map.getSpawnData();

		foreach(Vector2 position in spawnData.Keys)
		{
			if(spawnData[position] >= 2) // If greater than 2 then spawn enemy 
			{
				EnemyType type = types[spawnData[position]];

				GameObject newUnit = (GameObject)Instantiate(Resources.Load(type.source), transform, true);
				unitScript unit = newUnit.GetComponent<unitScript>();
				
				unit.setData(type.unitName, type.unitClass, type.health, type.speed, type.skill, type.strength, type.smarts, "", new List<string>(type.weapons));
				unit.gameObject.layer = 8;
				unit.setPosition((int)position.x, (int)position.y);
				unit.setOwner(1);
				AIUnitController ai = unit.gameObject.AddComponent<AIUnitController>();
				ai.healthThreshold = type.healthThreshold;
				team.Add(unit);
				unitsInGame.Add(unit);

			}
		}
		playerUnitList = team.ToArray();

	}
	// Update is called once per frame
	void Update () {
		if (map == null)
			initiateController();

		if (isTurn())
		{


			if (currentIndex == playerUnitList.Length)
			{
				endTurn();
				map.clearDisplay();
				currentIndex = 0;
				return;
			}
			if (!(playerUnitList[currentIndex] != null))
			{
				currentIndex++;
				return;
			}
			AIUnitController currentUnitAI = playerUnitList[currentIndex].GetComponent<AIUnitController>();

			if (!(currentUnitAI.moved && currentUnitAI.attacked))
			{
				unitScript unit = playerUnitList[currentIndex];
				// issue commands to current unit
				switch (unit.GetComponent<AIUnitController>().state)
				{
					case AIState.test:
						testMove(unit);
						break;
					case AIState.hunt:
						hunt(unit);
						break;

					case AIState.fallback:
						fallBack(unit);
						break;

					case AIState.groupUp:
						groupUp(unit);
						break;

					case AIState.recover:
						recover(unit);
						break;

					case AIState.support:
						support(unit);
						break;
				}
			}
			else if (playerUnitList[currentIndex].canEndTurn())
			{
				playerUnitList[currentIndex].GetComponent<AIUnitController>().endTurn();
				currentIndex++;
				
			}

			
		}
		

	}
	void testMove(unitScript unit)
	{
		pathFinder.getMovementPaths(unit.GetComponent<GridItem>().getPos(), unit.getMovementDistance(), false);
		var locations = pathFinder.getReachableLocations();
		bool f = true;
		foreach(Vector2 location in locations)
		{
			
			float chance = Random.Range(0.0f, 1.0f);
			if(!f && chance > 0.5)
			{
				if(!unit.hasMoved())
				{
					Vector2[] path = pathFinder.drawPath(location, false);
					unit.setPath(path);
				}
					
				
			}
			if(f)
				f = !f;
		}
	}
	void hunt(unitScript unit)
	{
		pathFinder.getMovementPaths(unit.GetComponent<GridItem>().getPos(), 100, false);
		//var locations = pathFinder.getReachableLocations();

		if (unit.getCurrentWeapon().type == WeaponType.melee)
		{
			if (!unit.GetComponent<AIUnitController>().moved)
			{
				//get position of all enemies
				unitScript[] units = getAllUnits();
				List<unitScript> targetList = new List<unitScript>(units);
				//Pick target
				unitScript target = targetPrioritisation(unit, targetList);
                if (target == null)
                {
                    unit.GetComponent<AIUnitController>().moved = true;
                    unit.GetComponent<AIUnitController>().attacked = true;
                    return;
                }

                unit.GetComponent<AIUnitController>().moved = true;
				//Get the positions one short of target
				
				var oneShort = target.GetComponent<GridItem>().getPos() - unit.GetComponent<GridItem>().getPos();
				var targetPosXclose = target.GetComponent<GridItem>().getPos() - new Vector2(Mathf.Sign(oneShort.x), 0);
				var targetPosYclose = target.GetComponent<GridItem>().getPos() - new Vector2(0, Mathf.Sign(oneShort.y));
				var targetPosXfar = target.GetComponent<GridItem>().getPos() + new Vector2(Mathf.Sign(oneShort.x), 0);
				var targetPosYfar = target.GetComponent<GridItem>().getPos() + new Vector2(0, Mathf.Sign(oneShort.y));
				List<Vector2[]> paths = new List<Vector2[]>();
				paths.Add(pathFinder.drawPath(targetPosXclose, false, true));
				paths.Add(pathFinder.drawPath(targetPosYclose, false, true));
				paths.Add(pathFinder.drawPath(targetPosXfar, false, true));
				paths.Add(pathFinder.drawPath(targetPosYfar, false, true));
				//For each potential places try to move there
				foreach (var path in paths)
				{

					if (path != null)
					{
						//if the path is not null attack
						Vector2[] newPath = new Vector2[Mathf.Min(path.Length, unit.getMovementDistance())];

						for (int i = 0; i < newPath.Length; i++)
							newPath[i] = path[i];

						unit.setPath(newPath);
						break;
					}
				}
				unit.GetComponent<AIUnitController>().moved = true;


			}
			else if (unit.canEndTurn())
			{
				//Attack target
				MeleeAttack(unit, unit.GetComponent<AIUnitController>().target);
				unit.GetComponent<AIUnitController>().attacked = true;

			}


		}
		else if (unit.getCurrentWeapon().type == WeaponType.ranged)
		{
			if (!unit.GetComponent<AIUnitController>().moved)
			{
				Vector2 initialPosition = unit.GetComponent<GridItem>().getPos();
				//Initialise heursticmap
				var locations = pathFinder.getReachableLocations();
				Dictionary<Vector2, float> heuristicMap = new Dictionary<Vector2, float>();
                RangedWeapon currentGun = (RangedWeapon)unit.getCurrentWeapon();
				var enemyUnits = getEnemyUnits();
				foreach (Vector2 pos in locations.Where(p=>Mathf.Abs(p.x-unit.GetComponent<GridItem>().getPos().x) + Mathf.Abs(p.y - unit.GetComponent<GridItem>().getPos().y) < unit.getMovementDistance()))
				{
					unit.setPosition((int)pos.x, (int)pos.y);
					float h = 0.0f;

                    //Get all cover tiles around the current
                    var neighboringTiles = this.map.getAllNeighbours(pos).Where(p => this.map.getTileData(p).coverValue != 0);

                    //How much will this tile hurt?
                    h += enemyUnits.Where(e => getRangePenalty(e, Vector3.Distance(e.GetComponent<GridItem>().getVectorPostion(), unit.GetComponent<GridItem>().getVectorPostion())) == 0)
                        .Aggregate(0, (subHeuristic, enemy) => subHeuristic - 1);

                    //Add back all values from cover
                    foreach (Vector2 cover in neighboringTiles)
                    {
                        //Direction of cover
                        Vector2 vectorToCover = cover - pos;
                        //If the cover is between me and an enemy then increase heurstic
                        h += enemyUnits
                            .Where(
                                e => // The enemy is not melee or shield
                                e.getCurrentWeapon().type == WeaponType.ranged
                                && //I am within enemy range
                                getRangePenalty(e, Vector3.Distance(e.GetComponent<GridItem>().getVectorPostion(), unit.GetComponent<GridItem>().getVectorPostion())) == 0
                                && //And cover is between the enemy and myself
                                (Vector2.Angle(e.GetComponent<GridItem>().getPos() - pos, vectorToCover) < 45)
                            ).Aggregate(0,(subHeurstic,e)=>subHeurstic+(this.map.getTileData(cover).coverValue+2));
                    }

                    // How much damage can I do from this position
                    h += enemyUnits.Select(e => getRangePenalty(unit, Vector3.Distance(e.GetComponent<GridItem>().getVectorPostion(), unit.GetComponent<GridItem>().getVectorPostion())) == 0)
                        .Aggregate(0, (subHeurisitic, e) => subHeurisitic + (e?1:-1));//unit.getCurrentWeapon().damage);

					heuristicMap[pos] = h;
                    this.map.displayDebugData((int)pos.x, (int)pos.y, h.ToString());
                    unit.setPosition((int)initialPosition.x, (int)initialPosition.y);
                }
				unit.setPosition((int)initialPosition.x, (int)initialPosition.y);
                
				//Pick location with highest score
				Vector2 moveLocation = heuristicMap.Aggregate((l, r) => l.Value >= r.Value ? l : r).Key;

                print("KEY/Value " + moveLocation + " " + heuristicMap[moveLocation]);
				//Move there
				var path = pathFinder.drawPath(moveLocation, false, true);
				

				//if the path is not null attack
				Vector2[] newPath = new Vector2[Mathf.Min(path.Length, unit.getMovementDistance())];

				for (int i = 0; i < newPath.Length; i++)
					newPath[i] = path[i];

				unit.setPath(newPath);

				unit.GetComponent<AIUnitController>().moved = true;

				//If an attack can be made make it
			}
			if (unit.canEndTurn())
			{
				//Find best target
				unitScript target = targetPrioritisation(unit, getEnemyUnits());
                if (target == null)
                {
                    unit.GetComponent<AIUnitController>().attacked = true;
                    return;
                }
                    
                //Attack target
                RangedAttack(unit, unit.GetComponent<AIUnitController>().target);
				unit.GetComponent<AIUnitController>().attacked = true;

			}


		}
		else if (unit.getCurrentWeapon().type == WeaponType.shield)
		{
			if (swapTo(unit, WeaponType.melee))
			{
				print("ready Sword");
			}
			else if(swapTo(unit,WeaponType.ranged))
			{
				print("ready Gun");
			}
		}
	}
	void fallBack(unitScript unit)
	{
		pathFinder.getMovementPaths(unit.GetComponent<GridItem>().getPos(), unit.getMovementDistance(), false);
		//run away from enemy
			//Calculate vector 



		//if has shield switch to it and deploy
		if (swapTo(unit, WeaponType.shield))
		{
			activateShield(unit, unit.GetComponent<GridItem>());
		}
	}

	void recover(unitScript unit)
	{
		

	}

	void groupUp(unitScript unit)
	{
		//get average position of allies

	}

	void support(unitScript unit)
	{

	}

	//Assisting functions
	unitScript targetPrioritisation(unitScript unit, List<unitScript> enemyList)
	{
		AIUnitController unitControl = unit.GetComponent<AIUnitController>();

		List<float> priorites = new List<float>();
		foreach (unitScript otherUnit in enemyList)
		{
			//Add factors
			float priority = 0;

			if (otherUnit == null || otherUnit.getOwner() == controllerID || otherUnit.getHealth() <= 0)
			{
				priorites.Add(-1000000);
				break;
			}
				//distance - wrecklessness (negative if over wrecklessness
			priority -= Mathf.Max(0, Vector2.Distance(unit.GetComponent<GridItem>().getPos(), otherUnit.GetComponent<GridItem>().getPos()) - unitControl.wrecklessness);
			//deuling preference
			bool weaponsMatch = unit.getCurrentWeapon().type == otherUnit.getCurrentWeapon().type;
			if ((weaponsMatch && unitControl.deulingPreference) || (!weaponsMatch && !unitControl.deulingPreference))
			{
				priority += 2;
			}
			//Agro multiplied by anger
			if(unitControl.agroTarget == otherUnit)
			{
				priority += 10 * unitControl.anger;
			}
			priorites.Add(priority);
		}
		priorites.ForEach(e=>print("Inlist"+e));
		int maxIndex = priorites.IndexOf(priorites.Max());
		unitControl.target = enemyList[maxIndex];
		print("Selected : "+enemyList[maxIndex]);
		return enemyList[maxIndex];

	}

	List<unitScript> getEnemyUnits()
	{
		unitScript[] units = getAllUnits();
		return new List<unitScript>(units).Where(a => a.getOwner() != controllerID && a.getHealth() > 0).ToList();
	}

	bool swapTo(unitScript unit,WeaponType type)
	{
		for (int i = 0; i < unit.getWeaponCount(); i++)
		{
			if (unit.getWeapon(i).type == type)
			{
				unit.setCurrentWeapon(i);
				return true;
			}
		}

		return false;
	}
}
