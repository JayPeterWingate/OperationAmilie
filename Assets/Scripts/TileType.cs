using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileType {
    [SerializeField]
    public string name;
	public Sprite img;
	public GameObject tileVisualPrefab;

    public bool isWalkable = true;
	public int coverValue;
	public int cost;
	public float height;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}