using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class mapScript : MonoBehaviour {
    int progression = 1 ;
    GameObject player;
    levelData levelManager;

    int target = 0;
    string[,] vertexObjects = { {"Lv1","Lv1"},{"Lv2","L2"},{"Lv3","L3"},{"Lv4","L4"},{"Lv5","L5"} };
    int[] vertexProgression = { 1, 2, 3, 3, 4 };
    Vector3[] vertexLocation;
    int[,] edges = {{-1,-1,-1,1},{3,0,4,2},{-1,1,-1,-1},{-1,-1,1,-1},{1,-1,-1,-1}};
	// Use this for initializatio
	void Start () {
		

        player = transform.FindChild("player").gameObject;
        levelManager = GameObject.Find("GameScript").GetComponent<levelData>();
        vertexLocation = new Vector3[5];
        vertexLocation[0] = transform.FindChild("Lv1").position;
        vertexLocation[1] = transform.FindChild("Lv2").position;
        vertexLocation[2] = transform.FindChild("Lv3").position;
        vertexLocation[3] = transform.FindChild("Lv4").position;
        vertexLocation[4] = transform.FindChild("Lv5").position;

		player.transform.position = vertexLocation[LevelData.playerPosition];
		target = LevelData.playerPosition;
		updateProgression(LevelData.progression);

	}
    bool atNode(){
        return Vector3.Distance(player.transform.position, vertexLocation[target]) < 1.0f;
    }
    void active(string str, bool v){transform.FindChild(str).gameObject.SetActive(v);}
    public void updateProgression(int i)
    {
        player.SetActive(true);
        progression = i;
        for(int j = 0; j < vertexLocation.Length; j++)
        {
            bool active = vertexProgression[j] <= i;
            for (int k = 0; k < 2; k++)
            {
                transform.FindChild(vertexObjects[j,k]).gameObject.SetActive(active);
            }
        }
    }
    public void hide()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
	// Update is called once per frame
	void Update () {
        if (atNode() && !levelManager.inLevel())
        {
            player.SendMessage("stopWalking");
            //If we are at node input required
            if (Input.GetKeyDown(KeyCode.W) && edges[target, 0] != -1 && progression >= vertexProgression[edges[target, 0]])
            {
                target = edges[target, 0];
            }
            else if (Input.GetKeyDown(KeyCode.A) && edges[target, 1] != -1 && progression >= vertexProgression[edges[target, 1]])
            {
                target = edges[target, 1];
            }
            else if (Input.GetKeyDown(KeyCode.S) && edges[target, 2] != -1 && progression >= vertexProgression[edges[target, 2]])
            {
                target = edges[target, 2];
            }
            else if (Input.GetKeyDown(KeyCode.D) && edges[target, 3] != -1 && progression >= vertexProgression[edges[target, 3]])
            {
                target = edges[target, 3];
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
				levelManager.playLevel(target);
				//RUN LEVEL!
				hide();
				LevelData.playerPosition = target;
                

            }
        }
        else
        {
            //If not at node go to target node
            Vector3 toTarget = vertexLocation[target] - player.transform.position; 
            toTarget.Normalize();
            player.transform.Translate(toTarget*50*Time.deltaTime);
        }
	}
}
