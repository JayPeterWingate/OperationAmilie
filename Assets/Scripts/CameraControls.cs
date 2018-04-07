using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {
	public Transform m_cameraTransform;
	public float m_cameraSpeed ;
	//If true animations need to be flipped and models need to be rotated 90 degrees
	bool inverted;
	// Use this for initialization
	void Start () {
		m_cameraTransform = transform.GetChild(0);
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.LeftShift))
		{
			m_cameraSpeed *= 2;
		}
		else if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			m_cameraSpeed /= 2;
		}
		

		if(Input.GetKey(KeyCode.W))
		{
			Vector3 forward = getForwardAxis();
			//transform.position += forward * m_cameraSpeed;
			transform.Translate(forward * m_cameraSpeed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.S))
		{
			Vector3 backwards = -getForwardAxis();
			transform.Translate(backwards * m_cameraSpeed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.A))
		{
			Vector3 localLeft = transform.worldToLocalMatrix.MultiplyVector(-m_cameraTransform.right);
			transform.Translate(localLeft* m_cameraSpeed*Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.D))
		{
			Vector3 localRight = transform.worldToLocalMatrix.MultiplyVector(m_cameraTransform.right);
			transform.Translate(localRight * m_cameraSpeed * Time.deltaTime);
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			transform.Rotate(new Vector3(0, 90, 0));
			inverted = !inverted;
			//GameObject.FindGameObjectWithTag("GameController").BroadcastMessage("rotateToCamera");
		}
		if (Input.GetKeyDown(KeyCode.Q))
		{
			transform.Rotate(new Vector3(0, -90, 0));
			inverted = !inverted;
		}

	}
	public bool getInverted()
	{
		return inverted;
	}
	Vector3 getForwardAxis()
	{
		Vector3 localRight = transform.worldToLocalMatrix.MultiplyVector(m_cameraTransform.right);
		return Vector3.Cross(localRight,transform.up); 
	}
}
