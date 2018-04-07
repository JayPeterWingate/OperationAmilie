using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageControl : MonoBehaviour {
	TextMesh mesh;
	float time, timeout;
	Vector3 initialPosition;
	float speed = 2.0f;
	// Use this for initialization
	void Start () {
		time = 0;
		timeout = 1.5f;//20;
		initialPosition = transform.localPosition;
		mesh = GetComponent<TextMesh>();	
	}
	
	// Update is called once per frame
	void Update () {
		if(time < timeout)
		{
			transform.position += new Vector3(0, 1.0f*Time.deltaTime, 0);

			time+= Time.deltaTime;
			if(time >= timeout)
			{
                transform.localPosition = initialPosition;
                mesh.text = "";
			}
		}	
		
	}

	public void displayMessage(string message, Color colour)
	{

		transform.rotation = GameObject.Find("camPosition").transform.rotation * Quaternion.Euler(new Vector3(0,45,0));
		mesh.text = message;
		mesh.color = colour;
		
		time = 0;
		transform.localPosition = initialPosition;
	}

	public bool isActive()
	{
		return time < timeout;
	}
}
