using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum discussionStatus
{
	running,
	complete
};
[System.Serializable]
public struct speaker
{
    public string name;
    public string imgHappy;
    public string imgVeryHappy;
    public string imgSerious;
    public string imgShocked;
}
[System.Serializable]
public struct line
{
    public int side;
    public int speakerIndex;
    public string emote;
    public string text;
}
[System.Serializable]
public class discussion 
{
    public speaker[] speakers;
    public line[] lines;
    public int index;
    discussion()
    {
        index = 1;
    }
    public void setIndex(int i) {
        index = i;
    }
    public line stepDiscussion()
    {
        if (index >= lines.Length)
        {
            line nullLine = new line();
            nullLine.speakerIndex = -1;
            return nullLine;
        }
        line returnLine = lines[index];
        index += 1;
        return returnLine;

    }
}


public class DiscussionSystem : MonoBehaviour {
	discussionStatus currentStatus;
    discussion conversation;

    public void show(string data)
    {
        show(Resources.Load<TextAsset>(data));
    }
    public void show(TextAsset data)
	{
        currentStatus = discussionStatus.running;
        conversation = JsonUtility.FromJson<discussion>(data.text);
        conversation.setIndex(0);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
		currentStatus = discussionStatus.running;

        line currentLine = conversation.stepDiscussion();
        displayLine(currentLine);
	}
	public void hide()
	{
		currentStatus = discussionStatus.complete;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
	}
	// Use this for initialization
	void Start () {
        currentStatus = discussionStatus.complete;
	}
    void displayLine(line data)
    {
        Transform panel,otherPanel;
        if (data.side == 0)
        {
            panel = transform.FindChild("Left");
            otherPanel = transform.FindChild("Right");
        }
        else
        {
            panel = transform.FindChild("Right");
            otherPanel = transform.FindChild("Left");
        }
        speaker talker = conversation.speakers[data.speakerIndex];
        panel.FindChild("Text").GetComponent<Text>().text = data.text;
        panel.FindChild("Name").GetComponent<Text>().text = talker.name;



        panel.SetSiblingIndex(2);
        otherPanel.SetSiblingIndex(0);

        Sprite image;
        switch (data.emote.ToLower())
        {
            case "happy":
                image = Resources.Load<Sprite>(talker.imgHappy);
                break;
            case "veryhappy":
                image = Resources.Load<Sprite>(talker.imgVeryHappy);
                break;
            case "serious":
                image = Resources.Load<Sprite>(talker.imgSerious);
                break;
            case "shocked":
                image = Resources.Load<Sprite>(talker.imgShocked);
                break;
            default:
                image = Resources.Load<Sprite>(talker.imgSerious);
                break;
        }
        panel.FindChild("Image").GetComponent<Image>().sprite = image;


    }
	// Update is called once per frame
	void Update () {
        if (currentStatus != discussionStatus.running)
            return;
        if (Input.anyKeyDown)
        {
            line currentLine = conversation.stepDiscussion();

            if (currentLine.speakerIndex == -1)
            {
                hide();
            }
            else
            {
                displayLine(currentLine);
            }
        }
	}

	public discussionStatus getStatus()
	{
		return currentStatus;
	}


}
