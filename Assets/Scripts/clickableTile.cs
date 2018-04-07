using UnityEngine;
using System.Collections;

public class clickableTile : MonoBehaviour {


	public bool isWalkable;
	public int coverValue;
	public int cost;

	GameObject numberRender;

	bool highlighted;
	GameObject outline;
	int x, y;
	// Use this for initialization
	void Start () {
		outline = (GameObject)Instantiate(Resources.Load("Tiles/TileOutline"), transform, true);
		outline.transform.position = transform.position + new Vector3(0, 0.60f, 0);

		unHilight();
	}

	// Update is called once per frame
	void Update () {
	
	}
	public bool getHighlighted()
	{
		return highlighted;
	}
	public void HighLight(Color highlightColor)
	{
		highlighted = true;
		Renderer rend = outline.GetComponent<Renderer>();
		rend.material.color = highlightColor;

		rend.material.SetColor("_EmissionColor", highlightColor);
		rend.material.EnableKeyword("_Emission");
		outline.SetActive(true);
	}
	
	public void unHilight()
	{
		highlighted = false;
		outline.SetActive(false);
	}
    void OnMouseUp()
    {

    }
	public void updateValues(TileType type)
	{
		isWalkable = type.isWalkable;
		cost = type.cost;
		coverValue = type.coverValue;

	}
    public void showValue(string value)
    {
		if(numberRender == null )
		{
			numberRender = new GameObject();
			numberRender.transform.parent = this.transform;
			numberRender.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
			numberRender.transform.localPosition = new Vector3(0, 1, 0);
			numberRender.AddComponent<TextMesh>();
			numberRender.AddComponent<MeshRenderer>();
		}

		TextMesh textMesh = numberRender.GetComponent<TextMesh>();
		

		textMesh.text = value;
        textMesh.fontSize = 100;
    }
	public void hideValue()
	{
		if(numberRender != null)
		{
			numberRender.GetComponent<TextMesh>().text = "";
		}
	}
}
