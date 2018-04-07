using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seeThroughMesh : MonoBehaviour
{
	public float reducedSize = 0.2f;
	public bool hide;
	public float hideDistance = 5.0f;
	bool visible = true;
	float speed = 4;

	GameObject camera;
	Material normal, seeThrough;
	// Use this for initialization
	void Start()
	{
		camera = GameObject.Find("camPosition");
		if (hide == true)
		{
			normal = gameObject.GetComponent<Renderer>().material;
			seeThrough = Resources.Load("Tiles/Materials/SeeThrough", typeof(Material)) as Material;
			

			setVisible(true);
		}
		
	}
	
	// Update is called once per frame
	void Update()
	{
		float distanceToCamera = Vector3.Distance(camera.transform.position, transform.position);
		if(distanceToCamera < hideDistance && visible == true)
		{
			// Lower wall
			transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1, (1.0f-Time.deltaTime*speed), 1));
			transform.localScale = new Vector3(1, Mathf.Min(Mathf.Max(transform.localScale.y, reducedSize), 1), 1);
			if(transform.localScale.y == reducedSize)
			{
				visible = false;
			}
		}	
		else if (distanceToCamera > hideDistance && visible == false)
		{
			//raise wall
			transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1, 1.0f+(Time.deltaTime*speed), 1));
			transform.localScale = new Vector3(1, Mathf.Min(transform.localScale.y, 1), 1);
			if (transform.localScale.y == 1)
			{
				visible = true;
			}
		}
		
	}

	void setVisible(bool flag)
	{
		if (flag != visible && !flag)
		{
			gameObject.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			//transform.GetChild(0).gameObject.SetActive(false);
			//gameObject.GetComponent<Renderer>().material = seeThrough;
		}
		else if(flag != visible)
		{
			gameObject.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			//transform.GetChild(0).gameObject.SetActive(true);
			//gameObject.GetComponent<Renderer>().material = normal;
		}
		visible = flag;
	}
	private void OnTriggerEnter(Collider other)
	{
		// -0.384
		//cube -0.497
	}
	/*private void OnTriggerStay(Collider other)
	{
		
		if (other.gameObject == GameObject.Find("camPosition").transform.GetChild(1).gameObject)
		{
			float dist = Vector3.Distance(transform.position, GameObject.Find("camPosition").transform.GetChild(1).position);
			if (hide == true)
			{
				if (visible == true && dist < 6.0f)
				{
					setVisible(false);
				}
				else if(visible == false && dist > 6.0f)
				{
					setVisible(true);
				}
			}
			else
			{
				// reduce y scale linarly based on distance
				float scaleY = dist / 7.0f; //Mathf.Min(Mathf.Max(dist / 4.0f,0.2f),1);
											// min 0.1 max 1
				transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1, scaleY, 1));
				transform.localScale = new Vector3(1, Mathf.Min(Mathf.Max(transform.localScale.y, reducedSize), 1), 1);
			}
			
		}
	}*/
	private void OnTriggerExit(Collider other)
	{
		/*
		if (hide == true && other.gameObject == GameObject.Find("camPosition").transform.GetChild(1).gameObject)
		{
			setVisible(true);
		}*/
	}
}