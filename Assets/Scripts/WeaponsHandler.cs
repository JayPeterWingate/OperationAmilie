using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public enum WeaponType
{
	melee,
	ranged,
	shield
};
public enum WeaponClass
{
	None,
	Sidearm,
	Automatic,
	Antique,
	Sniper,
	Heavy,
    Mechanised,
	Blade,
    Blunt,
    Sharp,
    Energy,
	Polearm,
	Point
};
public class Weapon
{
	public string name;
	public WeaponType type;
	public WeaponClass weaponClass;
	public int damage;
	public int requirement;
    
}
public class MeleeWeapon: Weapon
{
	public int range;
}
public class RangedWeapon : Weapon
{
	public int minRange;
	public int maxRange;
	public bool stationary;
}
public class Shield : Weapon
{
	public int range;
	static Dictionary<int,bool> brokenShield = new Dictionary<int, bool>();
	static Dictionary<int,GameObject> deployedShields = new Dictionary<int, GameObject>();

	public void setShield(int index, GameObject shield)
	{
		if (deployedShields.ContainsKey(index))
		{
			if(brokenShield[index] == false)
				deployedShields[index] = shield;
		}
		else if(!brokenShield.ContainsKey(index))
		{
			deployedShields.Add(index, shield);
			brokenShield.Add(index, false);
		}
	}
	public bool isBroken(int index)
	{
		if (brokenShield.ContainsKey(index))
			return brokenShield[index];
		else
			return false;
	}
	public GameObject getShield(int index)
	{
		if (deployedShields.ContainsKey(index))
		{
			return deployedShields[index];
		}
		return null;
	}
	public GameObject breakShield(int index)
	{
		brokenShield[index] = true;
		return deployedShields[index];
	}
	public void shieldBreakCheck()
	{
		int[] keys = new int[brokenShield.Keys.Count];
		brokenShield.Keys.CopyTo(keys,0);
		foreach (int index in keys)
		{
			if(deployedShields.ContainsKey(index) && deployedShields[index] != null)
			{
				GameObject shieldObject = deployedShields[index];
				shieldScript shield = shieldObject.GetComponent<shieldScript>();

				if (shield.broken || shield.health <= 0)
				{
					brokenShield[index] = true;
					shield.destoryShield();
					deployedShields.Remove(index);
				}
			}
		}
	}
}
public class WeaponsHandler : MonoBehaviour {

	
	Dictionary<string, Weapon> weaponList;
	public TextAsset weaponData;
	void Start()
	{
		weaponList = new Dictionary<string, Weapon>();
		XmlDocument xml = new XmlDocument();
		xml.LoadXml(weaponData.text);
		XmlNode data = xml.SelectSingleNode("data");

		for(int i = 0; i < data.ChildNodes.Count; i++)
		{


			XmlNode current = data.ChildNodes[i];
			Weapon newWeapon;
			if (current.SelectSingleNode("type").InnerText == "Melee")
			{
				MeleeWeapon newMelee = new MeleeWeapon();
				newMelee.type = WeaponType.melee;
				newMelee.range = int.Parse(current.SelectSingleNode("range").InnerText);
				
				newWeapon = newMelee;
				
			}
			else if(current.SelectSingleNode("type").InnerText == "Ranged")
			{
				RangedWeapon newRanged = new RangedWeapon();
				newRanged.type = WeaponType.ranged;
				newRanged.minRange = int.Parse(current.SelectSingleNode("minRange").InnerText);
				newRanged.maxRange = int.Parse(current.SelectSingleNode("maxRange").InnerText);
				newRanged.stationary = current.SelectSingleNode("stationary").InnerText.ToLower() == "true";

				newWeapon = newRanged;
			}
			else
			{
				Shield newShield = new Shield();
				newShield.type = WeaponType.shield;
				newShield.range = int.Parse(current.SelectSingleNode("range").InnerText);
				newWeapon = newShield;
			}
			newWeapon.name =  current.SelectSingleNode("name").InnerText;
			newWeapon.weaponClass = (WeaponClass)Enum.Parse(typeof(WeaponClass),current.SelectSingleNode("class").InnerText);
			newWeapon.damage = int.Parse(current.SelectSingleNode("damage").InnerText);
			newWeapon.requirement = int.Parse(current.SelectSingleNode("requirement").InnerText);

			weaponList[newWeapon.name] = newWeapon;
		}

	}
	private void Update()
	{
		foreach(Weapon w in weaponList.Values)
		{
			if(w.type == WeaponType.shield)
			{
				Shield shield = (Shield)w;
				shield.shieldBreakCheck();
			}
		}
	}
	public Dictionary<string, Weapon> getWeaponList()
	{
		return weaponList;
	}
}
