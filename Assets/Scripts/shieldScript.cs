using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shieldScript : MonoBehaviour {

	public bool broken = false;
	public int owner;
	public int health;
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 11)
			broken = true;
	}
	public void setStats(int o, int h)
	{
		owner = o;
		health = h;
	}
	public void destoryShield()
	{
		Object.Destroy(gameObject);
	}
	public void takeDamage(int damage)
	{
		transform.FindChild("MessageText").GetComponent<MessageControl>().displayMessage(damage.ToString(),Color.blue);
		health -= damage;
	}
}
