using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorScript : MonoBehaviour {
	bool isOpen;
	public void open()
	{
        Vector2 position = transform.GetComponent<GridItem>().getPos();
        isOpen = true;
		GetComponent<clickableTile>().isWalkable = true;

		GetComponent<Animator>().SetBool("isOpen",true);

        GameObject.Find("GameScripts").GetComponent<TileMap>().setTilePassable((int)position.x, (int)position.y, true);
    }
	public void close()
	{
        Vector2 position = transform.GetComponent<GridItem>().getPos();
        isOpen = false;
		GetComponent<clickableTile>().isWalkable = false;
		GetComponent<Animator>().SetBool("isOpen", false);
        GameObject.Find("GameScripts").GetComponent<TileMap>().setTilePassable((int)position.x,(int)position.y,false);
        GameObject.Find("GameScripts").GetComponent<TileMap>().setTilePassable((int)position.x, (int)position.y, false);
    }
	
	public void toggleDoor()
	{
		if(isOpen)
		{
			close();
		}
		else
		{
			open();
		}
	}

	/*
	private void OnTriggerEnter(Collider other)
	{
		open();
	}
	private void OnTriggerExit(Collider other)
	{
		close();
	}
	*/
}
