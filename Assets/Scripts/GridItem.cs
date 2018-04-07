using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItem : MonoBehaviour {

	static float gridSize = 1; 
	int x, y, z;
	// Use this for initialization
	void Start () {
		transform.position = new Vector3(x * gridSize, 0.01f, y * gridSize);
	}
	public void setPosition(int newX, int newY)
	{
		x = newX;
		y = newY;
		transform.position = new Vector3(x, 0.01f, y);
	}
	public void setZ(int newZ)
	{
		z = newZ;
	}
	public int getZ()
	{
		return z;
	}
	public void setPosition(Vector2 pos)
	{
		x = Mathf.FloorToInt(pos.x);
		y = Mathf.FloorToInt(pos.y);
	}
	public int getX()
	{
		return x;
	}
	public int getY()
	{
		return y;
	} 
	public Vector2 getPos()
	{
		return new Vector2(x, y);
	}
	public Vector3 getVectorPostion()
	{
		return new Vector3(x * gridSize, 0.01f, y * gridSize);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
