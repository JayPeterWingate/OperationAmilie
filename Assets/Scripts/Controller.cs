using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Controller : MonoBehaviour {

	protected static List<unitScript> unitsInGame = new List<unitScript>();

	protected bool currentTurn;
	protected int controllerID;

	protected TileMap map;
	protected unitScript[] playerUnitList;
	protected PathfinderScript pathFinder;

	public TextAsset playerData;

	public static void newGame()
	{
		unitsInGame = new List<unitScript>();
	}
	// Use this for initialization
	void Start () {
		print(unitsInGame.Count);
	}
	
	// Update is called once per frame
	void Update () {

	}
	protected void initiateController()
	{
		currentTurn = false;
		map = transform.parent.GetComponent<TileMap>();
		spawnTeam();
		pathFinder = gameObject.AddComponent<PathfinderScript>();
		pathFinder.setMap(map);
	}
	public bool isTurn(){ return currentTurn;}

	public virtual void spawnTeam()
	{
	}

	protected virtual bool Attack(unitScript unit, unitScript clickedUnit)
	{
		//determine attack type
		if(unit.getCurrentWeapon().type == WeaponType.melee)
		{
			return MeleeAttack(unit, clickedUnit);
		}
		if(unit.getCurrentWeapon().type == WeaponType.ranged)
		{
			return RangedAttack(unit,clickedUnit);
		}
		return false;
	}

	protected bool activateShield(unitScript unit,GridItem position)
	{

		if (!(unit.getCurrentWeapon().type == WeaponType.shield))
			return false;
		Shield currentShield = (Shield)unit.getCurrentWeapon();

		if (currentShield.isBroken(unit.GetInstanceID()))
			return false;

		if(currentShield.getShield(unit.GetInstanceID()) != null)
		{
			Destroy(currentShield.getShield(unit.GetInstanceID()));
		}

		//Check position is within range
		if(currentShield == null || Vector2.Distance(unit.GetComponent<GridItem>().getPos(),position.getPos()) > currentShield.range)
		{
			return false;
		}
		//Create the game object
		//TODO- Fix the roation and shape of shields to be frontal not surround
		Vector3 toPosition = position.getVectorPostion() - unit.GetComponent<GridItem>().getVectorPostion();
		float rotation = Vector3.Angle(new Vector3(0, 0, 1), Vector3.Cross(toPosition,new Vector3(0, 1, 0)));
		print(rotation);
		GameObject newShield = Instantiate<GameObject>(Resources.Load<GameObject>("Units/Shield"), position.getVectorPostion(), Quaternion.Euler(0, rotation, 0));
		currentShield.setShield(unit.GetInstanceID(),newShield);

		newShield.GetComponent<shieldScript>().setStats(controllerID, unit.getSmarts() * 3);
		//Check it's collisions with other shields
		currentShield.shieldBreakCheck();


		unit.activateAttack();

		print("ACTIVATE SHIELD");
		map.UnHilightMap();

		

		return true;
	}

	protected bool MeleeAttack(unitScript unit, unitScript clickedUnit, bool initialAttack = true)
	{

		// Check attack can occur
		if(unit.getCurrentWeapon().type == WeaponType.ranged || unit == null || clickedUnit == null)
			return false;
		
		float distance = Vector2.Distance(unit.GetComponent<GridItem>().getPos(), clickedUnit.GetComponent<GridItem>().getPos());
		if (!(unit.getCurrentWeapon().type == WeaponType.melee) || distance > ((MeleeWeapon)unit.getCurrentWeapon()).range)
			return false;

		// Given it can roll the dice
		float roll = UnityEngine.Random.Range(0.0f, 1.0f);
		
		float chanceToHit = meleeChanceToHit(unit, clickedUnit);
		print(roll + " " + chanceToHit);
		if (roll < chanceToHit)
		{
			clickedUnit.takeDamage(unit.getStrength()+unit.getCurrentWeapon().damage);

		}
		else
		{
			clickedUnit.displayMiss();
		}
		unit.activateAttack();
		if (initialAttack && clickedUnit.getStrikeback() == false)
		{
			bool result = MeleeAttack(clickedUnit, unit, false);
			if(result)
				clickedUnit.useStrikeback();
		}
		return true;
	}

	protected float meleeChanceToHit(unitScript unit, unitScript clickedUnit)
	{
		if(unit.getCurrentWeapon().type == WeaponType.ranged)
			return 0;
		
		// Check outflank bonus
		int flankBonus = 0; // Flank Check
		Vector2 enemyPosition = clickedUnit.GetComponent<GridItem>().getPos();
		foreach (unitScript u in playerUnitList)
		{
			if(u != null)
			{
				Vector2 unitPosition = u.GetComponent<GridItem>().getPos();
				if (u.getOwner() == controllerID && u != unit && Vector2.Distance(unitPosition, enemyPosition) < 2)
				{

					flankBonus = 2;
					print("Flanked Bitches");
				}
			}
			
		}


		// return chance to hit
		return (5.0f + (unit.getSkill() + flankBonus) - clickedUnit.getSkill()) / 10.0f;
	}

	protected bool RangedAttack(unitScript unit, unitScript clickedUnit, bool initialAttack = true)
	{

		// Check LoS
		if (!canSee(unit, clickedUnit))
			return false;

		if (hitShield(unit, clickedUnit))
			return true;

		//Resolve Attack

		float roll = UnityEngine.Random.Range(0.0f, 1.0f);
		float chanceToHit = rangedChanceToHit(unit, clickedUnit);
		print(roll + " " + chanceToHit);
		if (roll < chanceToHit)
		{
			clickedUnit.takeDamage(unit.getCurrentWeapon().damage);

		}
		else
		{
			clickedUnit.displayMiss();
		}
		unit.activateAttack();
		if (initialAttack && clickedUnit.getStrikeback() == false)
		{
			bool result;
			if (clickedUnit.getCurrentWeapon().type == WeaponType.melee)
			{
				result = MeleeAttack(clickedUnit, unit, false);
			}
			else if(clickedUnit.getCurrentWeapon().type == WeaponType.ranged)
			{
				result = RangedAttack(clickedUnit, unit, false);
			}
			else
			{
				result = false;
			}
			if (result)
				clickedUnit.useStrikeback();
		}
		return true;
	}

	protected float rangedChanceToHit(unitScript unit, unitScript clickedUnit)
	{
		if (unit.getCurrentWeapon().type == WeaponType.melee)
		{
			return 0.0f;
		}
		//Calculate range penalty
		float distance = Vector2.Distance(unit.GetComponent<GridItem>().getPos(), clickedUnit.GetComponent<GridItem>().getPos());

		float rangedPenalty = getRangePenalty(unit, distance);

		//Calculate cover penalty
		float coverPenalty = getCoverPenalty(unit,clickedUnit,distance);
		
		//Get base shooting skill
		float shootingSkill = getBaseHitChance(unit);
		//Return formula
		return shootingSkill - (rangedPenalty + coverPenalty) / 10;
	}
	protected float getBaseHitChance(unitScript unit)
	{
		return Mathf.Log10(unit.getSkill());
	}
	protected float getRangePenalty(unitScript unit, float distance)
	{
        RangedWeapon weapon;
        float rangedPenalty = 0.0f;
        try
        {
            weapon = (RangedWeapon)unit.getCurrentWeapon();
        } catch(InvalidCastException e)
        {
            return distance > unit.getMovementDistance() + 2 ? 0 : 100;
        }
        
		if (weapon.minRange > distance)
		{
			rangedPenalty = weapon.minRange - distance;
		}
		else if (weapon.maxRange < distance)
		{
			rangedPenalty = distance - weapon.maxRange;
		}
		else
		{
			rangedPenalty = 0;
		}
		return rangedPenalty;
	}
	protected float getCoverPenalty(unitScript unit, unitScript clickedUnit, float distance)
	{
		Vector2 unitPosition = unit.GetComponent<GridItem>().getPos();
		Vector2 enemyPosition = clickedUnit.GetComponent<GridItem>().getPos();
		return getCoverPenalty(new Vector3(unitPosition.x, 1, unitPosition.y), new Vector3(enemyPosition.x, 1.0f, enemyPosition.y)) ;
	}

	protected float getCoverPenalty(Vector3 unitPosition, Vector3 enemyPosition)
	{
		float coverPenatly = 0.0f;
		Ray ray = new Ray(enemyPosition, (unitPosition - enemyPosition));
		RaycastHit[] hits = Physics.RaycastAll(ray, 1.3f);
		//Debug.DrawRay(enemyPosition, ( unitPosition - enemyPosition),Color.red,100,false);
		
		foreach (RaycastHit hit in hits)
		{
			print(hit.transform.gameObject);
			if (hit.transform.gameObject.layer == 10)
			{
				coverPenatly = hit.transform.GetComponent<clickableTile>().coverValue;
			}
			if (hit.transform.gameObject.layer == 9)
			{
				coverPenatly = 100000;
			}
		}
		print(coverPenatly);
		return coverPenatly;
	}

	protected bool canSee(unitScript unit, unitScript clickedUnit)
	{
		Ray ray = new Ray(unit.transform.position + new Vector3(0, 1, 0), (clickedUnit.transform.position + new Vector3(0, 1, 0)- unit.transform.position + new Vector3(0, 1, 0)));
		float distance = Vector3.Distance(unit.transform.position, clickedUnit.transform.position);
		RaycastHit[] hits = Physics.RaycastAll(ray,distance);

		foreach (RaycastHit hit in hits)
		{
			if (hit.transform.gameObject.layer == 9)
			{
				print("shooting through wall");
				print(hit.transform.gameObject.name);
				return false;
			}
		}
		return true;
	}
	protected bool isShielded(unitScript unit, unitScript clickedUnit)
	{
		Ray ray = new Ray(unit.transform.position + new Vector3(0, 1, 0), (clickedUnit.transform.position + new Vector3(0, 1, 0) - unit.transform.position + new Vector3(0, 1, 0)));
		float distance = Vector3.Distance(unit.transform.position, clickedUnit.transform.position);
		RaycastHit[] hits = Physics.RaycastAll(ray, distance);

		foreach (RaycastHit hit in hits)
		{
			if (hit.transform.gameObject.layer == 11)
			{
				return true;
			}
		}
		return false;
	}
	protected bool hitShield(unitScript unit, unitScript clickedUnit)
	{
		Ray ray = new Ray(unit.transform.position + new Vector3(0, 1, 0), (clickedUnit.transform.position + new Vector3(0, 1, 0) - unit.transform.position + new Vector3(0, 1, 0)));
		float distance = Vector3.Distance(unit.transform.position, clickedUnit.transform.position);
		RaycastHit[] hits = Physics.RaycastAll(ray, distance).OrderBy(h => h.distance).ToArray();

		foreach (RaycastHit hit in hits)
		{
			if(hit.transform.gameObject.layer == 11)
			{
				shieldScript shield = hit.transform.gameObject.GetComponent<shieldScript>();
				if (shield.owner != controllerID)
					shield.takeDamage(unit.getCurrentWeapon().damage);
					return true;
			}
		}
		return false;

	}
	protected virtual bool endTurn()
	{
		foreach(unitScript unit in playerUnitList)
		{
			if (unit != null && !unit.canEndTurn())
			{
				GameObject.Find("camPosition").transform.position = unit.GetComponent<GridItem>().getVectorPostion();
				return false;
			}
			
		}
		currentTurn = false;
		return true;
	}

	public virtual bool newTurn()
	{
		if (playerUnitList == null)
			return false;
		currentTurn = true;	
		foreach (unitScript unit in playerUnitList)
		{
			if(unit != null)
				unit.newTurn();
		}
		return true;
	}
	public void addUnit(int typeIndex, int x, int y )
	{
		EnemyType[] types = map.getEnemyTypes();
		EnemyType type = types[typeIndex];
		GameObject newUnit = (GameObject)Instantiate(Resources.Load(type.source), transform, true);
		unitScript unit = newUnit.GetComponent<unitScript>();

		unit.setData(type.unitName, type.unitClass, type.health, type.speed, type.skill, type.strength, type.smarts, "", new List<string>(type.weapons));
		unit.gameObject.layer = 8;
		unit.setPosition(x, y);
		unit.setOwner(1);
		AIUnitController ai = unit.gameObject.AddComponent<AIUnitController>();
		ai.healthThreshold = type.healthThreshold;
		List<unitScript> newList = new List<unitScript>(playerUnitList);
		newList.Add(unit);
		this.playerUnitList = newList.ToArray();
		unitsInGame.Add(unit);
	}
	public int remainingAlive()
	{
		
		return playerUnitList != null ? playerUnitList.Where(e=> e != null).Select(e => e.getHealth()).Where(e=>e > 0).Aggregate(0, (a,u) => u+a):1;
	}
	static public HashSet<Vector2> getUnitPositions()
	{
		HashSet<Vector2> positions = new HashSet<Vector2>();
		foreach (unitScript unit in unitsInGame)
		{
			if(unit != null)
				positions.Add(unit.GetComponent<GridItem>().getPos());
		}
			
		return positions;
	}
	static public unitScript[] getAllUnits()
	{
		return unitsInGame.ToArray();
	}
}
