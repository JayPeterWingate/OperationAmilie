using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class editorLoop : MonoBehaviour {
	int panelIndex;
	int index,lastIndex;
	int rotation = 0;

	Vector2[] tempoaryTrigger;
	int tempTriggerIndex = 0;
	Vector2 renderedTriggerMin;
	Vector2 renderedTriggerMax;

	// Use this for initialization
	void Start () {
		tempoaryTrigger = new Vector2[2];
		tempoaryTrigger[0] = new Vector2(-1,-1);
		tempoaryTrigger[1] = new Vector2(-1, -1);
	}

	// Update is called once per frame
	void Update() {

		if (Input.GetKeyDown(KeyCode.Z))
		{
			print("+");
			rotation += 90;
		}
		if(Input.GetKeyDown(KeyCode.C))
		{
			print("-");
			rotation -= 90;
		}
		if (Input.GetMouseButton(0))
		{
			RaycastHit hit;

			// Check if over UI
			if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
			{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit,Mathf.Infinity,(1<<8)))
				{
					GameObject objectHit = hit.collider.gameObject;
					GridItem position = objectHit.GetComponent<GridItem>();
					//if (objectHit.tag == "tile")
					{
						if(panelIndex == 0)
						{
							GetComponent<TileMap>().setTile(position.getX(), position.getY(), rotation, index);

						}
						else if (panelIndex == 1)
						{
							GetComponent<TileMap>().setObject(position.getX(), position.getY(), rotation, index);
						}
						else if(panelIndex == 2)
						{
							if(tempoaryTrigger[0].x == -1)
							{
								print("Value1 set");
								tempoaryTrigger[0] = position.getPos();
							}
							else if(tempoaryTrigger[1].x == -1 && position.getPos() != tempoaryTrigger[0])
							{
								tempoaryTrigger[1] = position.getPos();
								print("Value2 set");
								
								setTrigger();
								
							}
						}
						else if(panelIndex == 3)
						{
							GameObject.Find("EffectPanel").GetComponent<EffectPanelScript>().updateClickedPosition(position.getPos());
							panelIndex = lastIndex;

						}
						else if(panelIndex == 4)
						{
							GetComponent<TileMap>().setSpawner(position.getX(), position.getY(), index);
						}
						else if(panelIndex == 5)
						{
							GetComponent<TileMap>().setModifer(position.getX(), position.getY(), index,true);
							print("CAKE IS A LIE");
						}
					}

				}
			}
			
		}
		else if (Input.GetMouseButton(1))
		{
			RaycastHit hit;

			// Check if over UI
			if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
			{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << 8)))
				{
					GameObject objectHit = hit.collider.gameObject;
					GridItem position = objectHit.GetComponent<GridItem>();
					//if (objectHit.tag == "tile")
					{
						if (panelIndex == 0)
						{
							index = GetComponent<TileMap>().getTileIndex(position.getX(),position.getY());

						}
						else if (panelIndex == 1)
						{
							GetComponent<TileMap>().setObject(position.getX(), position.getY(), rotation, 0);
						}
						else if (panelIndex == 4)
						{
							GetComponent<TileMap>().setSpawner(position.getX(), position.getY(), 0);
						}
						else if (panelIndex == 5)
						{
							GetComponent<TileMap>().setModifer(position.getX(), position.getY(), 0, true);
							print("CAKE IS A LIE");
						}
					}

				}
			}

		}

	}
	public void setPanelIndex(int pIndex)
	{
		lastIndex = panelIndex;
		//Remove any highlights if index was 2 and now is not
		if(panelIndex == 2 && pIndex != 2)
		{
			GetComponent<TileMap>().UnHilightBox(renderedTriggerMin, renderedTriggerMax);
		}
		if(panelIndex == 5 && pIndex != 5)
		{
			print("HIDE THEM QUICK");
			GetComponent<TileMap>().hideModifers();
		}
		else if (panelIndex != 5 && pIndex == 5)
		{
			print("It's alright lads");
			GetComponent<TileMap>().showModifers();
		}
		print("index: " + pIndex);
		panelIndex = pIndex;

	}
	public void setIndex(int pIndex, int i)
	{
		lastIndex = panelIndex;
		panelIndex = pIndex;
		index = i;
	}
	public void setTrigger()
	{
		//If an old highlight exists eliminate it
		if (renderedTriggerMin != null && renderedTriggerMax != null)
		{
			GetComponent<TileMap>().UnHilightBox(renderedTriggerMin, renderedTriggerMax);
		}

		print("Generating min max");
		// Calculate new min and max
		int minX = Mathf.Min((int)tempoaryTrigger[0].x, (int)tempoaryTrigger[1].x);
		int minY = Mathf.Min((int)tempoaryTrigger[0].y, (int)tempoaryTrigger[1].y);
		int maxX = Mathf.Max((int)tempoaryTrigger[0].x, (int)tempoaryTrigger[1].x);
		int maxY = Mathf.Max((int)tempoaryTrigger[0].y, (int)tempoaryTrigger[1].y);

		renderedTriggerMin = new Vector2(minX, minY);
		renderedTriggerMax = new Vector2(maxX, maxY);
		print(tempoaryTrigger[0] + " : " + tempoaryTrigger[1]);
		print(renderedTriggerMin + " : " + renderedTriggerMax);
		//Feed this to Event panel

		positionPanelScript positionPanel = GameObject.Find("PositionPanel").GetComponent<positionPanelScript>();
		positionPanel.setTrigger(renderedTriggerMin, renderedTriggerMax);

		tempoaryTrigger[0] = new Vector2(-1, -1);
		tempoaryTrigger[1] = new Vector2(-1, -1);
	}
	public void testplay()
	{
		GetComponent<TileMap>().SendMessage("save");
		#if UNITY_EDITOR
		AssetDatabase.ImportAsset(LevelData.getSavePath());
		#endif
		SceneManager.LoadScene("_scene");
	}
	public void exit()
	{
		SceneManager.LoadScene("EditorMenu");
	}
}
