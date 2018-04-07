using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

	unitScript[] playerTeam;
	unitScript unit;
	WeaponType lastWeaponType;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (unit != null)
		{ 
			if (unit.canOpenDoor())
			{
				transform.FindChild("CharacterPanel").FindChild("DoorButton").GetComponent<Button>().interactable = true;
			}
			else
			{
				transform.FindChild("CharacterPanel").FindChild("DoorButton").GetComponent<Button>().interactable = false;
			}


			if(unit.getCurrentWeapon().type != lastWeaponType)
			{
				
				Transform attackButton = transform.Find("CharacterPanel").FindChild("AttackButton");

				attackButton.FindChild("CrossHair").gameObject.SetActive(false);
				attackButton.FindChild("Sword").gameObject.SetActive(false);
				attackButton.FindChild("Shield").gameObject.SetActive(false);

				if (unit.getCurrentWeapon().type == WeaponType.ranged)
					attackButton.FindChild("CrossHair").gameObject.SetActive(true);

				else if (unit.getCurrentWeapon().type == WeaponType.melee)
					attackButton.FindChild("Sword").gameObject.SetActive(true);

				else if (unit.getCurrentWeapon().type == WeaponType.shield)
					attackButton.FindChild("Shield").gameObject.SetActive(true);
				

				lastWeaponType = unit.getCurrentWeapon().type;
			}
			if(unit.hasAttacked()|| unit.isMoving()  || (unit.getCurrentWeapon().type == WeaponType.shield && ((Shield)unit.getCurrentWeapon()).isBroken(unit.GetInstanceID())))
			{
				Transform attackButton = transform.Find("CharacterPanel").FindChild("AttackButton");
				attackButton.GetComponent<Button>().interactable = false;
			}
			else
			{
				Transform attackButton = transform.Find("CharacterPanel").FindChild("AttackButton");
				attackButton.GetComponent<Button>().interactable = true;
			}

		}
		
	}
	public void setPlayerTeam(unitScript[] team)
	{
		transform.Find("SelectionPanel").gameObject.SetActive(true);

		GameObject selectorPanel = GameObject.Find("SelectionPanel").gameObject;
		print(selectorPanel);
		playerTeam = team;
		int t = 0;
		for(int i = 0; i < team.Length; i++)
		{
			if (team[i] == null) continue; 
			var newPanel = Instantiate<GameObject>(Resources.Load<GameObject>("UIElements/Selector"), selectorPanel.transform);
			newPanel.transform.position = new Vector3( 55+(t * 110), Screen.height-60);
			newPanel.transform.FindChild("portait").GetComponent<Image>().sprite = team[i].getPortrait();
			newPanel.name = i.ToString();
			newPanel.transform.FindChild("SelectorButton").GetComponent<Button>().onClick.AddListener(() => {this.unitClicked(int.Parse(newPanel.name)); });
			t++;
		}
	}
	void Hide()
	{
		for(int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
		
	}
	public void setCharacterPanelVisibility(bool value, unitScript unit = null,bool targetLeft = true)
	{
		if (value && unit)
		{
			loadCharacter(unit, unit.getOwner() == 0);
		}
        if(unit && unit.getOwner() == 0)
        {
            transform.FindChild("CharacterPanel").gameObject.SetActive(value);
        }
        if(!value && targetLeft)
        {
            transform.FindChild("CharacterPanel").gameObject.SetActive(false);
        }
        if(unit)
        {
            transform.FindChild(unit.getOwner() == 0 ? "StatPanel" : "EnemyStatPanel").gameObject.SetActive(value);
            transform.FindChild(unit.getOwner() == 0 ? "WeaponPanel" : "EnemyWeaponPanel").gameObject.SetActive(value);
        }
        else 
        {
            transform.FindChild(targetLeft ? "StatPanel" : "EnemyStatPanel").gameObject.SetActive(value);
            transform.FindChild(targetLeft ? "WeaponPanel" : "EnemyWeaponPanel").gameObject.SetActive(value);
        }
		

	}
	public void setCombatPanelVisibility(bool value, unitScript attacker = null,unitScript defender = null, float chanceToHit = 0.0f, float defenderChanceToHit = 0.0f)
	{
		if(value && attacker && defender)
		{
			loadCombatData(attacker,defender,chanceToHit, defenderChanceToHit);
		}
		transform.FindChild("CombatPanel").gameObject.SetActive(value);
	}
    public void loadCharacter(unitScript u, bool ally = true)
	{
        if(ally)
		    unit = u;
		Transform characterPanel = transform;	
		Transform statPannel = characterPanel.Find(ally?"StatPanel":"EnemyStatPanel");

		UnityEngine.UI.Text nameText = statPannel.Find("Name").GetComponent<UnityEngine.UI.Text>();
		nameText.text = u.getName();

		statPannel.FindChild("Image").GetComponent<Image>().sprite = u.getPortrait();

		var strengthText = statPannel.Find("Strength").GetComponent<UnityEngine.UI.Text>();
		strengthText.text = u.getStrength().ToString();

		var skillText = statPannel.Find("Skill").GetComponent<UnityEngine.UI.Text>();
		skillText.text = u.getSkill().ToString();

		var speedText = statPannel.Find("Speed").GetComponent<UnityEngine.UI.Text>();
		speedText.text = u.getSpeed().ToString();

		var smartsText = statPannel.Find("Smarts").GetComponent<UnityEngine.UI.Text>();
		smartsText.text = u.getSmarts().ToString();

        loadHealth(u.getHealth(),u.getMaxHealth(),statPannel);
		
        if(ally)
        {
            if (unit.doorsWithinRange.Count > 0)
            {
                transform.FindChild("CharacterPanel").FindChild("DoorButton").GetComponent<Button>().interactable = true;
            }
            else
            {
                transform.FindChild("CharacterPanel").FindChild("DoorButton").GetComponent<Button>().interactable = false;
            }

        }

        loadWeaponData(u,ally);

	}
    public void loadHealth(int current, int max, Transform statPanel)
    {   
        Transform container = statPanel.Find("HeartContainer");

        foreach ( var i in Enumerable.Range(1, 15))
        {
            Image heart = container.Find("h"+i).GetComponent<Image>();

            if (current > i)
            {
                heart.enabled = true;
                heart.color = Color.white;
            }
            else if (max > i)
            {
                heart.enabled = true;
                heart.color = Color.HSVToRGB(0,0,0);
            }
            else
            {
                heart.enabled = false;
            }
                
        }

    }
    public void loadWeaponData(unitScript u,bool ally)
    {
        var WeaponPanel = transform.FindChild(ally?"WeaponPanel":"EnemyWeaponPanel");
        WeaponPanel.gameObject.SetActive(true);
        var weapon = u.getCurrentWeapon();
        WeaponPanel.transform.Find("Weapon").GetComponent<Text>().text = weapon.name;
        WeaponPanel.transform.Find("Damage").GetComponent<Text>().text = weapon.damage.ToString();

        var rangedPanel = WeaponPanel.transform.Find("Ranged");
        switch (weapon.type)
        {
            case WeaponType.ranged:
                rangedPanel.gameObject.SetActive(true);
                RangedWeapon current = (RangedWeapon)weapon;

                rangedPanel.Find("ShortRange").GetComponent<Text>().text = current.minRange.ToString();
                rangedPanel.Find("LongRange").GetComponent<Text>().text = current.maxRange.ToString();

                break;
            default:
                rangedPanel.gameObject.SetActive(false);
                break;

        }
    }
	public void loadCombatData(unitScript attacker, unitScript defender,float AttackerChanceToHit, float defenderChanceToHit)
	{
		Transform combatPanel = transform.Find("CombatPanel");
		Transform EnemyPanel = combatPanel.Find("EnemyPanel");

		var targetText = EnemyPanel.Find("Target").GetComponent<UnityEngine.UI.Text>();
		targetText.text = defender.getName();

		var classText = EnemyPanel.Find("Class").GetComponent<UnityEngine.UI.Text>();
		classText.text = defender.getClass();

		var healthText = EnemyPanel.Find("Health").GetComponent<UnityEngine.UI.Text>();
		healthText.text = defender.getHealth().ToString() + " : " + defender.getMaxHealth().ToString();

		var strikeBack = EnemyPanel.Find("StrikeBack").GetComponent<Text>();
		strikeBack.text = (!defender.getStrikeback()).ToString();

		var strikebackChanceText = EnemyPanel.Find("ChanceText").GetComponent<Text>();
		var strikebackDamageText = EnemyPanel.Find("DamageText").GetComponent<Text>();

		var strikebackChance = EnemyPanel.Find("Chance").GetComponent<Text>();
		var strikebackDamage = EnemyPanel.Find("Damage").GetComponent<Text>();


		if (!defender.getStrikeback())
		{
			strikebackChance.enabled = true;
			strikebackDamage.enabled = true;
			strikebackChanceText.enabled = true;
			strikebackDamageText.enabled = true;

			strikebackChance.text = defenderChanceToHit.ToString();

			strikebackDamage.text = defender.getCurrentWeapon().damage.ToString();

		}
		else
		{

			strikebackChance.enabled = false;
			strikebackDamage.enabled = false;
			strikebackChanceText.enabled = false;
			strikebackDamageText.enabled = false;
		}
		int chance = (int)Mathf.Max(0,Mathf.Min(100, AttackerChanceToHit * 100));
		combatPanel.Find("ChanceToHit").GetComponent<UnityEngine.UI.Text>().text = (chance).ToString()+"%";
		combatPanel.Find("Damage").GetComponent<UnityEngine.UI.Text>().text = unit.getCurrentWeapon().damage.ToString();

	}
	public void hideRangedPanel()
	{
		Transform combatPanel = transform.Find("CombatPanel");
		Transform rangedPanel = combatPanel.Find("RangedPanel");

		rangedPanel.gameObject.SetActive(false);
	}
	public void loadRangedData(bool s, float b, float r, float c)
	{
		
		Transform combatPanel = transform.Find("CombatPanel");
		Transform rangedPanel = combatPanel.Find("RangedPanel");

		rangedPanel.gameObject.SetActive(true);

		Text baseText = rangedPanel.Find("Base").GetComponent<Text>();
		Text coverText = rangedPanel.Find("CoverPenalty").GetComponent<Text>();
		Text rangedText = rangedPanel.Find("RangedPenalty").GetComponent<Text>();

		if(s)
		{
			baseText.text = "Shielded";
			rangedText.text = "Shielded";
			coverText.text = "Shielded";
		}
		else
		{
			baseText.text = ((int)(b * 100)).ToString() + "%";
			rangedText.text = ((int)r * -10).ToString() + "%";
			coverText.text = ((int)(c * -10)).ToString() + "%";
		}
		
		

	}

	public void unitClicked(int i)
	{
		GameObject.Find("camPosition").transform.position = playerTeam[i].transform.position;
		GameObject.Find("Player").GetComponent<PlayerController>().selectUnit(playerTeam[i]);
	}
}
