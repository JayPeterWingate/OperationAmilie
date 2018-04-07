using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
/*
public struct unitData
{
    public string name;
    public string unitClass;
    public int health,speed,skill,strength,smarts,exp;
    public string portraitString, source;
    public List<string> unitweapons;

};
public class PlayerData : MonoBehaviour {
    public TextAsset playerData;
    int money;
    unitData[] units;
    Dictionary<string,int[]> Inventory;
	WeaponsHandler weaponManager;

    public unitData getUnit(int i)
    {
        if (i < units.Length)
            return units[i];
        return new unitData();
    }
    public int getUnitCount(){
        return units.Length;
    }
	// Use this for initialization
	void Start () {
        money = 100;
        weaponManager = GetComponent<WeaponManager>();
        print(weaponManager);
        loadTeam();
        loadInventory();
	}
    public void loadInventory()
    {
        Inventory = new Dictionary<string, int[]>();
        print(Inventory);
        Inventory.Add("Weapons", new int[weaponManager.getWeaponCount()]);

        for (int i = 0; i < Inventory["Weapons"].Length; i++)
        {
            Inventory["Weapons"][i] = 3;
        }
    }
    public void loadTeam()
    {

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(playerData.text);

        XmlNode data = xml.SelectSingleNode("data");
        units = new unitData[data.ChildNodes.Count];

        for (int i = 0; i < data.ChildNodes.Count; i++)
        {
            XmlNode current = data.ChildNodes[i];
            unitData newUnit = new unitData();
            newUnit.name = current.SelectSingleNode("name").InnerText;
            newUnit.unitClass = current.SelectSingleNode("class").InnerText;
            newUnit.health = int.Parse(current.SelectSingleNode("health").InnerText);
            newUnit.speed = int.Parse(current.SelectSingleNode("speed").InnerText);
            newUnit.skill = int.Parse(current.SelectSingleNode("skill").InnerText);
            newUnit.strength = int.Parse(current.SelectSingleNode("strength").InnerText);
            newUnit.smarts = int.Parse(current.SelectSingleNode("smarts").InnerText);
            //newUnit.exp = int.Parse(current.SelectSingleNode("exp").InnerText);
            newUnit.portraitString = current.SelectSingleNode("portait").InnerText;
            newUnit.source = current.SelectSingleNode("source").InnerText;


            XmlNode weaponNode = current.SelectSingleNode("weapons");
            List<string> unitWeapons = new List<string>();
            foreach (XmlNode weapon in weaponNode.ChildNodes)
            {
                unitWeapons.Add(weapon.InnerText);
            }
            newUnit.unitweapons = unitWeapons;
            units[i]=(newUnit);
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
*/