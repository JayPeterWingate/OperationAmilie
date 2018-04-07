using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct levelInfo
{
    public int number;
    public string lvname;
    public TextAsset openingDiscussion;
    public TextAsset closingDiscussion;
    public bool completed;
};
enum levelStatus
{
    waiting,
    inOpenning,
    inLevel,
    inClosing
};
public class levelData : MonoBehaviour {

    public levelInfo[] levels;
    DiscussionSystem discussion;
    levelStatus status;
    int currentLevel;
	bool levelComplete;
	// Use this for initialization
	void Start () {

		currentLevel = LevelData.levelId;
		levelComplete = LevelData.levelComplete;
		if (levelComplete)
		{
			GameObject.Find("Map").GetComponent<mapScript>().hide();
			status = levelStatus.inLevel;
			for (int i = 0; i < 5; i++) { levels[i].completed = LevelData.levelCompletion[i]; }
			
		}
		else
			status = levelStatus.waiting;
        discussion = GameObject.Find("DiscussionSystem").GetComponent<DiscussionSystem>();
		GameObject.Find("LoadScreen").transform.GetChild(0).gameObject.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {

        switch(status)
        {
            case levelStatus.inOpenning:
				
                if (discussion.getStatus() == discussionStatus.complete)
                {
					GameObject.Find("LoadScreen").transform.GetChild(0).gameObject.SetActive(true);
					LevelData.LevelName = levels[currentLevel].lvname;
					LevelData.levelId = currentLevel;
					TextAsset mapFile = Resources.Load(LevelData.getLevelPath()) as TextAsset;
					LevelData.levelCompletion = levelCompletion();
					LevelData.levelId = currentLevel;
					if (mapFile != null)
					{
						LevelData.newMap = false;

						switch(currentLevel)
						{
							case 0:
								LevelData.party[0] = 0;
								LevelData.party[2] = 1;
								break;
							case 2:
								LevelData.party[3] = 1;
								break;
							case 3:
								LevelData.party[4] = 1;
								break;
						}

						SceneManager.LoadScene("_scene");
					}
                }
                break;
			case levelStatus.inLevel:
				discussion.show(levels[currentLevel].closingDiscussion);
				if(currentLevel == 3)
				{
					LevelData.party[5] = 1;
				}
				levels[currentLevel].completed = true;

				status = levelStatus.inClosing;
				break;
			case levelStatus.inClosing:
				LevelData.levelComplete = false;

				if (discussion.getStatus() == discussionStatus.complete)
                {
                    status = levelStatus.waiting;
                    GameObject.Find("Map").GetComponent<mapScript>().updateProgression(calculateProgression());

                }
                break;
        }
	}

    public void playLevel(int i)
    {
        discussion.show(levels[i].openingDiscussion);
        status = levelStatus.inOpenning;
        currentLevel = i;
    }
    public bool[] levelCompletion()
    {
        return new List<levelInfo>(levels).Select(e => e.completed).ToArray();
    }
    public bool inLevel()
    {
        return !(status == levelStatus.waiting);
    }
    int calculateProgression()
    {
        int progression = 1;
        print(progression);
        if (levels[0].completed)
            progression++;
        print(progression);
        if (levels[1].completed)
            progression++;
        print(progression);
        if (levels[2].completed && levels[3].completed)
            progression++;
        print(progression);
        return progression;
    }
}
