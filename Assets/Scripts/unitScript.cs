using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitScript : MonoBehaviour {

	//Control stuff
	int owner;
	bool selected;
	bool moved;
	bool attacked;

	//Animator
	Animator upDownAnim, leftRightAnim;
	
	//Stats
	string unitName;
	string className;
	int health = 1;
	int maxHealth;
	int skill;
	int smarts;
	int stength;

	int speed;
	int abilityCount;

	bool strikeBack;

	

	Sprite[] spriteSheet;
	int selectedWeapon = 0;
	List<string> weaponList;

	Sprite portrait;

	//Message
	MessageControl msgControl;

	//TileMap mapData;

	//Path stuff
	Vector2[] path;
	int position;
	float transferValue;

	//Environment variables
	public List<GameObject> doorsWithinRange; // This is a list of doors which the unit is within trigger of

	// Use this for initialization
	void Start() {
		deselect();
		//owner = 0;
		path = new Vector2[1];
		path[0] = GetComponent<GridItem>().getPos();
		upDownAnim = GetComponentsInChildren<Animator>()[0];
		leftRightAnim = GetComponentsInChildren<Animator>()[1];
		doorsWithinRange = new List<GameObject>();
		msgControl = gameObject.GetComponentInChildren<MessageControl>();
		strikeBack = false;

	}
	public void setData(string newName, string newClass,int newHealth, int newSpeed, int newSkill, int newStrength, int newSmarts, string newPortait, List<string> l)
	{
		unitName = newName;
		className = newClass;
		health = newHealth;
		maxHealth = health;
		speed = newSpeed;
		skill = newSkill;
		smarts = newSmarts;
		stength = newStrength;
		portrait = Resources.LoadAll<Sprite>(newPortait)[0];
		weaponList = l;

	}
	public void setPath(Vector2[] p)
	{
		if(p != null)
		{
			path = p;
			position = 0;
			transferValue = 0.0f;
			moved = true;
		}
		
	}
	public Sprite getPortrait()
	{
		return portrait;
	}
	// Update is called once per frame
	void Update() {

		Vector2 pos = GetComponent<GridItem>().getPos();
		if(pos != path[path.Length-1])
		{
			

			// IF not equal
			Vector2 origin = path[position];
			Vector2 target = path[position + 1];

			transferValue += speed*Time.deltaTime;

			//print(origin +" "+ target + "" + transferValue);


			Vector3 origin3 = new Vector3(origin.x, 0.01f, origin.y);
			Vector3 target3 = new Vector3(target.x, 0.01f, target.y);
			
			transform.position =  Vector3.Lerp(origin3, target3, transferValue);

			//Anim stuff
			//set up down anim
			if (origin.x < target.x || origin.y > target.y)
			{
				upDownAnim.SetBool("WalkRight", true);
			}
			else
			{
				upDownAnim.SetBool("WalkLeft", true);
			}
			//set left right anim
			if(origin.x > target.x || origin.y > target.y)
			{
				leftRightAnim.SetBool("WalkRight", true);
			}
			else
			{
				leftRightAnim.SetBool("WalkLeft", true);
			}
			if (transferValue >= 1.0f)
			{
				transferValue = 0.0f;
				GetComponent<GridItem>().setPosition(target);
				position++;
				if(position == path.Length-1)
				{
					upDownAnim.SetBool("TargetReached", true);
					leftRightAnim.SetBool("TargetReached", true);

					TileMap map = GameObject.FindGameObjectWithTag("GameController").GetComponent<TileMap>();
					map.eventManager.activateTriggers(gameObject);
					path[path.Length - 1] = GetComponent<GridItem>().getPos();
				}
					
				//print(GetComponent<GridItem>().getPos());
			}

		}

		if (!msgControl.isActive() && health <= 0)
		{
			GameObject.FindGameObjectWithTag("GameController").GetComponent<TileMap>().UnHilightMap();
			Destroy(gameObject);
		}
	}
	public void setPosition(int x, int y)
	{
		GridItem position = GetComponent<GridItem>();
		position.setPosition(x,y);
	}
	/*public void setMap(TileMap m)
	{
		mapData = m;
	}*/
	public void setName(string n)
	{
		unitName = n;
	}
	public string getName()
	{
		return unitName;
	}
	public void setCurrentWeapon(int i)
	{
		selectedWeapon = i;
	}
	public Weapon getCurrentWeapon()
	{
		WeaponsHandler wl = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<WeaponsHandler>();
		return wl.getWeaponList()[weaponList[selectedWeapon]];
	}
	public Weapon getWeapon(int i)
	{
		WeaponsHandler wl = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<WeaponsHandler>();
		return wl.getWeaponList()[weaponList[i]];
	}
	public int getWeaponCount() { return weaponList.Count; }
	public void setOwner(int value)
	{
		owner = value;
	}
	public int getOwner()
	{
		return owner;
	}
	public string getClass()
	{
		return className;
	}
	public int getSpeed()
	{
		return speed;
	}
	public int getStrength()
	{
		return stength; 
	}
	public int getSmarts()
	{
		return smarts;
	}
	public int getMovementDistance()
	{
		return 2 + (speed * 2); 
	}
	public int getSkill()
	{
		return skill;
	}
	public bool getStrikeback()
	{
		return strikeBack;
	}
	public void useStrikeback()
	{
		strikeBack = true;
	}
	public bool hasMoved()
	{
		return moved;
	}
	public bool isMoving()
	{
		Vector2 pos = GetComponent<GridItem>().getPos();
		return pos != path[path.Length - 1];
	}
	public int getHealth()
	{
		return health;
	}
	public int getMaxHealth()
	{
		return maxHealth;
	}
	public void activateAttack()
	{
		attacked = true;
		moved = true;
	}
	public bool hasAttacked()
	{
		return attacked;
	}
	public bool canOpenDoor()
	{
		// Return if player is in a door frame
		if (doorsWithinRange.Count == 0)
			return false;
		foreach(GameObject door in doorsWithinRange)
		{
			if (door.GetComponent<GridItem>().getPos() == GetComponent<GridItem>().getPos())
				return false;
		}
		return true;

	}
	public bool canEndTurn()
	{
		Vector2 pos = GetComponent<GridItem>().getPos();
		if (pos == path[path.Length - 1])
		{
			return true;
		}
		return false;
	}
	public bool newTurn()
	{
		Vector2 pos = GetComponent<GridItem>().getPos();
		moved = false;
		attacked = false;
		strikeBack = false;
		return true;
	}
	public void displayMiss()
	{
		msgControl.displayMessage("Miss", new Color(1, 1, 1));
	}
	public void takeDamage(int damage)
	{
		health -= damage;
		print(unitName + " has " + health + " left");
		msgControl.displayMessage(damage.ToString(), new Color(1, 0, 0));

		
	}

	public void select()
	{
		selected = true;
		transform.FindChild("Selector").GetComponent<MeshRenderer>().enabled = true;
		transform.FindChild("SelectorBall").GetComponent<MeshRenderer>().enabled = true;
	}
	public void deselect()
	{
		selected = false;
		transform.FindChild("Selector").GetComponent<MeshRenderer>().enabled = false;
		transform.FindChild("SelectorBall").GetComponent<MeshRenderer>().enabled = false;
	}

	private void OnDestroy()
	{
		print("Activate Death Message");
	}

	private void OnTriggerEnter(Collider other)
	{
		GameObject collider = other.gameObject;

		if (collider.tag == "door")
		{
			doorsWithinRange.Add(collider);

			print("Cake" + collider);
			print(doorsWithinRange[0]);
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		GameObject collider = other.gameObject;

		if (collider.tag == "door")
		{
			doorsWithinRange.Remove(collider);
		}
		print("Pie");

	}
}
